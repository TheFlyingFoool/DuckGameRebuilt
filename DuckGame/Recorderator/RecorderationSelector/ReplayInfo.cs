using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static DuckGame.RecorderationSelector;

namespace DuckGame
{
    public class ReplayInfo : IRMenuItem
    {
        public string DisplayName => !RealName.StartsWith("cord")
            ? Path.GetFileNameWithoutExtension(ReplayFilePath)
            : $"{CreationTime:ddd yyyy-MM-dd hh:mm:ss}";

        public string RealName;
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
        public DateTime CreationTime;
        public int FolderSub { get; set; }
        public IRMenuItem Parent { get; set; }

        public ReplayInfo(string replayFilePath)
        {
            FileInfo fileInfo = new FileInfo(replayFilePath);
            CreationTime = fileInfo.CreationTime;
            RealName = fileInfo.Name;
            
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

            using FileStream zipStream2 = new FileStream(ReplayFilePath, FileMode.OpenOrCreate);
            using ZipArchive archive2 = new ZipArchive(zipStream2, ZipArchiveMode.Update);
            
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

        public void LoadInfo()
        {
            using FileStream zipStream = new FileStream(ReplayFilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            
            byte[] hatsDataRaw = Recorderator.DataFromEntry(archive.Entries[2]);
            byte[] metadataDataRaw = Recorderator.DataFromEntry(archive.Entries[3]);

            if (archive.Entries.Count > 4) //was //4 QUAJILLION //what
            {
                using Stream entryStream = archive.Entries[4].Open();
                using MemoryStream current = new MemoryStream();
                entryStream.CopyTo(current);
                Preview = new Tex2D(Texture2D.FromStream(Graphics.device, current), "preview");
                
                DidLoadPreview = true;
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

        public Rectangle Draw(Vec2 position, bool selected)
        {
            if (selected)
                DrawHovered();

            Color textColor = selected ? Color.Yellow : Color.White;
            
            if (!RealName.StartsWith("cord"))
            {
                Vec2 textSize = Extensions.GetFancyStringSize(DisplayName, FONT_SIZE);
                Rectangle textBounds = new(position.x, position.y, textSize.x, textSize.y);

                Graphics.DrawFancyString(DisplayName, position, textColor, 1f, FONT_SIZE);
                
                return textBounds;
            }
            else
            {
                (string Item, int Width)[] columns = 
                {
                    ($"{CreationTime:ddd}", 20),
                    ($"{CreationTime:yyyy-MM-dd}", 52),
                    ($"{CreationTime:hh:mm:ss}", 0),
                };

                float width = columns.Sum(x => x.Width == 0 ? Graphics.GetFancyStringWidth(x.Item, scale: FONT_SIZE) : x.Width);
                Rectangle textBounds = new(position.x, position.y, width, Graphics._biosFont.height * FONT_SIZE);

                for (int i = 0, accWidth = 0; i < columns.Length; accWidth += columns[i].Width, i++)
                {
                    Vec2 drawPos = textBounds.tl + new Vec2(accWidth, 0);
                    Graphics.DrawFancyString(columns[i].Item, drawPos, textColor, 1f, FONT_SIZE);
                }

                return textBounds;
            }
        }

        private void DrawHovered()
        {
            TimeSpan matchDuration = TimeSpan.FromSeconds(MatchDurationFrames / 60f);
            string infoString =
                $"DURATION {(matchDuration.Minutes + (matchDuration.Hours * 60)):00}:{matchDuration.Seconds:00} " +
                $"HOST {HostName}\n" +
                $"PLAYERS {PlayerCount} " +
                $"SPECTATORS {SpectatorCount}";
            
            Graphics.DrawFancyString(infoString, new Vec2(195, 95f), Color.White, 1f, 0.5f);
            
            if (DidLoadPreview)
            {
                Graphics.Draw(Preview, 194, 9, 0.1f, 0.1f, -1);
            }

            for (int j = 0; j < Profiles.Count; j++)
            {
                ProfileInfo profile = Profiles[j];
                if (profile.Team != null)
                {
                    profile.Team.hat.center = Vec2.Zero;
                    if (profile.Team.metadata != null) 
                        profile.Team.hat.center = profile.Team.metadata.HatOffset.value;
                    profile.Team.hat.scale = new Vec2(0.4f);
                    profile.Team.hat.depth = -0.98f;
                    profile.Team.hat.alpha = profile.IsSpectator ? 0.5f : 1;
                    Graphics.Draw(profile.Team.hat, 191 + (j * 10), 141.6f);
                }
                profile.ChatBustDuck.depth = -0.99f;
                profile.ChatBustDuck.scale = new Vec2(0.4f);
                profile.ChatBustDuck.alpha = profile.IsSpectator ? 0.5f : 1;
                Graphics.Draw(profile.ChatBustDuck, 198 + (j * 10), 150.12f);
            }
        }

        public void UpdateHovered()
        {
            if (!DidLoadPreview)
            {
                if (FramesUntilPreviewLoad > 15)
                {
                    current.ReplayToLoadPreview = this;
                }
                else
                {
                    FramesUntilPreviewLoad++;
                }

                current.IsLoadingReplayPreview = true;
            }
            else
            {
                current.IsLoadingReplayPreview = false;
            }
        }

        public void OnHover()
        {
            if (!DidLoadInfo)
                LoadInfo();
        }

        public void OnUnhover() { }
        public void OnSelect()
        {
            current.ReplayToPlay = this;
        }
    }
}