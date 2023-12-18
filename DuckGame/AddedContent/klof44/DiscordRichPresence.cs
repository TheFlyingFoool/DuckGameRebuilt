using System;
using System.Linq;
using DiscordRPC;
using System.IO;
using System.Globalization;
using DiscordRPC.Message;

namespace DuckGame
{
    internal class DiscordRichPresence
    {
        public static DateTime whenGameStarted;

        static System.Timers.Timer updatePresence = new System.Timers.Timer(3500)
        {
            AutoReset = true,
        };

        static System.Timers.Timer tryReconnect = new System.Timers.Timer(30000)
        {
            AutoReset = true,
        };

        public static DiscordRpcClient client { get; private set; }
        public static RichPresence presence = new RichPresence()
        {
            Details = "Main Menu",
            State = "",
            Assets = new Assets()
            {
                LargeImageKey = "icon",
                LargeImageText = "Duck Game Rebuilt",
            },
            Timestamps = new Timestamps(whenGameStarted)
        };

        public static bool connected;

        public static bool noRPC = false;

        public static void Initialize()
        {

            client = new DiscordRpcClient("1006027196613267568"); // klof44, Dan and Firebreak have access
            client.RegisterUriScheme("312530");
            if (noRPC)
            {
                return;
            }

            //Set the logger. This way we can see the output of the client.
            //We can set it this way, but doing it directly in the constructor allows for the Register Uri Scheme to be logged too.
            //System.IO.File.WriteAllBytes("discord-rpc.log", new byte[0]);
            //client.Logger = new Logging.FileLogger("discord-rpc.log", DiscordLogLevel);

            //Register to the events we care about. We are registering to everyone just to show off the events

            // client.OnReady += OnReady;                                      //Called when the client is ready to send presences
            client.OnClose += OnClose;                                      //Called when connection to discord is lost
            client.OnError += OnError;                                      //Called when discord has a error

            client.OnConnectionEstablished += OnConnectionEstablished;      //Called when a pipe connection is made, but not ready
                                                                            // client.OnConnectionFailed += OnConnectionFailed;                //Called when a pipe connection failed.

            client.OnPresenceUpdate += OnPresenceUpdate;                    //Called when the presence is updated

            client.OnSubscribe += OnSubscribe;                              //Called when a event is subscribed too
            client.OnUnsubscribe += OnUnsubscribe;                          //Called when a event is unsubscribed from.

            client.OnJoin += OnJoin;                                        //Called when the client wishes to join someone else. Requires RegisterUriScheme to be called.
            client.OnSpectate += OnSpectate;                                //Called when the client wishes to spectate someone else. Requires RegisterUriScheme to be called.
            client.OnJoinRequested += OnJoinRequested;                      //Called when someone else has requested to join this client.
            //Before we send a initial presence, we will generate a random "game ID" for this example.
            // For a real game, this "game ID" can be a unique ID that your Match Maker / Master Server generates. 
            // This is used for the Join / Specate feature. This can be ignored if you do not plan to implement that feature.
            presence.Secrets = new Secrets()
            {
                //These secrets should contain enough data for external clients to be able to know which
                // game to connect too. A simple approach would be just to use IP address, but this is highly discouraged
                // and can leave your players vulnerable!
                JoinSecret = "join_myuniquegameid",
                SpectateSecret = "spectate_myuniquegameid"
            };

            //We also need to generate a initial party. This is because Join requires the party to be created too.
            // If no party is set, the join feature will not work and may cause errors within the discord client itself.


            //Give the game some time so we have a nice countdown
            presence.Timestamps = new Timestamps()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow + TimeSpan.FromSeconds(15)
            };

            //Subscribe to the join / spectate feature.
            //These require the RegisterURI to be true.
            client.SetSubscription(EventType.Join | EventType.Spectate | EventType.JoinRequest);        //This will alert us if discord wants to join a game

            // End
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
            presence.Timestamps = new Timestamps(whenGameStarted);
            client.SetPresence(presence);

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
            presence.Assets.LargeImageKey = "beam";
            presence.Assets.SmallImageKey = "icon";
            presence.Assets.SmallImageText = "Duck Game Rebuilt";
            Profile ActiveProfile = Profiles.active.FirstOrDefault();
            Level CurrentLevel = Level.current;
            switch (CurrentLevel)
            {
                case TitleScreen:
                    presence.Details = "Main Menu";
                    presence.State = "";
                    presence.Assets.LargeImageKey = "beam";
                    break;

                case ArcadeLevel:
                    presence.Details = "In Arcade";
                    int devs = 0;
                    int total = 0;
                    foreach (ChallengeSaveData cData in ActiveProfile.challengeData.Values)
                    {
                        if (cData.trophy == TrophyType.Developer)
                            devs++;

                        if (cData.trophy != TrophyType.Baseline)
                            total++;
                    }
                    presence.State = $"{devs} Dev medals - {total} total";
                    presence.Assets.LargeImageKey = "arcade";
                    presence.Assets.LargeImageText = $"Arcade";
                    presence.Assets.SmallImageKey = "ticket";
                    presence.Assets.SmallImageText = $"{ActiveProfile.ticketCount} Tickets";
                    break;

                case ChallengeLevel:
                    ChallengeLevel challengeLevel = CurrentLevel as ChallengeLevel;
                    presence.Details = "Playing Arcade Level";
                    if (challengeLevel._challenge != null)
                    {
                        presence.State = ToTitleCase(challengeLevel._challenge.challenge.name);
                    }
                    else
                    {
                        presence.State = "";
                    }
                    presence.Assets.LargeImageKey = "arcade";
                    presence.Assets.LargeImageText = $"Trophy: {ActiveProfile.challengeData[Level.current.level].trophy}";
                    presence.Assets.SmallImageKey = "ticket";
                    presence.Assets.SmallImageText = $"{ActiveProfile.ticketCount} Tickets";
                    break;
                case Editor:
                    Editor editorLevel = CurrentLevel as Editor;
                    presence.Details = "In Editor";
                    if (Path.GetFileNameWithoutExtension(editorLevel.saveName) == "")
                    {
                        presence.State = "Editing Unnamed Level";
                    }
                    else
                    {
                        presence.State = $"Editing {Path.GetFileNameWithoutExtension(editorLevel.saveName)}";
                    }
                    presence.Assets.LargeImageKey = "editor";
                    break;

                default:
                    if (!Network.isActive)
                    {
                        presence.Details = $"Local Game";
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
                        presence.Assets.LargeImageKey = "netgun";
                        if ((Level.current as GameLevel).displayName == null)
                        {
                            presence.State = "Random Level";
                        }
                        else
                        {
                            if (!(Level.current as GameLevel).isCustomLevel)
                            {
                                presence.Assets.LargeImageKey = $"https://klof44.github.io/static/img/DGR/{(Level.current as GameLevel).displayName}.png";
                            }
                            presence.State = $"Playing {(Level.current as GameLevel).displayName}";
                        }

                    }
                    if (Level.current is TeamSelect2)
                    {
                        if (spectators > 0)
                        {
                            presence.State = "In Lobby";
                        }
                        else
                        {
                            presence.State = "In Lobby";
                        }

                        if (Network.isActive && !Network.lanMode)
                        {
                            Button[] joinButton = new Button[1]
                            {
                                new Button()
                                {
                                    Label = "Join Game", //"Join Game",
									Url = $"steam://joinlobby/312530/{Steam.lobby.id}/{Steam.user.id}"
                                }
                            };
                            presence.Buttons = joinButton;
                        }
                    }
                    if (Level.current is RockIntro || Level.current is RockScoreboard)
                    {
                        presence.State = "In Rock Throw";
                    }
                    if (Network.isActive && DuckNetwork.localProfile != null)
                    {
                        if (DuckNetwork.localProfile.spectator)
                        {
                            presence.Assets.SmallImageKey = "spectator";
                            presence.Assets.SmallImageText = "Is Spectator";
                        }
                        if (DuckNetwork.localProfile.isHost)
                        {
                            presence.Assets.SmallImageKey = "hostcrown";
                            presence.Assets.SmallImageText = "Is Host";
                        }
                        presence.Details = $"Online Game";
                        presence.Assets.LargeImageText = DuckNetwork.localProfile.team.score.ToString() + (DuckNetwork.localProfile.team.score == 1 ? " Point" : " Points");
                    }
                    presence.Details += playersAndSpecCount;
                    break;
            }
            if (!Matches(presence, client.CurrentPresence))
            {
                client.SetPresence(presence);
                client.Invoke();
            }
        }
        public static bool Matches(RichPresence Presence, RichPresence other)
        {
            if (!DiscordRichPresence.MatchesBase(Presence, other))
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
        #region Events

        #region State Events
        private static void OnReady(object sender, ReadyMessage args)
        {
            //This is called when we are all ready to start receiving and sending discord events. 
            // It will give us some basic information about discord to use in the future.

            //DEBUG: Update the presence timestamp
            presence.Timestamps = Timestamps.Now;

            //It can be a good idea to send a inital presence update on this event too, just to setup the inital game state.
            Console.WriteLine("On Ready. RPC Version: {0}", args.Version);

        }
        private static void OnClose(object sender, CloseMessage args)
        {
            //This is called when our client has closed. The client can no longer send or receive events after this message.
            // Connection will automatically try to re-establish and another OnReady will be called (unless it was disposed).
            Console.WriteLine("Lost Connection with client because of '{0}'", args.Reason);
        }
        private static void OnError(object sender, ErrorMessage args)
        {
            //Some error has occured from one of our messages. Could be a malformed presence for example.
            // Discord will give us one of these events and its upto us to handle it
            Console.WriteLine("Error occured within discord. ({1}) {0}", args.Message, args.Code);
        }
        #endregion

        #region Pipe Connection Events
        private static void OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            //This is called when a pipe connection is established. The connection is not ready yet, but we have at least found a valid pipe.
            Console.WriteLine("Pipe Connection Established. Valid on pipe #{0}", args.ConnectedPipe);
        }
        private static void OnConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            //This is called when the client fails to establish a connection to discord. 
            // It can be assumed that Discord is unavailable on the supplied pipe.
            Console.WriteLine("Pipe Connection Failed. Could not connect to pipe #{0}", args.FailedPipe);
            //isRunning = false;
        }
        #endregion

        private static void OnPresenceUpdate(object sender, PresenceMessage args)
        {
            //This is called when the Rich Presence has been updated in the discord client.
            // Use this to keep track of the rich presence and validate that it has been sent correctly.
            Console.WriteLine("Rich Presence Updated. Playing {0}", args.Presence == null ? "Nothing (NULL)" : args.Presence.State);
        }

        #region Subscription Events
        private static void OnSubscribe(object sender, SubscribeMessage args)
        {
            //This is called when the subscription has been made succesfully. It will return the event you subscribed too.
            Console.WriteLine("Subscribed: {0}", args.Event);
        }
        private static void OnUnsubscribe(object sender, UnsubscribeMessage args)
        {
            //This is called when the unsubscription has been made succesfully. It will return the event you unsubscribed from.
            Console.WriteLine("Unsubscribed: {0}", args.Event);
        }
        #endregion

        #region Join / Spectate feature
        private static void OnJoin(object sender, JoinMessage args)
        {
            /*
			 * This is called when the Discord Client wants to join a online game to play.
			 * It can be triggered from a invite that your user has clicked on within discord or from an accepted invite.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
            Console.WriteLine("Joining Game '{0}'", args.Secret);
        }

        private static void OnSpectate(object sender, SpectateMessage args)
        {   /*
			 * This is called when the Discord Client wants to join a online game to watch and spectate.
			 * It can be triggered from a invite that your user has clicked on within discord.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
            Console.WriteLine("Spectating Game '{0}'", args.Secret);
        }

        private static void OnJoinRequested(object sender, JoinRequestMessage args)
        {
            /*
			 * This is called when the Discord Client has received a request from another external Discord User to join your game.
			 * You should trigger a UI prompt to your user sayings 'X wants to join your game' with a YES or NO button. You can also get
			 *  other information about the user such as their avatar (which this library will provide a useful link) and their nickname to
			 *  make it more personalised. You can combine this with more API if you wish. Check the Discord API documentation.
			 *  
			 *  Once a user clicks on a response, call the Respond function, passing the message, to respond to the request.
			 *  A example is provided below.
			 *  
			 * This feature requires the RegisterURI to be true on the client.
			*/

            //We have received a request, dump a bunch of information for the user
            Console.WriteLine("'{0}' has requested to join our game.", args.User.ToString());
            Console.WriteLine(" - User's Avatar: {0}", args.User.GetAvatarURL(DiscordRPC.User.AvatarFormat.GIF, DiscordRPC.User.AvatarSize.x2048));
            Console.WriteLine(" - User's Username: {0}", args.User.Username);
            Console.WriteLine(" - User's Snowflake: {0}", args.User.ID);
            Console.WriteLine();

            //Ask the user if they wish to accept the join request.
            Console.Write("Do you give this user permission to join? [Y / n]: ");
            bool accept = Console.ReadKey().Key == ConsoleKey.Y; Console.WriteLine();

            //Tell the client if we accept or not.
            DiscordRpcClient client = (DiscordRpcClient)sender;
            client.Respond(args, accept);

            //All done.
            Console.WriteLine(" - Sent a {0} invite to the client {1}", accept ? "ACCEPT" : "REJECT", args.User.Username);
        }
        #endregion

        #endregion

    }
}
