using AddedContent.Firebreak;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace DuckGame
{
    /// <summary>
    /// The changelogs shown in Options > Rebuilt > What's New, loaded from the GitHub releases (once per session, in the
    /// background). Whether the changelog has been read is remembered per version, the WHAT'S NEW button is pink until
    /// it's been opened on a new version.
    /// </summary>
    public static class Changelog
    {
        public const string ReleasesPage = "https://github.com/TheFlyingFoool/DuckGameRebuilt/releases";
        private const string ReleasesApi = "https://api.github.com/repos/TheFlyingFoool/DuckGameRebuilt/releases?per_page=50";
        private const int TimeoutMilliseconds = 10000;

        /// <summary>Everything after this line is left out in-game (it's invisible on GitHub), for Discord/Steam-only parts.</summary>
        private const string InGameEndMarker = "<!-- ingame-end -->";

        public enum LoadState
        {
            NotLoaded,
            Loading,
            Loaded,
            Failed,
        }

        public sealed class Release
        {
            public string tag;
            public string name;
            public string body;
            public string url;
            public DateTime published;
            public bool prerelease;
        }

        private static volatile LoadState s_state = LoadState.NotLoaded;
        private static volatile List<Release> s_releases = new List<Release>();

        private static DateTime s_loadStarted;

        /// <summary>Counts as failed once it's taken too long, the request timeout doesn't cover everything (like DNS lookups).</summary>
        public static LoadState state => s_state == LoadState.Loading && (DateTime.UtcNow - s_loadStarted).TotalMilliseconds > TimeoutMilliseconds + 2000 ? LoadState.Failed : s_state;

        /// <summary>Newest first.</summary>
        public static List<Release> releases => s_releases;

        /// <summary>True until the changelog has been opened on this version.</summary>
        public static bool unread => DGRSettings.ChangelogSeenVersion != Program.CURRENT_VERSION_ID;

        public static void MarkSeen()
        {
            if (!unread)
                return;
            DGRSettings.ChangelogSeenVersion = Program.CURRENT_VERSION_ID;
            AutoConfigHandler.SaveAll(false);
        }

        public static void ResetSeen()
        {
            DGRSettings.ChangelogSeenVersion = "";
            AutoConfigHandler.SaveAll(false);
        }

        /// <summary>Starts loading the releases if they haven't been loaded yet, or tries again if it failed last time.</summary>
        public static void Load()
        {
            LoadState current = state;
            if (current == LoadState.Loading || current == LoadState.Loaded)
                return;
            s_state = LoadState.Loading;
            s_loadStarted = DateTime.UtcNow;
            new Thread(Fetch) { IsBackground = true, Name = "DGR changelog" }.Start();
        }

        private static void Fetch()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ReleasesApi);
                // GitHub refuses API requests without a user agent
                request.UserAgent = "DuckGameRebuilt/" + Program.CURRENT_VERSION_ID;
                request.Accept = "application/vnd.github+json";
                request.Timeout = TimeoutMilliseconds;
                request.ReadWriteTimeout = TimeoutMilliseconds;
                string json;
                using (WebResponse response = request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    json = reader.ReadToEnd();

                s_releases = ParseReleases(json);
                s_state = LoadState.Loaded;
            }
            catch (Exception e)
            {
                DevConsole.Log("Couldn't load the GitHub releases: " + e.Message, Colors.DGRed);
                s_state = LoadState.Failed;
            }
        }

        /// <summary>Reads the GitHub API's list of releases (newest first), leaving out drafts.</summary>
        public static List<Release> ParseReleases(string json)
        {
            List<Release> releases = new List<Release>();
            foreach (JToken token in JArray.Parse(json))
            {
                if ((bool?)token["draft"] == true)
                    continue;
                string tag = (string)token["tag_name"] ?? "";
                string name = (string)token["name"];
                releases.Add(new Release
                {
                    tag = tag,
                    name = string.IsNullOrWhiteSpace(name) ? tag : name.Trim(),
                    body = Clean((string)token["body"] ?? ""),
                    url = (string)token["html_url"] ?? ReleasesPage,
                    published = (DateTime?)token["published_at"] ?? DateTime.MinValue,
                    prerelease = (bool?)token["prerelease"] == true,
                });
            }
            return releases;
        }

        private static string Clean(string markdown)
        {
            markdown = markdown.Replace("\r\n", "\n").Replace('\r', '\n');
            int end = markdown.IndexOf(InGameEndMarker, StringComparison.OrdinalIgnoreCase);
            if (end >= 0)
                markdown = markdown.Substring(0, end);
            markdown = Regex.Replace(markdown, "<!--.*?-->", "", RegexOptions.Singleline);
            // The release title ("# Released v1.4.8") is replaced by our own heading
            markdown = Regex.Replace(markdown, @"\A\s*#{1,6}[ \t]*Released\b[^\n]*\n?", "", RegexOptions.IgnoreCase);
            return markdown.Trim('\n', ' ', '\t');
        }
    }

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Marks the What's New changelog (Options > Rebuilt) as not read yet, for testing")]
        public static void ResetChangelog()
        {
            Changelog.ResetSeen();
            DevConsole.Log("The changelog is marked as unread again", Color.White);
        }
    }
}
