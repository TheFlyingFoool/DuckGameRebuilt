using System;
using System.Linq;
using DiscordRPC;
using System.IO;

namespace DuckGame
{
	internal class DiscordRichPresence
	{
		static DateTime whenGameStarted;

		static System.Timers.Timer updatePresence = new System.Timers.Timer(3500)
		{
			AutoReset = true,
		};

		static System.Timers.Timer tryReconnect = new System.Timers.Timer(30000)
		{
			AutoReset = true,
		};

		public static DiscordRpcClient client;

		public static bool connected;

		public static void Initialize()
		{
			client = new DiscordRpcClient("1006027196613267568"); // App registered under klof44 (for art assets)

			client.OnReady += (sender, e) =>
			{
				tryReconnect.Enabled = false;
				DevConsole.Log("|DGRED|DGREBUILT |PREV|Connected to discord", Color.LightGreen);
				connected = true;
			};
			
			client.OnConnectionFailed += (sender, e) =>
			{
				connected = false;
				tryReconnect.Start();
			};
			
			tryReconnect.Elapsed += (sender, e) => client.Initialize();
			updatePresence.Elapsed += (sender, e) => TriggerRPCUpdate();
			
			client.Initialize();

			whenGameStarted = DateTime.UtcNow;

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

			switch (Level.current)
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
					foreach (ChallengeSaveData cData in Profiles.active.FirstOrDefault().challengeData.Values)
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
					assets.SmallImageText = $"{Profiles.active.FirstOrDefault().ticketCount} Tickets";
					break;

				case ChallengeLevel:
					rpc.Details = "Playing Arcade Level";
					rpc.State = (Level.current as ChallengeLevel)._challenge.challenge.GetNameForDisplay() ?? "";
					assets.LargeImageKey = "arcade";
					assets.LargeImageText = $"Trophy: {Profiles.active.FirstOrDefault().challengeData[(Level.current as ChallengeLevel)._challenge.challenge.name].trophy}";
					assets.SmallImageKey = "ticket";
					assets.SmallImageText = $"{Profiles.active.FirstOrDefault().ticketCount} Tickets";
					break;

				case Editor:
					rpc.Details = "In Editor";
					if (Path.GetFileNameWithoutExtension((Level.current as Editor).saveName) == "")
					{
						rpc.State = "Editing Unnamed Level";
					}
					else
					{
						rpc.State = $"Editing {Path.GetFileNameWithoutExtension((Level.current as Editor).saveName)}";
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
						if ((Level.current as GameLevel).displayName == null)
						{
							rpc.State = "Random Level";
						}
						else
						{
							rpc.State = $"Playing {(Level.current as GameLevel).displayName}";
						}
						assets.LargeImageKey = "netgun"; // Placeholder Image
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

						if (Network.isActive)
                        {
							Button[] joinButton = new Button[1]
							{						
								new Button()
								{
									Label = "Join Game",
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
						assets.LargeImageText = $"{DuckNetwork.localProfile.team.score} Points";
					}

					rpc.Details += playersAndSpecCount;
					break;
			}

			rpc.Assets = assets;

			client.SetPresence(rpc);
			client.Invoke();
		}
	}
}
