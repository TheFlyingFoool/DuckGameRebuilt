using System;
using System.Linq;
using DiscordRPC;
using System.IO;
using System.Globalization;

namespace DuckGame
{
    internal class DiscordRichPresence
    {
        public static DateTime whenGameStarted;

        private static System.Timers.Timer updatePresence;

        private static System.Timers.Timer tryReconnect;

        public static DiscordRpcClient client { get; private set;}

        public static bool connected;

        public static bool noRPC = false;

        public static void Initialize()
        {
            if (noRPC)
            {
                return;
            }

            client = new DiscordRpcClient("1006027196613267568"); // klof44, Dan and Firebreak have access

            tryReconnect = new System.Timers.Timer(30000)
            {
                AutoReset = true
            };
            updatePresence = new System.Timers.Timer(3500)
            {
                AutoReset = true
            };

            client.OnReady += (sender, e) =>
            {
                tryReconnect.Enabled = false;
                DevConsole.Log("|PINK|DGR |PREV|Connected to discord");
                connected = true;
            };

            client.OnConnectionFailed += (sender, e) =>
            {
                connected = false;
                tryReconnect.Start();
            };

            tryReconnect.Elapsed += (sender, e) =>
            {
                // FOR THE LOVE OF ALLAH DONT FORGET YOUR CHECKS - Firebreak
                if (!client.IsDisposed && !client.IsInitialized)
                    client.Initialize();
            };
            updatePresence.Elapsed += (sender, e) => TriggerRPCUpdate();

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "Main Menu",
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "Duck Game Rebuilt",
                },
                Timestamps = new Timestamps(whenGameStarted)
            });

            updatePresence.Start();
        }

        public static void Deinitialize()
        {
            updatePresence.Stop();
            tryReconnect.Stop();
            client.ClearPresence();
            client.Deinitialize();
            client.Dispose();
        }
        public static string ToTitleCase(string str)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str);
        }
        public static void TriggerRPCUpdate()
        {
            RichPresence rpc = new RichPresence()
            {
                Timestamps = new Timestamps(whenGameStarted)
            };

            Assets assets = new Assets()
            {
                LargeImageKey = "beam",
                SmallImageKey = "icon",
                SmallImageText = "Duck Game Rebuilt",
            };
            Profile ActiveProfile = Profiles.active.FirstOrDefault();
            Level CurrentLevel = Level.current;
            switch (CurrentLevel)
            {
                case TitleScreen:
                    rpc.Details = "Main Menu";
                    rpc.State = "";
                    assets.LargeImageKey = "beam";
                    break;

                case ArcadeLevel:
                    rpc.Details = "In Arcade";
                    int devs = 0;
                    int total = 0;
                    foreach (ChallengeSaveData cData in ActiveProfile.challengeData.Values)
                    {
                        if (cData.trophy == TrophyType.Developer)
                            devs++;

                        if (cData.trophy != TrophyType.Baseline)
                            total++;
                    }
                    rpc.State = $"{devs} Dev medals - {total} total";
                    assets.LargeImageKey = "arcade";
                    assets.LargeImageText = $"Arcade";
                    assets.SmallImageKey = "ticket";
                    assets.SmallImageText = $"{ActiveProfile.ticketCount} Tickets";
                    break;

                case ChallengeLevel:
                    ChallengeLevel challengeLevel = CurrentLevel as ChallengeLevel;
                    rpc.Details = "Playing Arcade Level";
                    if (challengeLevel._challenge != null)
                    {
                        rpc.State = ToTitleCase(challengeLevel._challenge.challenge.name);
                    }
                    else
                    {
                        rpc.State = "";
                    }
                    assets.LargeImageKey = "arcade";
                    assets.LargeImageText = $"Trophy: {ActiveProfile.challengeData[Level.current.level].trophy}";
                    assets.SmallImageKey = "ticket";
                    assets.SmallImageText = $"{ActiveProfile.ticketCount} Tickets";
                    break;
                case Editor:
                    Editor editorLevel = CurrentLevel as Editor;
                    rpc.Details = "In Editor";
                    if (Path.GetFileNameWithoutExtension(editorLevel.saveName) == "")
                    {
                        rpc.State = "Editing Unnamed Level";
                    }
                    else
                    {
                        rpc.State = $"Editing {Path.GetFileNameWithoutExtension(editorLevel.saveName)}";
                    }
                    assets.LargeImageKey = "editor";
                    break;

                default:
                    if (!Network.isActive)
                    {
                        rpc.Details = $"Local Game";
                    }
                    int spectators = 0;
                    foreach (Profile p in Profiles.active)
                    {
                        if (p.spectator)
                            spectators++;
                    }
                    string playersAndSpecCount;
                    playersAndSpecCount = $" ({Profiles.active.Count - spectators} Players)";
                    if (spectators > 0)
                    {
                        playersAndSpecCount = $" ({Profiles.active.Count - spectators} Players - {spectators} Spectators)";
                    }
                    if (Level.current is GameLevel)
                    {
                        assets.LargeImageKey = "netgun";
                        if ((Level.current as GameLevel).displayName == null)
                        {
                            rpc.State = "Random Level";
                        }
                        else
                        {
                            if (!(Level.current as GameLevel).isCustomLevel)
                            {
                                assets.LargeImageKey = $"https://klof44.github.io/static/img/DGR/{(Level.current as GameLevel).displayName}.png";
                            }
                            rpc.State = $"Playing {(Level.current as GameLevel).displayName}";
                        }

                    }
                    if (Level.current is TeamSelect2)
                    {
                        if (spectators > 0)
                        {
                            rpc.State = "In Lobby";
                        }
                        else
                        {
                            rpc.State = "In Lobby";
                        }

                        if (Network.isActive && !Network.lanMode && DuckNetwork.lobbyType == DuckNetwork.LobbyType.Public)
                        {
                            Button[] joinButton = new Button[1]
                            {
                                new Button()
                                {
                                    Label = "Join Game", //"Join Game",
									Url = $"steam://joinlobby/312530/{Steam.lobby.id}/{Steam.user.id}"
								}
                            };
                            rpc.Buttons = joinButton;
                        }
                    }
                    if (Level.current is RockIntro || Level.current is RockScoreboard)
                    {
                        rpc.State = "In Rock Throw";
                    }
                    if (Network.isActive && DuckNetwork.localProfile != null)
                    {
                        if (DuckNetwork.localProfile.spectator)
                        {
                            assets.SmallImageKey = "spectator";
                            assets.SmallImageText = "Is Spectator";
                        }
                        if (DuckNetwork.localProfile.isHost)
                        {
                            assets.SmallImageKey = "hostcrown";
                            assets.SmallImageText = "Is Host";
                        }
                        rpc.Details = $"Online Game";
                        assets.LargeImageText = DuckNetwork.localProfile.team.score.ToString() + (DuckNetwork.localProfile.team.score == 1 ? " Point" : " Points");
                    }
                    rpc.Details += playersAndSpecCount;
                    break;
            }
            rpc.Assets = assets;
            if (!Matches(rpc, client.CurrentPresence))
            {
                client.SetPresence(rpc);
                client.Invoke();
            }
        }
        public static bool Matches(RichPresence Presence, RichPresence other)
        {
            if (!MatchesBase(Presence, other))
            {
                return false;
            }

            if ((Presence.Buttons == null) ^ (other.Buttons == null))
            {
                return false;
            }

            if (Presence.Buttons != null)
            {
                if (Presence.Buttons.Length != other.Buttons.Length)
                {
                    return false;
                }

                for (int i = 0; i < Presence.Buttons.Length; i++)
                {
                    Button button = Presence.Buttons[i];
                    Button button2 = other.Buttons[i];
                    if (button.Label != button2.Label || button.Url != button2.Url)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public static bool MatchesBase(RichPresence Presence, RichPresence other)
        {
            if (other == null)
            {
                return false;
            }

            if (Presence.State != other.State || Presence.Details != other.Details)
            {
                return false;
            }

            if (Presence.Timestamps != null)
            {
                if (other.Timestamps == null || other.Timestamps.StartUnixMilliseconds != Presence.Timestamps.StartUnixMilliseconds || other.Timestamps.EndUnixMilliseconds != Presence.Timestamps.EndUnixMilliseconds)
                {
                    return false;
                }
            }
            else if (other.Timestamps != null)
            {
                return false;
            }

            if (Presence.Secrets != null)
            {
                if (other.Secrets == null || other.Secrets.JoinSecret != Presence.Secrets.JoinSecret || other.Secrets.MatchSecret != Presence.Secrets.MatchSecret || other.Secrets.SpectateSecret != Presence.Secrets.SpectateSecret)
                {
                    return false;
                }
            }
            else if (other.Secrets != null)
            {
                return false;
            }

            if (Presence.Party != null)
            {
                if (other.Party == null || other.Party.ID != Presence.Party.ID || other.Party.Max != Presence.Party.Max || other.Party.Size != Presence.Party.Size || other.Party.Privacy != Presence.Party.Privacy)
                {
                    return false;
                }
            }
            else if (other.Party != null)
            {
                return false;
            }

            if (Presence.Assets != null)
            {
                if (other.Assets == null || other.Assets.LargeImageKey != Presence.Assets.LargeImageKey || other.Assets.LargeImageText != Presence.Assets.LargeImageText || other.Assets.SmallImageKey != Presence.Assets.SmallImageKey || other.Assets.SmallImageText != Presence.Assets.SmallImageText)
                {
                    return false;
                }
            }
            else if (other.Assets != null)
            {
                return false;
            }

            return true;//Presence.Instance == other.Instance;
        }
    }
}
