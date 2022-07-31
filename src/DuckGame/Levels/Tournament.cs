//// Decompiled with JetBrains decompiler
//// Type: DuckGame.Tournament
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System;
//using System.Collections.Generic;

//namespace DuckGame
//{
//    public class Tournament : Level
//    {
//        private List<TourneyGroup> _groups = new List<TourneyGroup>();
//        private List<TourneyGroup> _loserGroups = new List<TourneyGroup>();
//        private Random r;

//        public override void Initialize()
//        {
//            Layer.HUD.camera.size *= 4f;
//            int num1 = 19;
//            int num2 = 3;
//            TourneyGroup tourneyGroup1 = new TourneyGroup();
//            int num3 = 0;
//            foreach (Team p in Teams.allRandomized)
//            {
//                tourneyGroup1.AddPlayer(p, true);
//                if (tourneyGroup1.players.Count == num2)
//                {
//                    this._groups.Add(tourneyGroup1);
//                    tourneyGroup1 = new TourneyGroup();
//                    tourneyGroup1.groupIndex = this._groups.Count;
//                }
//                ++num3;
//                if (num3 == num1)
//                    break;
//            }
//            if (tourneyGroup1.players.Count > 0)
//                this._groups.Add(tourneyGroup1);
//            List<TourneyGroup> tourneyGroupList1 = this._groups;
//            List<TourneyGroup> tourneyGroupList2 = new List<TourneyGroup>();
//            int num4 = 1;
//            while (tourneyGroupList1.Count > 1)
//            {
//                int num5 = 0;
//                TourneyGroup tourneyGroup2 = (TourneyGroup)null;
//                foreach (TourneyGroup tourneyGroup3 in tourneyGroupList1)
//                {
//                    if (tourneyGroup2 == null)
//                    {
//                        num5 = 0;
//                        tourneyGroup2 = new TourneyGroup();
//                        tourneyGroup2.groupIndex = tourneyGroupList2.Count;
//                        tourneyGroup2.depth = num4;
//                        tourneyGroupList2.Add(tourneyGroup2);
//                    }
//                    tourneyGroup3.next = tourneyGroup2;
//                    tourneyGroup3.next.AddPlayer(tourneyGroup3.players[Rando.Int(tourneyGroup3.players.Count - 1)]);
//                    ++num5;
//                    if (num5 == num2)
//                        tourneyGroup2 = (TourneyGroup)null;
//                }
//                ++num4;
//                tourneyGroupList1 = tourneyGroupList2;
//                tourneyGroupList2 = new List<TourneyGroup>();
//            }
//            base.Initialize();
//        }

//        public override void Update()
//        {
//            if (InputProfile.DefaultPlayer1.Pressed("CANCEL"))
//            {
//                this._groups.Clear();
//                Rando.generator = new Random(30502);
//                int num1 = 2 + (int)(InputProfile.DefaultPlayer1.leftTrigger * 30.0);
//                int num2 = 2 + (int)(InputProfile.DefaultPlayer1.rightTrigger * 2.0);
//                TourneyGroup tourneyGroup1 = new TourneyGroup();
//                int num3 = 0;
//                foreach (Team p in Teams.allRandomized)
//                {
//                    tourneyGroup1.AddPlayer(p, true);
//                    if (tourneyGroup1.players.Count == num2)
//                    {
//                        this._groups.Add(tourneyGroup1);
//                        tourneyGroup1 = new TourneyGroup();
//                        tourneyGroup1.groupIndex = this._groups.Count;
//                    }
//                    ++num3;
//                    if (num3 == num1)
//                        break;
//                }
//                if (tourneyGroup1.players.Count > 0)
//                    this._groups.Add(tourneyGroup1);
//                TourneyGroup tourneyGroup2 = (TourneyGroup)null;
//                List<TourneyGroup> tourneyGroupList1 = this._groups;
//                List<TourneyGroup> tourneyGroupList2 = new List<TourneyGroup>();
//                int num4 = 1;
//                while (tourneyGroupList1.Count > 1)
//                {
//                    int num5 = 0;
//                    TourneyGroup tourneyGroup3 = (TourneyGroup)null;
//                    foreach (TourneyGroup tourneyGroup4 in tourneyGroupList1)
//                    {
//                        if (tourneyGroup3 == null)
//                        {
//                            num5 = 0;
//                            tourneyGroup3 = new TourneyGroup();
//                            tourneyGroup3.groupIndex = tourneyGroupList2.Count;
//                            tourneyGroup3.depth = num4;
//                            tourneyGroupList2.Add(tourneyGroup3);
//                        }
//                        tourneyGroup4.next = tourneyGroup3;
//                        int index = Rando.Int(tourneyGroup4.players.Count - 1);
//                        tourneyGroup4.next.AddPlayer(tourneyGroup4.players[index], true);
//                        ++num5;
//                        if (num5 == num2)
//                            tourneyGroup3 = (TourneyGroup)null;
//                    }
//                    ++num4;
//                    tourneyGroupList1 = tourneyGroupList2;
//                    tourneyGroupList2 = new List<TourneyGroup>();
//                }
//                this._loserGroups.Clear();
//                tourneyGroup2 = (TourneyGroup)null;
//                List<TourneyGroup> tourneyGroupList3 = this._groups;
//                List<TourneyGroup> loserGroups = this._loserGroups;
//                List<TourneyGroup> tourneyGroupList4 = new List<TourneyGroup>();
//                List<Team> teamList = new List<Team>();
//                int num6 = 0;
//                while (tourneyGroupList3.Count > 1)
//                {
//                    TourneyGroup tourneyGroup5 = (TourneyGroup)null;
//                    foreach (TourneyGroup tourneyGroup6 in tourneyGroupList3)
//                    {
//                        if (tourneyGroup6.next != null)
//                        {
//                            if (!tourneyGroupList4.Contains(tourneyGroup6))
//                                tourneyGroupList4.Add(tourneyGroup6);
//                            foreach (Team player in tourneyGroup6.players)
//                            {
//                                if (tourneyGroup5 == null)
//                                {
//                                    tourneyGroup5 = new TourneyGroup();
//                                    tourneyGroup5.groupIndex = loserGroups.Count;
//                                    tourneyGroup5.depth = num6;
//                                    loserGroups.Add(tourneyGroup5);
//                                }
//                                if (!tourneyGroup6.next.players.Contains(player))
//                                {
//                                    tourneyGroup5.AddPlayer(player, true);
//                                    teamList.Add(player);
//                                }
//                                if (tourneyGroup5.players.Count == num2)
//                                    tourneyGroup5 = (TourneyGroup)null;
//                            }
//                        }
//                    }
//                    ++num6;
//                    tourneyGroupList3 = tourneyGroupList4;
//                    tourneyGroupList4 = new List<TourneyGroup>();
//                }
//            }
//            base.Update();
//        }

//        public void DrawGroup(List<TourneyGroup> gr, Vec2 drawPos)
//        {
//            Vec2 vec2 = new Vec2(0f, 0f);
//            List<TourneyGroup> tourneyGroupList1 = gr;
//            List<TourneyGroup> tourneyGroupList2 = new List<TourneyGroup>();
//            Vec2 zero = Vec2.Zero;
//            float num1 = 8f;
//            float num2 = 0f;
//            while (tourneyGroupList1.Count > 0)
//            {
//                int num3 = 0;
//                foreach (TourneyGroup tourneyGroup in tourneyGroupList1)
//                {
//                    if (tourneyGroup.next != null && !tourneyGroupList2.Contains(tourneyGroup.next))
//                        tourneyGroupList2.Add(tourneyGroup.next);
//                    Graphics.DrawLine(drawPos + vec2 + new Vec2(96f, 4f), drawPos + vec2 + new Vec2(96f, (float)((tourneyGroup.players.Count - 1) * (num1 + 8.0) + 4.0)), Color.White);
//                    foreach (Team player in tourneyGroup.players)
//                    {
//                        string text = tourneyGroup.assigned[tourneyGroup.players.IndexOf(player)] ? player.name : "???";
//                        if (tourneyGroup.depth > 0)
//                        {
//                            Graphics.DrawLine(drawPos + vec2 + new Vec2(0f, 4f), drawPos + vec2 + new Vec2((float)((9 - (text.Length - 1)) * 8), 4f), Color.White);
//                            Graphics.DrawLine(drawPos + vec2 + new Vec2(0f, 4f), player.prevTreeDraw, Color.White);
//                        }
//                        player.prevTreeDraw = drawPos + vec2 + new Vec2(96f, 4f);
//                        Graphics.DrawLine(drawPos + vec2 + new Vec2(90f, 4f), drawPos + vec2 + new Vec2(96f, 4f), Color.White);
//                        Graphics.DrawString(text, drawPos + vec2 + new Vec2((float)(88 - text.Length * 8), 0f), Color.White, (Depth)1f);
//                        vec2.y += num1 + 8f;
//                        ++num3;
//                    }
//                }
//                vec2.x += 96f;
//                int num4 = (num3 * 16 + tourneyGroupList1.Count * 8) / 2;
//                vec2.y = num2 + (float)((tourneyGroupList1[0].players.Count - 1) * (num1 + 8.0) / 2.0);
//                num2 = vec2.y;
//                num1 = num1 * (float)tourneyGroupList1[0].players.Count + (float)(8 * (tourneyGroupList1[0].players.Count - 1));
//                tourneyGroupList1 = tourneyGroupList2;
//                tourneyGroupList2 = new List<TourneyGroup>();
//                if (tourneyGroupList1.Count <= 0)
//                    break;
//            }
//        }

//        public override void PostDrawLayer(Layer layer)
//        {
//            if (layer == Layer.HUD)
//            {
//                this.DrawGroup(this._groups, new Vec2(10f, 10f));
//                this.DrawGroup(this._loserGroups, new Vec2(550f, 10f));
//            }
//            base.PostDrawLayer(layer);
//        }

//        public override void Draw() => base.Draw();
//    }
//}
