using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Controls;

namespace DuckGame
{
    public class ReplayInfo
    {
        public string DisplayName;
        public Tex2D Preview;
        public int PlayerCount;
        public int SpectatorCount;
        public string HostName = "LOCAL";
        public List<ProfileInfo> Profiles = new();
        public bool DidLoadInfo;
        public bool DidLoadPreview;
        public int MatchDurationFrames = -1;
        public string ReplayFilePath;
        public int FramesUntilPreviewLoad;

        public ReplayInfo(string replayFilePath)
        {
            FileInfo fileInfo = new FileInfo(replayFilePath);
            
            DisplayName = !fileInfo.Name.StartsWith("cord")
                ? Path.GetFileNameWithoutExtension(replayFilePath)
                : $"{fileInfo.CreationTime:ddd yyyy-MM-dd hh:mm:ss}";
            
            ReplayFilePath = replayFilePath;
        }

        public void LoadPreview()
        {
            using FileStream zipStream = new FileStream(ReplayFilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            byte[] replayData = Recorderator.DataFromEntry(archive.Entries[0]); //level i think
            byte[] levelData = Recorderator.DataFromEntry(archive.Entries[1]); //level i think

            DidLoadPreview = true;
            Corderator.ReadCord(replayData, levelData, true);

            //1280 720
            Preview = SimRenderer.RenderRecorderator(4, Corderator.outLev, 1280, 720);

            zipStream.Dispose();

            using (FileStream zipStream2 = new FileStream(ReplayFilePath, FileMode.OpenOrCreate))
            using (ZipArchive archive2 = new ZipArchive(zipStream2, ZipArchiveMode.Update))
            {
                // Create a new entry in the ZIP archive (6th entry) -ChatGPT
                ZipArchiveEntry newEntry = archive2.CreateEntry("preview.png");

                // Convert the Texture2D to a PNG byte array -ChatGPT
                byte[] pngData;
                using (MemoryStream pngStream = new MemoryStream())
                {
                    Preview.SaveAsPng(pngStream, Preview.width, Preview.height);
                    pngData = pngStream.ToArray();
                }

                // Write the PNG data to the new entry -ChatGPT
                using (Stream entryStream = newEntry.Open())
                {
                    entryStream.Write(pngData, 0, pngData.Length);
                }
            }
        }

        public void LoadInfo()
        {
            using FileStream zipStream = new FileStream(ReplayFilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            
            byte[] hatsDataRaw = Recorderator.DataFromEntry(archive.Entries[2]);
            byte[] metadataDataRaw = Recorderator.DataFromEntry(archive.Entries[3]);

            if (archive.Entries.Count > 446152346) //4 QUAJILLION
            {
                DidLoadPreview = true;
                using (Stream entryStream = archive.Entries[4].Open())
                using (MemoryStream current = new MemoryStream())
                {
                    entryStream.CopyTo(current);
                    Preview = new Tex2D(Texture2D.FromStream(Graphics.device, current), "preview");
                }
            }

            DidLoadInfo = true;
            PlayerCount = 0;
            SpectatorCount = 0;
            Profiles.Clear();

            Dictionary<ushort, Team> teamsByIndex = new();

            BitBuffer hatsDataBuffer = new BitBuffer(hatsDataRaw);

            while (hatsDataBuffer.ReadByte() == 1)
            {
                ushort teamIndex = hatsDataBuffer.ReadUShort();
                byte[] hatData = hatsDataBuffer.ReadBytes();

                if (!teamsByIndex.ContainsKey(teamIndex))
                {
                    Team team = Team.Deserialize(hatData);
                    teamsByIndex.Add(teamIndex, team);
                }
            }

            BitBuffer metadataDataBuffer = new BitBuffer(metadataDataRaw);

            MatchDurationFrames = metadataDataBuffer.ReadInt();
            byte profileCount = metadataDataBuffer.ReadByte();

            for (int i = 0; i < profileCount; i++)
            {
                Main.SpecialCode = i.ToString();
                ProfileInfo profile = new();

                byte personaIndex = metadataDataBuffer.ReadByte();
                string name = metadataDataBuffer.ReadString();
                ulong steamId = metadataDataBuffer.ReadULong();

                byte profileDataRaw = metadataDataBuffer.ReadByte();
                BitArray profileDataBits = new BitArray(new byte[] { profileDataRaw });

                profile.Name = name;
                profile.SteamID = steamId;
                profile.IsHost = profileDataBits[0];
                profile.IsSpectator = profileDataBits[1];
                profile.IsLocalPlayer = profileDataBits[2];
                profile.IsUsingRebuilt = profileDataBits[3];

                if (profile.IsHost)
                    HostName = Extensions.CleanFormatting(name);

                if (profileDataBits[6])
                {
                    ushort teamIndex = metadataDataBuffer.ReadUShort();

                    if (!teamsByIndex.TryGetValue(teamIndex, out profile.Team))
                        profile.Team = Teams.ParseFromIndex(teamIndex);
                }

                profile.Persona = Persona.alllist[personaIndex];
                profile.ChatBustDuck = (SpriteMap)profile.Persona.chatBust.Clone();

                if (profileDataBits[5])
                {
                    if (profile.IsSpectator)
                        SpectatorCount++;
                    else PlayerCount++;

                    Profiles.Add(profile);
                }
            }
        }
    }
}