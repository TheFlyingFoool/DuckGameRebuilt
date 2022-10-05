using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class RockScoreboard : Level
	{
		public override string networkIdentifier
		{
			get
			{
				if (this.mode == ScoreBoardMode.ShowScores)
				{
					return "@ROCKTHROW|SHOWSCORE";
				}
				if (this.afterHighlights)
				{
					return "@ROCKTHROW|SHOWEND";
				}
				return "@ROCKTHROW|SHOWWINNER";
			}
		}

		public static Level returnLevel
		{
			get
			{
				if (RockScoreboard._returnLevel == null)
				{
					RockScoreboard._returnLevel = new GameLevel(Deathmatch.RandomLevelString("", "deathmatch"), 0, false, false);
				}
				return RockScoreboard._returnLevel;
			}
		}

		public bool afterHighlights
		{
			get
			{
				return this._afterHighlights;
			}
			set
			{
				this._afterHighlights = value;
			}
		}

		public RockScoreboard(Level r = null, ScoreBoardMode mode = ScoreBoardMode.ShowScores, bool afterHighlights = false)
		{
			this._afterHighlights = afterHighlights;
			if (Network.isServer)
			{
				RockScoreboard._returnLevel = r;
			}
			this._mode = mode;
			if (mode == ScoreBoardMode.ShowWinner)
			{
				this._state = ScoreBoardState.None;
			}
		}

		public ScoreBoardMode mode
		{
			get
			{
				return this._mode;
			}
		}

		public Vec3 fieldAddColor
		{
			set
			{
				if (this._field != null)
				{
					this._field.colorAdd = value;
					this._fieldForeground.colorAdd = value;
					this._fieldForeground2.colorAdd = value;
					this._wall.colorAdd = value;
				}
			}
		}

		public Vec3 fieldMulColor
		{
			set
			{
				if (this._field != null)
				{
					this._field.colorMul = value;
					this._fieldForeground.colorMul = value;
					this._fieldForeground2.colorMul = value;
					this._wall.colorMul = value;
				}
			}
		}

		public ContinueCountdown netCountdown
		{
			get
			{
				return this._netCountdown;
			}
		}

		public float cameraY
		{
			get
			{
				return base.camera.y;
			}
			set
			{
				base.camera.y = value;
				this._field.ypos = base.camera.y * 1.4f;
			}
		}

		public int controlMessage
		{
			get
			{
				return this._controlMessage;
			}
			set
			{
				if (this._controlMessage != value)
				{
					HUD.CloseAllCorners();
					if (value == 0)
					{
						HUD.AddCornerControl(HUDCorner.BottomRight, "@START@SKIP", null, false);
					}
					else if (value > 0)
					{
						if (!Network.isServer)
						{
							this._continueHUD = HUD.AddCornerMessage(HUDCorner.BottomRight, "WAITING");
						}
						else
						{
							this._continueHUD = HUD.AddCornerControl(HUDCorner.BottomRight, "@START@CONTINUE", null, false);
							if (value > 1)
							{
								HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@QUIT", null, false);
								HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@LOBBY", null, false);
							}
						}
					}
				}
				this._controlMessage = value;
			}
		}

		public void SetWeather(Weather w)
		{
			if (this._weather != null)
			{
				this._weather.SetWeather(w);
			}
		}

		public override void SendLevelData(NetworkConnection c)
		{
			if (Network.isServer)
			{
				Send.Message(new NMCrowdData(this._crowd.NetSerialize()), c);
				Send.Message(new NMWeatherData(this._weather.NetSerialize()), c);
			}
		}

		public override void OnMessage(NetMessage message)
		{
			if (message is NMCrowdData && Network.isClient)
			{
				this._crowd.NetDeserialize((message as NMCrowdData).data);
			}
			if (message is NMWeatherData && Network.isClient)
			{
				this._weather.NetDeserialize((message as NMWeatherData).data);
			}
		}

		public InputProfile GetNetInput(sbyte index)
		{
			if (index >= this._inputs.Count || this._inputs[index].duckProfile == null || this._inputs[index].duckProfile.inputProfile == null)
			{
				return new InputProfile("");
			}
			return this._inputs[index].duckProfile.inputProfile;
		}

		public override void Initialize()
		{
			if (Network.isActive && Network.isServer && this._mode == ScoreBoardMode.ShowScores)
			{
				int idx = 0;
				foreach (Profile p in DuckNetwork.profiles)
				{
					if (p.connection != null && p.slotType != SlotType.Spectator)
					{
						InputObject o = new InputObject();
						o.profileNumber = (sbyte)idx;
						Level.Add(o);
						this._inputs.Add(o);
						idx++;
					}
				}
			}
			HighlightLevel.didSkip = false;
			if (this._afterHighlights)
			{
				this._skipFade = true;
			}
			this._weather = new RockWeather(this);
			this._weather.Start();
			Level.Add(this._weather);
			for (int i = 0; i < 350; i++)
			{
				this._weather.Update();
			}
			if (RockScoreboard._sunEnabled)
			{
				float aspect = 0.5625f;
				this._sunshineTarget = new RenderTarget2D(Graphics.width / 12, (int)(Graphics.width * aspect) / 12, false);
				this._screenTarget = new RenderTarget2D(Graphics.width, (int)(Graphics.width * aspect), false);
				this._pixelTarget = new RenderTarget2D(160, (int)(320f * aspect / 2f), false);
				this._sunLayer = new Layer("SUN LAYER", 99999, null, false, default(Vec2));
				Layer.Add(this._sunLayer);
				Thing tthing = new SpriteThing(150f, 120f, new Sprite("sun", 0f, 0f));
				tthing.z = -9999f;
				tthing.depth = -0.99f;
				tthing.layer = this._sunLayer;
				tthing.xscale = 1f;
				tthing.yscale = 1f;
				tthing.collisionSize = new Vec2(1f, 1f);
				tthing.collisionOffset = new Vec2(0f, 0f);
				Level.Add(tthing);
				this.sunThing = tthing;
				SpriteThing rbow = new SpriteThing(150f, 80f, new Sprite("rainbow", 0f, 0f));
				rbow.alpha = 0.15f;
				rbow.z = -9999f;
				rbow.depth = -0.99f;
				rbow.layer = this._sunLayer;
				rbow.xscale = 1f;
				rbow.yscale = 1f;
				rbow.color = new Color(100, 100, 100);
				rbow.collisionSize = new Vec2(1f, 1f);
				rbow.collisionOffset = new Vec2(0f, 0f);
				Level.Add(rbow);
				this.rainbowThing = rbow;
				this.rainbowThing.visible = false;
				SpriteThing rbow2 = new SpriteThing(150f, 80f, new Sprite("rainbow", 0f, 0f));
				rbow2.z = -9999f;
				rbow2.depth = -0.99f;
				rbow2.layer = this._sunLayer;
				rbow2.xscale = 1f;
				rbow2.yscale = 1f;
				rbow2.color = new Color(255, 255, 255, 90);
				rbow2.collisionSize = new Vec2(1f, 1f);
				rbow2.collisionOffset = new Vec2(0f, 0f);
				Level.Add(rbow2);
				this.rainbowThing2 = rbow2;
				this.rainbowThing2.visible = false;
			}
			List<Team> random = Teams.allRandomized;
			if (!Network.isActive && RockScoreboard.returnLevel == null)
			{
				random[0].Join(Profiles.DefaultPlayer1, true);
				random[1].Join(Profiles.DefaultPlayer2, true);
				random[0].score = 10;
				random[1].score = 2;
				Teams.Player3.score = 3;
				Teams.Player4.score = 4;
			}
			this._crowd = new Crowd();
			Level.Add(this._crowd);
			Crowd.mood = Mood.Calm;
			this._field = new FieldBackground("FIELD", 9999);
			Layer.Add(this._field);
			this._bleacherSeats = new Sprite("bleacherSeats", 0f, 0f);
			if (RockWeather.weather == Weather.Snowing)
			{
				this._bleachers = new Sprite("bleacherBackSnow", 0f, 0f);
			}
			else
			{
				this._bleachers = new Sprite("bleacherBack", 0f, 0f);
			}
			this._bleachers.center = new Vec2(this._bleachers.w / 2, this._bleachers.height - 3);
			this._intermissionText = new Sprite("rockThrow/intermission", 0f, 0f);
			this._winnerPost = new Sprite("rockThrow/winnerPost", 0f, 0f);
			this._winnerBanner = new Sprite("rockThrow/winnerBanner", 0f, 0f);
			this._font = new BitmapFont("biosFont", 8, -1);
			List<Team> teams = new List<Team>();
			foreach (Team t in Teams.all)
			{
				if (t.activeProfiles.Count > 0)
				{
					teams.Add(t);
				}
			}
			int count = 0;
			foreach (Team t2 in Teams.all)
			{
				count += t2.activeProfiles.Count;
			}
			if (this._mode == ScoreBoardMode.ShowWinner)
			{
				Vote.ClearVotes();
			}
			foreach (Team tea in teams)
			{
				tea.rockScore = tea.score;
				if (RockScoreboard.wallMode && this._mode == ScoreBoardMode.ShowScores)
				{
					tea.score = Math.Min(tea.score, GameMode.winsPerSet);
				}
				if (this._mode != ScoreBoardMode.ShowWinner && !this._afterHighlights)
				{
					foreach (Profile who in tea.activeProfiles)
					{
						Vote.RegisterVote(who, VoteType.None);
					}
				}
			}
			if (Network.isActive)
			{
				Level.Add(new HostTable(160f, 170f));
			}
			bool smallMode = teams.Count > 4;
			if (this._mode == ScoreBoardMode.ShowScores)
			{
				this._intermissionSlide = 1f;
				Graphics.fade = 1f;
				Layer.Game.fade = 0f;
				Layer.Background.fade = 0f;
				Crowd.UpdateFans();
				int index = 0;
				int highestScore = 0;
				foreach (Team t3 in teams)
				{
					float xpos = 223f;
					float ypos = 0f;
					float mul = 26f;
					if (index % 4 == 1)
					{
						mul = 24f;
					}
					else if (index % 4 == 2)
					{
						mul = 27f;
					}
					else if (index % 4 == 3)
					{
						mul = 30f;
					}
					float zpos = 158f - index % 4 * mul;
					if (index > 3)
					{
						zpos -= 12f;
					}
					Depth deep = zpos / 200f;
					int dif = t3.prevScoreboardScore;
					int total = GameMode.winsPerSet * 2;
					int rlScore = t3.score;
					if (RockScoreboard.wallMode && rlScore > GameMode.winsPerSet)
					{
						rlScore = GameMode.winsPerSet;
					}
					this._slots.Add(new Slot3D());
					if (rlScore >= GameMode.winsPerSet && rlScore == highestScore)
					{
						this._tie = true;
					}
					else if (rlScore >= GameMode.winsPerSet && rlScore > highestScore)
					{
						this._tie = false;
						highestScore = rlScore;
						this._highestSlot = this._slots[this._slots.Count - 1];
					}
					List<Profile> sortedList = new List<Profile>();
					Profile activeThrower = null;
					bool nextIsThrower = false;
					foreach (Profile p2 in t3.activeProfiles)
					{
						if (nextIsThrower)
						{
							activeThrower = p2;
							nextIsThrower = false;
						}
						if (p2.wasRockThrower)
						{
							p2.wasRockThrower = false;
							nextIsThrower = true;
						}
						sortedList.Add(p2);
					}
					if (activeThrower == null)
					{
						activeThrower = t3.activeProfiles[0];
					}
					sortedList.Remove(activeThrower);
					sortedList.Insert(0, activeThrower);
					activeThrower.wasRockThrower = true;
					byte plane = (byte)(this._slots.Count - 1);
					int duckPos = 0;
					foreach (Profile p3 in sortedList)
					{
						if (p3 == activeThrower)
						{
							RockScoreboard.initializingDucks = true;
							this._slots[plane].duck = new RockThrowDuck(xpos - duckPos * 10, ypos - 16f, p3);
							this._slots[plane].duck.planeOfExistence = plane;
							this._slots[plane].duck.ignoreGhosting = true;
							this._slots[plane].duck.forceMindControl = true;
							Level.Add(this._slots[plane].duck);
							this._slots[plane].duck.connection = DuckNetwork.localConnection;
							RockScoreboard.initializingDucks = false;
							TeamHat h = this._slots[this._slots.Count - 1].duck.GetEquipment(typeof(TeamHat)) as TeamHat;
							if (h != null)
							{
								h.ignoreGhosting = true;
							}
							this._slots[this._slots.Count - 1].duck.z = zpos;
							this._slots[this._slots.Count - 1].duck.depth = deep;
							this._slots[this._slots.Count - 1].ai = new DuckAI(p3.inputProfile);
							if (Network.isActive && p3.connection != DuckNetwork.localConnection)
							{
								this._slots[this._slots.Count - 1].ai._manualQuack = this.GetNetInput((sbyte)p3.networkIndex);
							}
							this._slots[this._slots.Count - 1].duck.derpMindControl = false;
							this._slots[this._slots.Count - 1].duck.mindControl = this._slots[this._slots.Count - 1].ai;
							this._slots[this._slots.Count - 1].rock = new ScoreRock(xpos + 18f + dif / (float)total * this._fieldWidth, ypos, p3);
							this._slots[this._slots.Count - 1].rock.planeOfExistence = plane;
							this._slots[this._slots.Count - 1].rock.ignoreGhosting = true;
							Level.Add(this._slots[this._slots.Count - 1].rock);
							this._slots[this._slots.Count - 1].rock.z = zpos;
							this._slots[this._slots.Count - 1].rock.depth = this._slots[this._slots.Count - 1].duck.depth + 1;
							this._slots[this._slots.Count - 1].rock.grounded = true;
							this._slots[this._slots.Count - 1].duck.isRockThrowDuck = true;
						}
						else
						{
							RockScoreboard.initializingDucks = true;
							Duck d = new RockThrowDuck(xpos - duckPos * 12, ypos - 16f, p3);
							d.forceMindControl = true;
							d.planeOfExistence = plane;
							d.ignoreGhosting = true;
							Level.Add(d);
							RockScoreboard.initializingDucks = false;
							d.depth = deep;
							d.z = zpos;
							d.derpMindControl = false;
							DuckAI ai = new DuckAI(p3.inputProfile);
							if (Network.isActive && p3.connection != DuckNetwork.localConnection)
							{
								ai._manualQuack = this.GetNetInput((sbyte)p3.networkIndex);
							}
							d.mindControl = ai;
							d.isRockThrowDuck = true;
							d.connection = DuckNetwork.localConnection;
							this._slots[this._slots.Count - 1].subDucks.Add(d);
							this._slots[this._slots.Count - 1].subAIs.Add(ai);
						}
						duckPos++;
					}
					this._slots[this._slots.Count - 1].slotIndex = index;
					this._slots[this._slots.Count - 1].startX = xpos;
					index++;
				}
				for (int j = 0; j < DG.MaxPlayers; j++)
				{
					Level.Add(new Block(-50f, 0f, 1200f, 32f, PhysicsMaterial.Default)
					{
						planeOfExistence = (byte)j
					});
				}
				if (!this._tie && highestScore > 0)
				{
					this._matchOver = true;
				}
				if (this._tie)
				{
					GameMode.showdown = true;
				}
			}
			else if (this._mode == ScoreBoardMode.ShowWinner)
			{
				Level.core.gameFinished = true;
				PurpleBlock.Reset();
				Level.core.gameInProgress = false;
				if (Teams.active.Count > 1 && !this._afterHighlights)
				{
					Global.data.matchesPlayed += 1;
					Global.WinMatch(Teams.winning[0]);
					if (Network.isActive)
					{
						using (List<Profile>.Enumerator enumerator = Teams.winning[0].activeProfiles.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.connection == DuckNetwork.localConnection)
								{
									DuckNetwork.GiveXP("Won Match", 0, 150, 4, 9999999, 9999999, 9999999);
									break;
								}
							}
						}
						if (DuckNetwork.localProfile != null && DuckNetwork.localProfile.slotType == SlotType.Spectator && DuckNetwork.profiles.Where<Profile>(x => x.connection == DuckNetwork.localConnection).Count<Profile>() == 1 && DuckNetwork._xpEarned.Count == 0)
							DuckNetwork.GiveXP("Observer Bonus", 0, 50);
					}
					DuckNetwork.finishedMatch = true;
					if (GameMode.winsPerSet > Global.data.longestMatchPlayed)
					{
						Global.data.longestMatchPlayed.valueInt = GameMode.winsPerSet;
					}
				}
				this._intermissionSlide = 0f;
				teams.Sort((a, b) =>
				{
					if (a.score == b.score)
						return 0;
					return a.score >= b.score ? -1 : 1;
				});
				float center = 160f - teams.Count * 42 / 2 + 21f;
				if (smallMode)
				{
					center = 160f - teams.Count * 24 / 2 + 12f;
				}
				foreach (Team team in Teams.all)
				{
					team.prevScoreboardScore = 0;
				}
				List<List<Team>> positions = new List<List<Team>>();
				foreach (Team t4 in teams)
				{
					int curVal = t4.score;
					bool inserted = false;
					for (int k = 0; k < positions.Count; k++)
					{
						if (positions[k][0].score < curVal)
						{
							positions.Insert(k, new List<Team>());
							positions[k].Add(t4);
							inserted = true;
							break;
						}
						if (positions[k][0].score == curVal)
						{
							positions[k].Add(t4);
							inserted = true;
							break;
						}
					}
					if (!inserted)
					{
						positions.Add(new List<Team>());
						positions.Last<List<Team>>().Add(t4);
					}
				}
				this._winningTeam = teams[0];
				this.controlMessage = 1;
				this._state = ScoreBoardState.None;
				Crowd.mood = Mood.Dead;
				bool localWin = false;
				if (!this._afterHighlights)
				{
					if (Network.isServer)
					{
						Level.Add(new FloorWindow(10f, -5f));
						Level.Add(new Trombone(10f, -15f));
						Level.Add(new Saxaphone(14f, -15f));
						Level.Add(new Trumpet(6f, -15f));
						Level.Add(new Trumpet(8f, -15f));
					}
					if (Network.isActive)
					{
						StatBinding onlineMatches = Global.data.onlineMatches;
						int num = onlineMatches.valueInt;
						onlineMatches.valueInt = num + 1;
					}
					int placeIndex = 0;
					int pedestalIndex = 0;
					foreach (List<Team> list in positions)
					{
						foreach (Team t5 in list)
						{
							Level.Add(new Pedestal(center + pedestalIndex * (smallMode ? 24 : 42), 150f, t5, placeIndex, smallMode));
							pedestalIndex++;
						}
						placeIndex++;
					}
					if (this._winningTeam.activeProfiles.Count > 1)
					{
						this._winningTeam.wins++;
					}
					else
					{
						this._winningTeam.activeProfiles[0].wins++;
					}
					foreach (Profile p4 in this._winningTeam.activeProfiles)
					{
						ProfileStats stats = p4.stats;
						int num = stats.trophiesWon;
						stats.trophiesWon = num + 1;
						p4.stats.trophiesSinceLastWin = p4.stats.trophiesSinceLastWinCounter;
						p4.stats.trophiesSinceLastWinCounter = 0;
						if ((!Network.isActive || p4.connection == DuckNetwork.localConnection) && !localWin)
						{
							localWin = true;
							if (Network.isActive)
							{
								StatBinding onlineWins = Global.data.onlineWins;
								num = onlineWins.valueInt;
								onlineWins.valueInt = num + 1;
							}
							if (p4.team.name == "SWACK")
							{
								StatBinding winsAsSwack = Global.data.winsAsSwack;
								num = winsAsSwack.valueInt;
								winsAsSwack.valueInt = num + 1;
							}
							if (p4.team.isHair)
							{
								StatBinding winsAsHair = Global.data.winsAsHair;
								num = winsAsHair.valueInt;
								winsAsHair.valueInt = num + 1;
							}
						}
						if (!Network.isActive && p4.team.name == "SWACK")
						{
							StatBinding winsAsSwack2 = Global.data.winsAsSwack;
							num = winsAsSwack2.valueInt;
							winsAsSwack2.valueInt = num + 1;
						}
					}
					foreach (Team team2 in teams)
					{
						foreach (Profile profile in team2.activeProfiles)
						{
							ProfileStats stats2 = profile.stats;
							int num = stats2.trophiesSinceLastWinCounter;
							stats2.trophiesSinceLastWinCounter = num + 1;
							ProfileStats stats3 = profile.stats;
							num = stats3.gamesPlayed;
							stats3.gamesPlayed = num + 1;
						}
					}
					Main.lastLevel = "";
				}
			}
			this._bottomRight = new Vec2(1000f, 1000f);
			this.lowestPoint = 1000f;
			this._scoreBoard = new GinormoBoard(300f, -320f, (this._mode == ScoreBoardMode.ShowScores) ? BoardMode.Points : BoardMode.Wins, teams.Count > 4);
			this._scoreBoard.z = -130f;
			Level.Add(this._scoreBoard);
			base.backgroundColor = new Color(0, 0, 0);
			Music.volume = 1f;
			if (this._mode != ScoreBoardMode.ShowWinner && !this._afterHighlights)
			{
				Music.Play("SportsTime", true, 0f);
			}
			this.cameraY = 0f;
			Sprite field;
			if (RockWeather.weather == Weather.Snowing)
			{
				field = new Sprite("fieldNoiseSnow", 0f, 0f);
			}
			else if (RockWeather.weather == Weather.Raining)
			{
				field = new Sprite("fieldNoiseRain", 0f, 0f);
			}
			else
			{
				field = new Sprite("fieldNoise", 0f, 0f);
			}
			field.scale = new Vec2(4f, 4f);
			field.depth = 0.5f;
			field.y -= 16f;
			this._field.AddSprite(field);
			Sprite fieldWall = new Sprite("fieldWall", 0f, 0f);
			fieldWall.scale = new Vec2(4f, 4f);
			fieldWall.depth = 0.5f;
			fieldWall.y -= 16f;
			this._wall = new WallLayer("FIELDWALL", 80);
			if (RockScoreboard.wallMode)
			{
				this._wall.AddWallSprite(fieldWall);
			}
			Layer.Add(this._wall);
			this._fieldForeground = new FieldBackground("FIELDFOREGROUND", 80);
			this._fieldForeground.fieldHeight = -13f;
			Layer.Add(this._fieldForeground);
			this._fieldForeground2 = new FieldBackground("FIELDFOREGROUND2", 70);
			this._fieldForeground2.fieldHeight = -15f;
			Layer.Add(this._fieldForeground2);
			if (this._mode != ScoreBoardMode.ShowWinner)
			{
				Sprite teevee = new Sprite("rockThrow/chairSeat", 0f, 0f);
				teevee.CenterOrigin();
				teevee.x = 300f;
				teevee.y = 20f;
				teevee.scale = new Vec2(1.2f, 1.2f);
				this._fieldForeground.AddSprite(teevee);
				teevee = new Sprite("rockThrow/tableTop", 0f, 0f);
				teevee.CenterOrigin();
				teevee.x = 450f;
				teevee.y = 14f;
				teevee.scale = new Vec2(1.2f, 1.4f);
				this._fieldForeground2.AddSprite(teevee);
				int ychange = -95;
				Sprite c = new Sprite("rockThrow/chairBottomBack", 0f, 0f);
				Level.Add(new SpriteThing(300f, -10f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 106 + ychange,
					depth = 0.5f,
					layer = Layer.Background
				});
				c = new Sprite("rockThrow/chairBottom", 0f, 0f);
				Level.Add(new SpriteThing(300f, -6f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange,
					depth = 0.8f,
					layer = Layer.Background
				});
				c = new Sprite("rockThrow/chairFront", 0f, 0f);
				Level.Add(new SpriteThing(300f, -9f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 122 + ychange,
					depth = 0.9f,
					layer = Layer.Background
				});
				c = new Sprite("rockThrow/tableBottomBack", 0f, 0f);
				Level.Add(new SpriteThing(450f, -7f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 106 + ychange,
					depth = 0.5f,
					layer = Layer.Background
				});
				c = new Sprite("rockThrow/tableBottom", 0f, 0f);
				Level.Add(new SpriteThing(450f, -7f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange,
					depth = 0.8f,
					layer = Layer.Background
				});
				c = new Sprite("rockThrow/keg", 0f, 0f);
				Level.Add(new SpriteThing(460f, -24f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange - 4,
					depth = -0.4f,
					layer = Layer.Game
				});
				c = new Sprite("rockThrow/cup", 0f, 0f);
				Level.Add(new SpriteThing(445f, -21f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange - 6,
					depth = -0.5f,
					layer = Layer.Game
				});
				c = new Sprite("rockThrow/cup", 0f, 0f);
				Level.Add(new SpriteThing(437f, -20f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange,
					depth = -0.3f,
					layer = Layer.Game
				});
				c = new Sprite("rockThrow/cup", 0f, 0f);
				Level.Add(new SpriteThing(472f, -20f, c)
				{
					center = new Vec2(c.w / 2, c.h / 2),
					z = 120 + ychange - 7,
					depth = -0.5f,
					layer = Layer.Game,
					angleDegrees = 80f
				});
			}
			for (int l = 0; l < 3; l++)
			{
				Level.Add(new DistanceMarker(230 + l * 175, -25f, (int)Math.Round((double)(l * GameMode.winsPerSet / 2f)))
				{
					z = 0f,
					depth = 0.34f,
					layer = Layer.Background
				});
			}
			Sprite cs;
			if (RockWeather.weather == Weather.Snowing)
			{
				cs = new Sprite("bleacherBackSnow", 0f, 0f);
			}
			else
			{
				cs = new Sprite("bleacherBack", 0f, 0f);
			}
			for (int m = 0; m < 24; m++)
			{
				SpriteThing spriteThing = new SpriteThing(100 + m * (cs.w + 13), cs.h + 15, cs);
				spriteThing.center = new Vec2(cs.w / 2, cs.h - 1);
				spriteThing.collisionOffset = new Vec2(spriteThing.collisionOffset.x, (float)(-(float)cs.h));
				spriteThing.z = 0f;
				spriteThing.depth = 0.33f;
				spriteThing.layer = Layer.Background;
				Level.Add(spriteThing);
			}
			Level.Add(new SpriteThing(600f, 0f, new Sprite("blackSquare", 0f, 0f))
			{
				z = -90f,
				centery = 7f,
				depth = 0.1f,
				layer = Layer.Background,
				xscale = 100f,
				yscale = 7f
			});
			this._weather.Update();
		}

		public Vec2 sunPos
		{
			get
			{
				return this.sunThing.position;
			}
		}

		public Layer sunLayer
		{
			get
			{
				return this._sunLayer;
			}
		}

		public static bool drawingSunTarget
		{
			get
			{
				return RockScoreboard._drawingSunTarget;
			}
		}

		public static bool drawingLighting
		{
			get
			{
				return RockScoreboard._drawingLighting;
			}
		}

		public static bool drawingNormalTarget
		{
			get
			{
				return RockScoreboard._drawingNormalTarget;
			}
		}

		public void DoRender()
		{
			Color backColor = base.backgroundColor;
			if (NetworkDebugger.enabled)
			{
				RockScoreboard._drawingSunTarget = true;
				Layer.Game.camera.width = 320f;
				Layer.Game.camera.height = 180f;
				this._field.fade = Layer.Game.fade;
				this._fieldForeground.fade = Layer.Game.fade;
				this._wall.fade = Layer.Game.fade;
				this._fieldForeground2.fade = Layer.Game.fade;
				base.backgroundColor = backColor;
				MonoMain.RenderGame(this._screenTarget);
				RockScoreboard._drawingSunTarget = false;
				return;
			}
			base.backgroundColor = Color.Black;
			RockScoreboard._drawingSunTarget = true;
			float hudFade = Layer.HUD.fade;
			float consoleFade = Layer.Console.fade;
			float gameFade = Layer.Game.fade;
			float backFade = Layer.Background.fade;
			float fieldFade = this._field.fade;
			Layer.Game.fade = 0f;
			Layer.Background.fade = 0f;
			Layer.Foreground.fade = 0f;
			this._field.fade = 0f;
			this._fieldForeground.fade = 0f;
			this._wall.fade = 0f;
			this._fieldForeground2.fade = 0f;
			Vec3 gameColorMul = Layer.Game.colorMul;
			Vec3 backColorMul = Layer.Background.colorMul;
			Layer.Game.colorMul = Vec3.One;
			Layer.Background.colorMul = Vec3.One;
			Layer.HUD.fade = 0f;
			Layer.Console.fade = 0f;
			this.fieldMulColor = Vec3.One;
			Vec3 colorAdd = Layer.Game.colorAdd;
			Layer.Game.colorAdd = Vec3.Zero;
			Layer.Background.colorAdd = Vec3.Zero;
			this.fieldAddColor = Vec3.Zero;
			Layer.blurry = true;
			this.sunThing.alpha = RockWeather.sunOpacity;
			((SpriteThing)this.rainbowThing2).alpha = 0f;
			RockScoreboard._drawingLighting = true;
			MonoMain.RenderGame(this._sunshineTarget);
			RockScoreboard._drawingLighting = false;
			if (this._sunshineMaterialBare == null)
			{
				this._sunshineMaterialBare = new MaterialSunshineBare();
			}
			Vec2 pos = this.sunPos;
			Vec3 newPos = new Vec3(pos.x, -9999f, pos.y);
			Viewport v = new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height);
			newPos = v.Project(newPos, this.sunLayer.projection, this.sunLayer.view, Matrix.Identity);
			newPos.y -= 256f;
			newPos.x /= v.Width;
			newPos.y /= v.Height;
			this._sunshineMaterialBare.effect.effect.Parameters["lightPos"].SetValue(new Vec2(newPos.x, newPos.y));
			this._sunshineMaterialBare.effect.effect.Parameters["weight"].SetValue(1f);
			this._sunshineMaterialBare.effect.effect.Parameters["density"].SetValue(0.4f);
			this._sunshineMaterialBare.effect.effect.Parameters["decay"].SetValue(0.68f + RockWeather.sunGlow);
			this._sunshineMaterialBare.effect.effect.Parameters["exposure"].SetValue(1f);
			Viewport viewport = Graphics.viewport;
			Graphics.SetRenderTarget(this._pixelTarget);
			Graphics.viewport = new Viewport(0, 0, this._pixelTarget.width, this._pixelTarget.height);
			Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, base.camera.getMatrix());
			Graphics.material = this._sunshineMaterialBare;
			float scale = this._pixelTarget.width * 2 / (float)this._sunshineTarget.width;
			Graphics.Draw(this._sunshineTarget, Vec2.Zero, null, Color.White, 0f, Vec2.Zero, new Vec2(scale), SpriteEffects.None, default(Depth));
			Graphics.material = null;
			Graphics.screen.End();
			Graphics.SetRenderTarget(null);
			Graphics.viewport = viewport;
			Layer.blurry = false;
			Layer.HUD.fade = hudFade;
			Layer.Console.fade = consoleFade;
			Layer.Game.fade = gameFade;
			Layer.Foreground.fade = gameFade;
			Layer.Background.fade = backFade;
			this._field.fade = fieldFade;
			this._fieldForeground.fade = fieldFade;
			this._fieldForeground2.fade = fieldFade;
			this._wall.fade = fieldFade;
			Layer.Game.colorMul = gameColorMul;
			Layer.Background.colorMul = backColorMul;
			this.fieldMulColor = backColorMul;
			Layer.Game.colorAdd = colorAdd;
			Layer.Background.colorAdd = colorAdd;
			this.fieldAddColor = colorAdd;
			RockScoreboard._drawingSunTarget = false;
			this.sunThing.x = 290f + RockWeather.sunPos.x * 8000f;
			this.sunThing.y = 10000f - RockWeather.sunPos.y * 8000f;
			this.rainbowThing.y = (this.rainbowThing2.y = 2000f + this._fieldScroll * 12f);
			this.rainbowThing.x = (this.rainbowThing2.x = -this._field.scroll * 15f + 6800f);
			this.rainbowThing.alpha = this._weather.rainbowLight;
			((SpriteThing)this.rainbowThing2).alpha = this._weather.rainbowLight2;
			this.rainbowThing.visible = (this.rainbowThing.alpha > 0.01f);
			this.rainbowThing2.visible = (this.rainbowThing2.alpha > 0.01f);
			RockScoreboard._drawingSunTarget = true;
			Layer.Game.camera.width = 320f;
			Layer.Game.camera.height = 180f;
			this._field.fade = Layer.Game.fade;
			this._fieldForeground.fade = Layer.Game.fade;
			this._fieldForeground2.fade = Layer.Game.fade;
			this._wall.fade = Layer.Game.fade;
			base.backgroundColor = backColor;
			RockScoreboard._drawingNormalTarget = true;
			MonoMain.RenderGame(this._screenTarget);
			RockScoreboard._drawingNormalTarget = false;
			RockScoreboard._drawingSunTarget = false;
		}

		public override void Update()
		{
			if (Network.isActive)
			{
				if (this._netCountdown == null)
				{
					if (Network.isServer)
					{
						if (DuckNetwork.isDedicatedServer)
						{
							this._netCountdown = new ContinueCountdown((this._mode == ScoreBoardMode.ShowScores) ? 4f : (this._afterHighlights ? 5f : 10f));
						}
						else
						{
							this._netCountdown = new ContinueCountdown((this._mode == ScoreBoardMode.ShowScores) ? 5f : 15f);
						}
						Level.Add(this._netCountdown);
					}
					else
					{
						IEnumerable<Thing> cd = Level.current.things[typeof(ContinueCountdown)];
						if (cd.Count<Thing>() > 0)
						{
							this._netCountdown = (cd.ElementAt(0) as ContinueCountdown);
						}
					}
				}
				else if (this._continueHUD != null)
				{
					if (Network.isServer)
					{
						this._continueHUD.text = "@START@CONTINUE(" + ((int)Math.Ceiling(_netCountdown.timer)).ToString() + ")";
						this._netCountdown.UpdateTimer();
					}
					else
					{
						this._continueHUD.text = "WAITING(" + ((int)Math.Ceiling(_netCountdown.timer)).ToString() + ")";
					}
				}
				if (Network.isServer && this.netCountdown != null && !this.netCountdown.isServerForObject)
				{
					int oldValue = this.controlMessage;
					if (oldValue > 0)
					{
						this.controlMessage = -1;
						this.controlMessage = oldValue;
					}
					Thing.Fondle(this.netCountdown, DuckNetwork.localConnection);
				}
			}
			bool isServer = Network.isServer;
			Network.isServer = true;
			base.backgroundColor = new Color(139, 204, 248) * this._backgroundFade;
			Layer.Game.fade = this._backgroundFade;
			Layer.Background.fade = this._backgroundFade;
			this._backgroundFade = Lerp.Float(this._backgroundFade, 1f, 0.02f);
			this._field.rise = this._fieldScroll;
			this._fieldForeground.rise = this._fieldScroll;
			this._fieldForeground2.rise = this._fieldScroll;
			this._wall.rise = this._fieldScroll;
			this._bottomRight = new Vec2(1000f, 1000f);
			this.lowestPoint = 1000f;
			bool scrollDone = false;
			this._field.scroll = Lerp.Float(this._field.scroll, this._desiredScroll, 6f);
			if (this._field.scroll < 297f)
			{
				this._field.scroll = 0f;
				scrollDone = true;
			}
			if (this._field.scroll < 302f)
			{
				this._field.scroll = 302f;
			}
			this._fieldForeground.scroll = this._field.scroll;
			this._fieldForeground2.scroll = this._field.scroll;
			this._wall.scroll = this._field.scroll;
			if (this._state != ScoreBoardState.Transition)
			{
				if (this._state == ScoreBoardState.Intro)
				{
					if (this._animWait > 0f)
					{
						this._animWait -= 0.021f;
					}
					else
					{
						Crowd.mood = Mood.Silent;
						this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 2.1f, 0.1f, 1.05f);
						if (this._intermissionSlide > 2.09f)
						{
							this.controlMessage = 0;
							Vote.OpenVoting("", "", false);
							this._state = ScoreBoardState.ThrowRocks;
						}
					}
				}
				else if (this._state == ScoreBoardState.MatchOver)
				{
					if (this._highestSlot.duck.position.x < this._highestSlot.rock.x - 16f)
					{
						this._highestSlot.ai.Release("LEFT");
						this._highestSlot.ai.Press("RIGHT");
					}
					if (this._highestSlot.duck.position.x > this._highestSlot.rock.x + 16f)
					{
						this._highestSlot.ai.Release("RIGHT");
						this._highestSlot.ai.Press("LEFT");
					}
					if (this._highestSlot.duck.position.x > this._highestSlot.rock.position.x - 16f || this._highestSlot.duck.profile.team == null)
					{
						this._focusRock = true;
					}
					for (int i = 0; i < this._highestSlot.subAIs.Count; i++)
					{
						DuckAI ai = this._highestSlot.subAIs[i];
						Duck duck = this._highestSlot.subDucks[i];
						if (duck.position.x < this._highestSlot.rock.x - 16f)
						{
							ai.Release("LEFT");
							ai.Press("RIGHT");
						}
						if (duck.position.x > this._highestSlot.rock.x + 16f)
						{
							ai.Release("RIGHT");
							ai.Press("LEFT");
						}
					}
					if (this._focusRock)
					{
						this._highestSlot.ai.Release("JUMP");
						if (Rando.Float(1f) > 0.98f)
						{
							this._highestSlot.ai.Press("JUMP");
						}
						for (int j = 0; j < this._highestSlot.subAIs.Count; j++)
						{
							DuckAI ai2 = this._highestSlot.subAIs[j];
							Duck duck2 = this._highestSlot.subDucks[j];
							ai2.Release("JUMP");
							if (Rando.Float(1f) > 0.98f)
							{
								ai2.Press("JUMP");
							}
						}
						if (!this._droppedConfetti)
						{
							this._desiredScroll = this._highestSlot.duck.position.x;
							if (this._desiredScroll >= this._highestSlot.rock.position.x)
							{
								this._desiredScroll = this._highestSlot.rock.position.x;
								Crowd.mood = Mood.Extatic;
								this._droppedConfetti = true;
								for (int k = 0; k < 64; k++)
								{
									Level.Add(new Confetti(this._confettiDrop + Rando.Float(-32f, 32f), this._highestSlot.rock.y - 220f - Rando.Float(50f)));
								}
							}
						}
						if (Network.isServer && (Input.Pressed("START", "Any") || (this._netCountdown != null && this._netCountdown.timer <= 0f)))
						{
							this._finished = true;
						}
						this._winnerWait -= 0.007f;
						if (this._winnerWait < 0f)
						{
							this._finished = true;
						}
					}
					else
					{
						this._desiredScroll = this._highestSlot.duck.position.x;
						Crowd.mood = Mood.Excited;
					}
				}
			}
			if (this._state == ScoreBoardState.ThrowRocks)
			{
				if (!this._shiftCamera)
				{
					this._controlSlide = Lerp.FloatSmooth(this._controlSlide, 1f, 0.1f, 1.05f);
				}
				bool allowStateUpdate = true;
				using (List<Slot3D>.Enumerator enumerator = this._slots.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Slot3D slot = enumerator.Current;
						slot.follow = false;
						if (allowStateUpdate)
						{
							if (slot.state != RockThrow.Finished)
							{
								allowStateUpdate = false;
								slot.follow = true;
							}
							else if (slot == this._slots[this._slots.Count - 1])
							{
								if (this._matchOver)
								{
									this._skipFade = true;
								}
								else
								{
									this._state = ScoreBoardState.ShowBoard;
								}
							}
							if (slot.state == RockThrow.Idle)
							{
								slot.state = RockThrow.PickUpRock;
							}
							if (slot.state == RockThrow.PickUpRock)
							{
								if (slot.duck.position.x < slot.rock.position.x)
								{
									slot.ai.Press("RIGHT");
								}
								else
								{
									slot.state = RockThrow.ThrowRock;
									slot.duck.position.x = slot.rock.position.x;
									slot.duck.hSpeed = 0f;
									if (TeamSelect2.eightPlayersActive)
									{
										this._throwWait = 0.5f;
									}
									else
									{
										this._throwWait = 0.9f;
									}
								}
							}
							if (slot.state == RockThrow.ThrowRock)
							{
								if (this._throwWait > 0f)
								{
									this._throwWait -= 0.08f;
									slot.ai.Release("RIGHT");
									slot.duck.GiveHoldable(slot.rock);
									if (TeamSelect2.eightPlayersActive)
									{
										this._afterThrowWait = 0.5f;
									}
									else
									{
										this._afterThrowWait = 0.7f;
									}
								}
								else
								{
									if (slot.duck.holdObject != null)
									{
										if (slot.duck.profile.team == null)
										{
											slot.duck.Kill(new DTDisconnect(slot.duck));
										}
										else
										{
											this._misfire = false;
											slot.duck.ThrowItem(true);
											float dif = slot.duck.profile.team.rockScore;
											int total = GameMode.winsPerSet * 2;
											if (dif > total - 2)
											{
												dif = total - 2 + Math.Min((slot.duck.profile.team.rockScore - GameMode.winsPerSet * 2) / 16f, 1f);
											}
											float distance = slot.startX + 30f + dif / total * this._fieldWidth - slot.rock.x;
											slot.rock.vSpeed = -2f - Maths.Clamp(distance / 300f, 0f, 1f) * 4f;
											float airTime = Math.Abs(2f * slot.rock.vSpeed) / slot.rock.currentGravity;
											float currentFriction = slot.rock.currentFriction;
											float reqSpeed = distance / airTime;
											slot.rock.frictionMult = 0f;
											slot.rock.grounded = false;
											slot.rock.hMax = 100f;
											slot.rock.vMax = 100f;
											if (slot.duck.profile.team.rockScore == slot.duck.profile.team.prevScoreboardScore)
											{
												reqSpeed = 0.3f;
												slot.rock.vSpeed = -0.6f;
												this._misfire = true;
											}
											slot.rock.hSpeed = reqSpeed * 0.88f;
											if (RockScoreboard.wallMode && slot.duck.profile.team.rockScore > GameMode.winsPerSet)
											{
												slot.rock.hSpeed += 1f;
											}
										}
									}
									if (slot.rock.grounded)
									{
										if (slot.duck.profile.team == null)
										{
											slot.duck.Kill(new DTDisconnect(slot.duck));
										}
										float sub = 0.015f;
										if (slot.duck.profile.team != null)
										{
											int change = slot.duck.profile.team.rockScore - slot.duck.profile.team.prevScoreboardScore;
											if (change == 0)
											{
												Crowd.mood = Mood.Dead;
											}
											else if (change < 2)
											{
												Crowd.mood = Mood.Calm;
											}
											else if (change < 5)
											{
												Crowd.mood = Mood.Excited;
												sub = 0.013f;
											}
											else if (change < 99)
											{
												Crowd.mood = Mood.Extatic;
												sub = 0.01f;
											}
										}
										int winsPerSet = GameMode.winsPerSet;
										if (slot.rock.frictionMult == 0f)
										{
											Sprite s;
											if (RockWeather.weather == Weather.Snowing)
											{
												s = new Sprite("rockThrow/rockSmudgeSnow", 0f, 0f);
											}
											else if (RockWeather.weather == Weather.Raining)
											{
												s = new Sprite("rockThrow/rockSmudgeMud", 0f, 0f);
											}
											else
											{
												s = new Sprite("rockThrow/rockSmudge", 0f, 0f);
											}
											s.position = new Vec2(slot.rock.x - 12f, slot.rock.z - 10f);
											s.depth = 0.9f;
											s.xscale = 0.8f;
											s.yscale = 1.4f;
											s.alpha = 0.9f;
											this._field.AddSprite(s);
										}
										slot.slideWait++;
										if (slot.slideWait > 3 && slot.rock.hSpeed > 0f)
										{
											Sprite s2;
											if (RockWeather.weather == Weather.Snowing)
											{
												s2 = new Sprite("rockThrow/rockSmearSnow", 0f, 0f);
											}
											else if (RockWeather.weather == Weather.Raining)
											{
												s2 = new Sprite("rockThrow/rockSmearMud", 0f, 0f);
											}
											else
											{
												s2 = new Sprite("rockThrow/rockSmear", 0f, 0f);
											}
											s2.position = new Vec2(slot.rock.x - 5f, slot.rock.z - 10f);
											s2.depth = 0.9f;
											s2.xscale = 0.6f;
											s2.yscale = 1.4f;
											s2.alpha = 0.9f;
											slot.slideWait = 0;
											this._field.AddSprite(s2);
										}
										slot.rock.frictionMult = 4f;
										this._afterThrowWait -= sub;
										if (this._afterThrowWait < 0.4f)
										{
											slot.state = RockThrow.ShowScore;
											SFX.Play("scoreDing", 1f, 0f, 0f, false);
											if (slot.duck.profile.team != null && RockScoreboard.wallMode && slot.duck.profile.team.rockScore > GameMode.winsPerSet)
											{
												slot.duck.profile.team.rockScore = GameMode.winsPerSet;
											}
											if (TeamSelect2.eightPlayersActive)
											{
												this._showScoreWait = 0.5f;
											}
											else
											{
												this._showScoreWait = 0.6f;
											}
											Crowd.ThrowHats(slot.duck.profile);
											if (!slot.showScore)
											{
												slot.showScore = true;
												Level.Add(new PointBoard(slot.rock, slot.duck.profile.team)
												{
													depth = slot.rock.depth + 1,
													z = slot.rock.z
												});
											}
										}
									}
									else if (slot.duck.profile.team == null)
									{
										slot.duck.Kill(new DTDisconnect(slot.duck));
									}
									else
									{
										int dif2 = slot.duck.profile.team.rockScore;
										int total2 = GameMode.winsPerSet * 2;
										if (!this._misfire && slot.rock.x > slot.startX + 30f + dif2 / (float)total2 * this._fieldWidth)
										{
											slot.rock.x = slot.startX + 30f + dif2 / (float)total2 * this._fieldWidth;
										}
									}
								}
							}
							if (slot.state == RockThrow.ShowScore)
							{
								this._showScoreWait -= 0.016f;
								if (this._showScoreWait < 0f)
								{
									if (slot.duck.profile.team == null)
									{
										slot.state = RockThrow.Finished;
										if (TeamSelect2.eightPlayersActive)
										{
											this._backWait = 0.5f;
										}
										else
										{
											this._backWait = 0.9f;
										}
									}
									else
									{
										slot.state = RockThrow.RunBack;
									}
								}
							}
							if (slot.state == RockThrow.RunBack)
							{
								if (slot == this._slots[this._slots.Count - 1])
								{
									slot.follow = false;
								}
								if (slot.duck.position.x > slot.startX)
								{
									slot.ai.Press("LEFT");
								}
								else
								{
									slot.duck.position.x = slot.startX;
									slot.duck.hSpeed = 0f;
									slot.duck.offDir = 1;
									slot.ai.Release("LEFT");
									this._backWait -= 0.05f;
									Crowd.mood = Mood.Silent;
									if (this._backWait < 0f && (scrollDone || slot == this._slots[this._slots.Count - 1]))
									{
										slot.state = RockThrow.Finished;
										if (TeamSelect2.eightPlayersActive)
										{
											this._backWait = 0.5f;
										}
										else
										{
											this._backWait = 0.9f;
										}
									}
								}
							}
						}
						if (slot.follow)
						{
							if (slot.state == RockThrow.ThrowRock || slot.state == RockThrow.ShowScore)
							{
								this._desiredScroll = slot.rock.position.x;
							}
							else
							{
								this._desiredScroll = slot.duck.position.x;
							}
						}
						if (Input.Pressed("START", "Any"))
						{
							foreach (Profile d in Profiles.active)
							{
								if (d.inputProfile != null && d.inputProfile.Pressed("START", false) && (!Network.isActive || d.connection == DuckNetwork.localConnection))
								{
									Vote.RegisterVote(d, VoteType.Skip);
									if (Network.isActive)
									{
										Send.Message(new NMVoteToSkip(d));
									}
								}
							}
						}
						if (Vote.Passed(VoteType.Skip))
						{
							this._skipFade = true;
						}
					}
					goto IL_1274;
				}
			}
			Vote.CloseVoting();
		IL_1274:
			if (this._state == ScoreBoardState.MatchOver)
			{
				Network.isServer = isServer;
				this._controlSlide = Lerp.FloatSmooth(this._controlSlide, (this.controlMessage == 1) ? 1f : 0f, 0.1f, 1.05f);
				if (this._controlSlide < 0.01f)
				{
					this.controlMessage = -1;
				}
			}
			if (this._state == ScoreBoardState.ShowBoard)
			{
				Network.isServer = isServer;
				this._shiftCamera = true;
				this._controlSlide = Lerp.FloatSmooth(this._controlSlide, (this.controlMessage == 1) ? 1f : 0f, 0.1f, 1.05f);
				if (this._controlSlide < 0.01f)
				{
					this.controlMessage = 1;
				}
			}
			if (this._shiftCamera)
			{
				if (this._state == ScoreBoardState.ThrowRocks)
				{
					this._controlSlide = Lerp.FloatSmooth(this._controlSlide, 0f, 0.1f, 1.05f);
				}
				this._desiredScroll = -79f;
				if (this._fieldScroll < 220f)
				{
					this._fieldScroll += 4f;
				}
				else
				{
					if (this._state == ScoreBoardState.ThrowRocks)
					{
						this._state = ScoreBoardState.ShowBoard;
					}
					if (!this._scoreBoard.activated)
					{
						this._scoreBoard.Activate();
					}
					if (!this._finished && isServer && (Input.Pressed("START", "Any") || (this._netCountdown != null && this._netCountdown.timer <= 0f)))
					{
						this._finished = true;
					}
					Crowd.mood = Mood.Dead;
				}
			}
			if (this._skipFade)
			{
				Network.isServer = isServer;
				this.controlMessage = -1;
				Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.02f);
				if (Graphics.fade < 0.01f)
				{
					this._skipFade = false;
					if (this._mode == ScoreBoardMode.ShowScores)
					{
						if (!this._matchOver)
						{
							this._state = ScoreBoardState.ShowBoard;
							this._fieldScroll = 220f;
							this._desiredScroll = -79f;
							this._field.scroll = this._desiredScroll;
							goto IL_161F;
						}
						this._state = ScoreBoardState.MatchOver;
						this._field.scroll = 0f;
						using (List<Slot3D>.Enumerator enumerator = this._slots.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Slot3D slot2 = enumerator.Current;
								if (slot2.duck.profile.team != null)
								{
									float dif3 = slot2.duck.profile.team.rockScore;
									int total3 = GameMode.winsPerSet * 2;
									if (dif3 > total3 - 2)
									{
										dif3 = total3 - 2 + Math.Min((slot2.duck.profile.team.rockScore - GameMode.winsPerSet * 2) / 16f, 1f);
									}
									slot2.rock.x = slot2.startX + 30f + dif3 / total3 * this._fieldWidth;
									if (RockScoreboard.wallMode && slot2.duck.profile.team.rockScore >= GameMode.winsPerSet)
									{
										slot2.rock.x -= 10f;
									}
								}
								slot2.rock.hSpeed = 0f;
							}
							goto IL_161F;
						}
					}
					if (this._afterHighlights)
					{
						this._fieldScroll = 220f;
						this._desiredScroll = -79f;
						this._field.scroll = this._desiredScroll;
						this._scoreBoard.Activate();
						this._viewBoard = true;
					}
					else if (isServer && Network.isActive)
					{
						Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
					}
					else
					{
						Level.current = new HighlightLevel(false, false);
					}
				}
			}
		IL_161F:
			if (this._finished)
			{
				Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.03f);
				if (Graphics.fade < 0.01f)
				{
					foreach (Team team in Teams.all)
					{
						team.prevScoreboardScore = team.score;
					}
					if (isServer)
					{
						if (this._mode == ScoreBoardMode.ShowWinner)
						{
							if (this._returnToScoreboard)
							{
								Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, true);
							}
							else
							{
								Main.ResetMatchStuff();
								if (this._hatSelect)
								{
									Level.current = new TeamSelect2(true);
								}
								else if (!this._quit)
								{
									Music.Stop();
									Level.current = RockScoreboard.returnLevel;
									Graphics.fade = 1f;
								}
								else
								{
									if (Network.isActive)
									{
										Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Game Over!"));
									}
									Level.current = new TitleScreen();
								}
							}
						}
						else if (this._state != ScoreBoardState.MatchOver)
						{
							Music.Stop();
							Level.current = RockScoreboard.returnLevel;
							Graphics.fade = 1f;
						}
						else
						{
							Level.current = new RockScoreboard(RockScoreboard.returnLevel, ScoreBoardMode.ShowWinner, false);
						}
					}
				}
			}
			else if (!this._skipFade && !this._finished)
			{
				Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.03f);
			}
			Network.isServer = isServer;
			if (this._mode == ScoreBoardMode.ShowWinner)
			{
				this._controlSlide = Lerp.FloatSmooth(this._controlSlide, (this.controlMessage == 1) ? 1f : 0f, 0.1f, 1.05f);
				if (this._controlSlide < 0.01f)
				{
					this.controlMessage = 1;
				}
				if (this._viewBoard)
				{
					this.controlMessage = 2;
					this._controlSlide = 1f;
				}
				if (!this._scoreBoard.activated)
				{
					if (isServer && (Input.Pressed("START", "Any") || (this._netCountdown != null && this._netCountdown.timer <= 0f)))
					{
						if (Network.isActive)
						{
							this._finished = true;
							this._returnToScoreboard = true;
						}
						else
						{
							this._takePicture = true;
							HUD.CloseAllCorners();
						}
					}
					if (this._takePicture && this._flashSkipFrames == 0)
					{
						this._cameraWait -= 0.01f;
						if (this._cameraWait < 0.6f && this._playedBeeps == 0)
						{
							this._playedBeeps = 1;
							SFX.Play("cameraBeep", 1f, -0.5f, 0f, false);
						}
						else if (this._cameraWait < 0.3f && this._playedBeeps == 1)
						{
							this._playedBeeps = 2;
							SFX.Play("cameraBeep", 1f, -0.5f, 0f, false);
						}
						if (this._cameraWait < 0f && !this._playedFlash)
						{
							this._playedFlash = true;
							SFX.Play("cameraFlash", 0.8f, 1f, 0f, false);
						}
						if (this._cameraWait < 0.1f)
						{
							this._cameraFadeVel += 0.003f;
							if (this._cameraWait < 0.04f)
							{
								this._cameraFadeVel += 0.01f;
							}
						}
						Graphics.fadeAdd += this._cameraFadeVel;
						if (Graphics.fadeAdd > 1f)
						{
							int wide = Graphics.width;
							int high = Graphics.height;
							if (!RockScoreboard._sunEnabled)
							{
								int yCut = Graphics.height / 4 * 3 + 30;
								Graphics.fadeAdd = 0f;
								Layer.Background.fade = 0.8f;
								this._flashSkipFrames++;
								RockScoreboard.finalImage = new RenderTarget2D(wide, high, false);
								Layer.Game.visible = false;
								Rectangle sciss = this._field.scissor;
								this._field.scissor = new Rectangle(0f, 0f, Resolution.size.x, yCut);
								this._field.visible = true;
								MonoMain.RenderGame(RockScoreboard.finalImage);
								Layer.Game.visible = true;
								Color c = Level.current.backgroundColor;
								Level.current.backgroundColor = Color.Transparent;
								RockScoreboard.finalImage2 = new RenderTarget2D(wide, high, false);
								Layer.allVisible = false;
								Layer.Game.visible = true;
								yCut -= 5;
								this._field.scissor = new Rectangle(0f, yCut, wide, high - yCut);
								this._field.visible = true;
								MonoMain.RenderGame(RockScoreboard.finalImage2);
								this._field.scissor = sciss;
								Layer.allVisible = true;
								Level.current.backgroundColor = c;
								this._getScreenshot = true;
								this._finalSprite = new Sprite(RockScoreboard.finalImage, 0f, 0f);
								Stream stream = DuckFile.Create(DuckFile.albumDirectory + "album" + DateTime.Now.ToString("MM-dd-yy H;mm;ss") + ".png");
								((Texture2D)this._finalSprite.texture.nativeObject).SaveAsPng(stream, wide, high);
								stream.Dispose();
							}
							else
							{
								Graphics.fadeAdd = 0f;
								Layer.Background.fade = 0.8f;
								this._weather.Update();
								this.DoRender();
								RockScoreboard.finalImage = new RenderTarget2D(wide, high, false);
								this.RenderFinalImage(RockScoreboard.finalImage, false);
								this._finalSprite = new Sprite(RockScoreboard.finalImage, 0f, 0f);
								this._getScreenshot = true;
								Graphics.fadeAdd = 1f;
								wide = 320;
								high = 180;
								RenderTarget2D image = new RenderTarget2D(wide, high, false);
								this.RenderFinalImage(image, true);
								Graphics.fadeAdd = 1f;
								Stream stream2 = DuckFile.Create(DuckFile.albumDirectory + DateTime.Now.ToString("MM-dd-yy H;mm") + ".png");
								((Texture2D)_finalSprite.texture.nativeObject).SaveAsPng(stream2, wide, high);
								stream2.Dispose();
								this.DoRender();
							}
						}
					}
					if (this._getScreenshot && Graphics.screenCapture == null)
					{
						Level.current.simulatePhysics = false;
						this._flashSkipFrames++;
						if (this._flashSkipFrames > 2)
						{
							Graphics.fadeAdd = 1f;
						}
						if (this._flashSkipFrames > 20)
						{
							Level.current = new HighlightLevel(false, false);
						}
					}
				}
				else if (!this._finished && isServer)
				{
					if (Input.Pressed("START", "Any") || (this._netCountdown != null && this._netCountdown.timer <= 0f))
					{
						this._finished = true;
						this._hatSelect = DuckNetwork.isDedicatedServer;
					}
					if (Input.Pressed("CANCEL", "Any"))
					{
						this._finished = true;
						this._quit = true;
					}
					if (Input.Pressed("MENU2", "Any"))
					{
						this._finished = true;
						this._hatSelect = true;
					}
				}
			}
			Network.isServer = isServer;
			base.Update();
		}

		public override void Terminate()
		{
			if (this._mode == ScoreBoardMode.ShowWinner)
			{
				using (List<Team>.Enumerator enumerator = Teams.all.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Team team = enumerator.Current;
						team.prevScoreboardScore = 0;
					}
					goto IL_74;
				}
			}
			foreach (Team team2 in Teams.all)
			{
				team2.prevScoreboardScore = team2.score;
			}
		IL_74:
			Vote.CloseVoting();
		}

		public void RenderFinalImage(RenderTarget2D image, bool shrink)
		{
			if (this._sunshineMaterial == null)
			{
				this._sunshineMaterial = new MaterialSunshine(this._screenTarget);
			}
			Graphics.SetRenderTarget(image);
			Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, base.camera.getMatrix());
			Graphics.material = this._sunshineMaterial;
			float scale = Graphics.width / (_pixelTarget.width * (Graphics.width / 320f));
			if (shrink)
			{
				scale = 2f;
			}
			Graphics.Draw(this._pixelTarget, Vec2.Zero, null, Color.White, 0f, Vec2.Zero, new Vec2(scale), SpriteEffects.None, default(Depth));
			Graphics.material = null;
			Graphics.screen.End();
			Graphics.SetRenderTarget(null);
		}

		public override void DoDraw()
		{
			if (NetworkDebugger.enabled)
			{
				base.DoDraw();
				return;
			}
			if (RockScoreboard._drawingSunTarget || !RockScoreboard._sunEnabled)
			{
				base.DoDraw();
				return;
			}
			if (RockScoreboard._sunEnabled)
			{
				this.DoRender();
			}
			Graphics.Clear(Color.Black);
			if (this._sunshineMaterial == null)
			{
				this._sunshineMaterial = new MaterialSunshine(this._screenTarget);
			}
			if (NetworkDebugger.enabled)
			{
				Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, base.camera.getMatrix());
				float scale = Graphics.width / (_screenTarget.width * (Graphics.width / 320f));
				Graphics.Draw(this._screenTarget, Vec2.Zero, null, Color.White, 0f, Vec2.Zero, new Vec2(scale), SpriteEffects.None, default(Depth));
				Graphics.material = null;
				Graphics.screen.End();
				return;
			}
			Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, base.camera.getMatrix());
			Graphics.material = this._sunshineMaterial;
			float scale2 = Graphics.width / (_pixelTarget.width * (Graphics.width / 320f));
			Graphics.Draw(this._pixelTarget, Vec2.Zero, null, Color.White, 0f, Vec2.Zero, new Vec2(scale2), SpriteEffects.None, default(Depth));
			Graphics.material = null;
			Graphics.screen.End();
		}

		public override void Draw()
		{
			Layer.Game.perspective = (this._mode == ScoreBoardMode.ShowScores);
			Layer.Game.projection = this._field.projection;
			Layer.Game.view = this._field.view;
			Layer.Background.perspective = true;
			Layer.Background.projection = this._field.projection;
			Layer.Background.view = this._field.view;
			Layer.Foreground.perspective = true;
			Layer.Foreground.projection = this._field.projection;
			Layer.Foreground.view = this._field.view;
			if (RockScoreboard._sunEnabled)
			{
				this._sunLayer.perspective = true;
				this._sunLayer.projection = this._field.projection;
				this._sunLayer.view = this._field.view;
			}
		}

		public override void PostDrawLayer(Layer layer)
		{
			if (layer == Layer.HUD)
			{
				if (this._getScreenshot && Graphics.screenCapture == null)
				{
					this._finalSprite.scale = new Vec2(0.25f, 0.25f);
					Graphics.Draw(this._finalSprite, 0f, 0f);
				}
				if (this._intermissionSlide > 0.01f)
				{
					this._intermissionText.depth = 0.91f;
					float xpos = -320f + this._intermissionSlide * 320f;
					float ypos = 60f;
					Graphics.DrawRect(new Vec2(xpos, ypos), new Vec2(xpos + 320f, ypos + 30f), Color.Black, 0.9f, true, 1f);
					xpos = 320f - this._intermissionSlide * 320f;
					ypos = 60f;
					Graphics.DrawRect(new Vec2(xpos, ypos + 30f), new Vec2(xpos + 320f, ypos + 60f), Color.Black, 0.9f, true, 1f);
					Graphics.Draw(this._intermissionText, -320f + this._intermissionSlide * 336f, ypos + 18f);
				}
			}
			else if (layer == Layer.Game)
			{
				if (this._mode == ScoreBoardMode.ShowWinner && !this._afterHighlights)
				{
					this._winnerPost.depth = -0.962f;
					this._winnerBanner.depth = -0.858f;
					float yOff = -10f;
					Graphics.Draw(this._winnerPost, 63f, 40f + yOff);
					Graphics.Draw(this._winnerPost, 248f, 40f + yOff);
					Graphics.Draw(this._winnerBanner, 70f, 43f + yOff);
					string text = Results.winner.name;
					BitmapFont font = Results.winner.font;
					font.scale = new Vec2(2f, 2f);
					float hOffset = 0f;
					float vOffset = 0f;
					if (text.Length > 12)
					{
						font.scale = new Vec2(1f);
						vOffset = 3f;
					}
					else if (text.Length > 9)
					{
						font.scale = new Vec2(1.5f);
						hOffset = 2f;
						vOffset = 1f;
					}
					font.Draw(text, 160f - font.GetWidth(text, false, null) / 2f + hOffset, 50f + yOff + vOffset, Color.Black, this._winnerBanner.depth + 1, null, false);
					font.scale = new Vec2(1f, 1f);
				}
			}
			else if (layer == Layer.Foreground)
			{
				bool drawingSunTarget = RockScoreboard._drawingSunTarget;
			}
			base.PostDrawLayer(layer);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static RockScoreboard()
		{
		}

		private static Level _returnLevel;

		private bool _afterHighlights;

		private ScoreBoardMode _mode;

		private ScoreBoardState _state = ScoreBoardState.Intro;

		private float _throwWait = 1f;

		private float _afterThrowWait = 1f;

		private Sprite _bleachers;

		private Sprite _bleacherSeats;

		private Sprite _intermissionText;

		private Sprite _winnerPost;

		private Sprite _winnerBanner;

		private BitmapFont _font;

		private FieldBackground _field;

		private WallLayer _wall;

		private FieldBackground _fieldForeground;

		private FieldBackground _fieldForeground2;

		private ContinueCountdown _netCountdown;

		private GinormoBoard _scoreBoard;

		private bool _shiftCamera;

		private bool _finished;

		private bool _getScreenshot;

		private Sprite _finalSprite;

		private float _intermissionSlide = 1f;

		private float _controlSlide;

		private int _controlMessage = -1;

		private CornerDisplay _continueHUD;

		private float _desiredScroll;

		private float _animWait = 1f;

		private float _backWait = 1f;

		private float _showScoreWait = 1f;

		private float _fieldWidth = 680f;

		private bool _skipFade;

		private float _winnerWait = 1f;

		private bool _matchOver;

		private bool _tie;

		private bool _viewBoard;

		private bool _quit;

		private bool _misfire;

		private float _cameraWait = 1f;

		private bool _takePicture;

		private int _playedBeeps;

		private bool _playedFlash;

		private float _cameraFadeVel;

		private int _flashSkipFrames;

		private float _fieldScroll;

		private SinWave _sin = 0.01f;

		private Crowd _crowd;

		public List<Slot3D> _slots = new List<Slot3D>();

		private Slot3D _highestSlot;

		private Team _winningTeam;

		public static RenderTarget2D finalImage;

		public static RenderTarget2D finalImage2;

		private float _backgroundFade;

		private Layer _sunLayer;

		private RenderTarget2D _sunshineTarget;

		private RenderTarget2D _screenTarget;

		private RenderTarget2D _pixelTarget;

		public static bool _sunEnabled = true;

		private RockWeather _weather;

		private List<InputObject> _inputs = new List<InputObject>();

		public static bool initializingDucks = false;

		public static bool wallMode = false;

		private Thing sunThing;

		private Thing rainbowThing;

		private Thing rainbowThing2;

		private static bool _drawingSunTarget = false;

		private static bool _drawingLighting = false;

		private static bool _drawingNormalTarget = false;

		private bool _hatSelect;

		private bool _focusRock;

		private bool _droppedConfetti;

		private float _confettiDrop;

		private bool _returnToScoreboard;

		private Material _sunshineMaterial;

		private Material _sunshineMaterialBare;


	}
}
