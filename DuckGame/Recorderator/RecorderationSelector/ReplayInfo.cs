using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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
            /*using FileStream zipStream = new FileStream(ReplayFilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            
            byte[] previewDataRaw = Recorderator.DataFromEntry(archive.Entries[5]);

            DidLoadPreview = true;*/
        }

        public void LoadInfo()
        {
            using FileStream zipStream = new FileStream(ReplayFilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            
            byte[] hatsDataRaw = Recorderator.DataFromEntry(archive.Entries[2]);
            byte[] metadataDataRaw = Recorderator.DataFromEntry(archive.Entries[3]);

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