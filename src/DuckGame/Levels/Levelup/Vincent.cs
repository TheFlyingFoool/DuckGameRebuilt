// Decompiled with JetBrains decompiler
// Type: DuckGame.Vincent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Vincent
    {
        public static Vincent context = new Vincent();
        public static List<VincentProduct> products = new List<VincentProduct>();
        public static float alpha = 0.0f;
        public static bool lookingAtList = false;
        public static bool lookingAtChallenge = false;
        public static bool hover = false;
        private static FancyBitmapFont _font;
        private static BitmapFont _priceFont;
        private static BitmapFont _priceFontCrossout;
        private static BitmapFont _priceFontRightways;
        private static BitmapFont _descriptionFont;
        private static SpriteMap _dealer;
        private static Sprite _tail;
        private static Sprite _photo;
        private static SpriteMap _newSticker;
        private static SpriteMap _rareSticker;
        private static Sprite _soldSprite;
        private static List<string> _lines = new List<string>();
        private static DealerMood _mood;
        private static string _currentLine = "";
        private static List<TextLine> _lineProgress = new List<TextLine>();
        private static float _waitLetter = 1f;
        private static float _waitAfterLine = 1f;
        private static float _talkMove = 0.0f;
        private static float _showLerp = 0.0f;
        private static bool _allowMovement = false;
        private static bool _hasYoYo = true;
        public static DayType type;
        public static bool show = false;
        public static bool hasKid = false;
        public static bool openedCorners = false;
        private static float _afterShowWait = 0.0f;
        public static bool _willGiveYoYo = false;
        public static bool _willGiveVooDoo = false;
        public static bool _willGivePerimeterDefence = false;
        private static float _listLerp = 0.0f;
        private static float _challengeLerp = 0.0f;
        private static float _chancyLerp = 0.0f;
        public static Sprite _furniFrame;
        public static Sprite _furniFill;
        public static Sprite _furniHov;
        public static SpriteMap _furniTag;
        public static Sprite _cheapTape;
        public static Sprite _bigBanner;
        public static Sprite _fancyBanner;
        private static List<RenderTarget2D> _priceTargets = new List<RenderTarget2D>();
        private static int _soldSelectIndex = -1;
        private static bool killSkip = false;
        private static float _extraWait = 0.0f;
        public static int _giveTickets = 0;
        public static bool afterChallenge = false;
        public static float afterChallengeWait = 0.0f;
        private static List<ChallengeData> _chancyChallenges = new List<ChallengeData>();
        public static bool showingDay = false;
        private static string lastWord = "";
        private static int wait = 0;
        public static int _selectIndex = -1;
        public static int _selectDescIndex = -2;

        public static int frame
        {
            get
            {
                if (Vincent._mood == DealerMood.Concerned)
                    return Vincent._dealer.frame - 6;
                return Vincent._mood == DealerMood.Point ? Vincent._dealer.frame - 3 : Vincent._dealer.frame;
            }
            set
            {
                if (Vincent._mood == DealerMood.Concerned)
                    Vincent._dealer.frame = value + 6;
                else if (Vincent._mood == DealerMood.Point)
                    Vincent._dealer.frame = value + 3;
                else
                    Vincent._dealer.frame = value;
            }
        }

        public static void Clear()
        {
            Vincent._lines.Clear();
            Vincent._lineProgress.Clear();
            Vincent._waitLetter = 0.0f;
            Vincent._waitAfterLine = 0.0f;
            Vincent._currentLine = "";
            Vincent._mood = DealerMood.Normal;
        }

        public static void Add(string line) => Vincent._lines.Add(line);

        public static int SortSaleDayFurniture(Furniture t1, Furniture t2)
        {
            float num1 = Rando.Float(1f) - Profiles.experienceProfile.GetNumFurnitures(t1.index) * 1f;
            float num2 = Rando.Float(1f) - Profiles.experienceProfile.GetNumFurnitures(t2.index) * 1f;
            if ((double)num1 > (double)num2)
                return 1;
            return (double)num1 < (double)num2 ? -1 : 0;
        }

        public static void Open(DayType t)
        {
            Vincent._lineProgress.Clear();
            Vincent.show = false;
            Vincent._afterShowWait = 0.0f;
            Vincent._showLerp = 0.0f;
            Vincent._allowMovement = false;
            Vincent._waitAfterLine = 1f;
            Vincent._waitLetter = 1f;
            Vincent._mood = DealerMood.Normal;
            Vincent._chancyLerp = 0.0f;
            Vincent.hasKid = false;
            Vincent._allowMovement = false;
            Vincent._afterShowWait = 0.0f;
            Vincent.show = false;
            if (Profiles.experienceProfile == null)
                return;
            Vincent.openedCorners = false;
            Music.Play("vincent");
            Vincent._dealer = Vincent._hasYoYo ? new SpriteMap("vincent", 113, 106) : new SpriteMap("vincentNoYo", 113, 106);
            Vincent._selectDescIndex = -2;
            Vincent._selectIndex = -1;
            Vincent.hasKid = false;
            Vincent.type = t;
            switch (t)
            {
                case DayType.Special:
                    Vincent.Add("|CALM|I've got something special for you today, |SHOW||POINT|Check it out!");
                    break;
                case DayType.SaleDay:
                    if (Profiles.experienceProfile.timesMetVincentSale == 0)
                    {
                        Vincent.Add("|CALM|Hey!|2| |POINT|You're new to this so let me explain.");
                        Vincent.Add("|CALM|At the end of every month I have a super sale.");
                        Vincent.Add("|CONCERNED|Where I sell stuff I can't move, |POINT|at WAY LOW PRICES!");
                        Vincent.Add("|CALM||SHOW|Check it out!");
                    }
                    else
                    {
                        List<List<string>> stringListList = new List<List<string>>();
                        stringListList.Add(new List<string>()
            {
              "|CONCERNED|Hey, guess what? |POINT||SHOW|It's SALE DAY!"
            });
                        stringListList.Add(new List<string>()
            {
              "|CALM|Dang, I hope you're ready.. |SHOW|For |POINT|INSANE SAVINGS."
            });
                        stringListList.Add(new List<string>()
            {
              "|CALM|+SAAALE |SHOW|DAAAAY+!"
            });
                        stringListList.Add(new List<string>()
            {
              "|CALM|Oh wow, now here's some |SHOW|quality stuff."
            });
                        foreach (string line in stringListList[Rando.Int(stringListList.Count - 1)])
                            Vincent.Add(line);
                    }
                    Vincent.hasKid = new List<int>()
          {
            0,
            1,
            2,
            3,
            4,
            6,
            9,
            12,
            18,
            28,
            51,
            52,
            53,
            80,
            140
          }.Contains(Profiles.experienceProfile.timesMetVincentSale);
                    ++Profiles.experienceProfile.timesMetVincentSale;
                    break;
                case DayType.ImportDay:
                    if (Profiles.experienceProfile.timesMetVincentImport == 0)
                    {
                        Vincent.Add("|CALM|OK. On the first day of every month I have a |POINT||GREEN|SPECIAL SALE|WHITE|!");
                        Vincent.Add("|CALM|Today I sell only the |POINT||GREEN|FANCIEST IMPORTS|GREEN|.");
                        Vincent.Add("|CONCERNED|This stuff aint cheap!|POINT||SHOW| See anything you like?");
                    }
                    else
                    {
                        List<List<string>> stringListList = new List<List<string>>()
            {
              new List<string>()
              {
                "|POINT|Hey hey hey!",
                "|POINT||SHOW|It's FANCY IMPORTS day!"
              },
              new List<string>()
              {
                "|CALM|Ahh, Fancy Imports day!",
                "|CALM|Hope you're ready for some |SHOW|EXOTIC FURNITURE."
              },
              new List<string>()
              {
                "|CALM|W-what's that?",
                "|POINT|OOH-HUHUU! |SHOW|Is that IMPORTED?"
              }
            };
                        if (Rando.Int(40) == 0)
                            stringListList.Add(new List<string>()
              {
                "|POINT|Fancy fancy imports day!",
                "|POINT|Get ready to overpay baby!|SHOW|"
              });
                        else if (Rando.Int(100) == 0)
                            stringListList.Add(new List<string>()
              {
                "|CONCERNED|Hmm, I actually had this stuff reserved for the Duck Duke of the Dark Spire..",
                "|POINT|But hey you're basically him so,|SHOW| have at 'er."
              });
                        foreach (string line in stringListList[Rando.Int(stringListList.Count - 1)])
                            Vincent.Add(line);
                    }
                    Vincent.hasKid = new List<int>()
          {
            3,
            6,
            9
          }.Contains(Profiles.experienceProfile.timesMetVincentImport);
                    ++Profiles.experienceProfile.timesMetVincentImport;
                    break;
                case DayType.PawnDay:
                    if (Profiles.experienceProfile.timesMetVincentSell == 0)
                    {
                        Vincent.Add("|CALM|I keep an eye out on all the furniture that goes around here.");
                        Vincent.Add("|CALM|Every second Wednesday I'll come see |POINT|if you have anything I like!");
                        Vincent.Add("|CONCERNED|If something catches my eye I'll try to buy it from you..");
                        Vincent.Add("|POINT|So!");
                    }
                    List<Furniture> list1 = Profiles.experienceProfile.GetAvailableFurnis().Where<Furniture>(x => Profiles.experienceProfile.GetNumFurnitures(x.index) > Profiles.experienceProfile.GetNumFurnituresPlaced(x.index) && x.group != Furniture.Momento && x.group != Furniture.Default).ToList<Furniture>();
                    if (list1.Count<Furniture>() > 0)
                    {
                        List<Furniture> list2 = list1.OrderBy<Furniture, float>(x => Rando.Float(1f) - Math.Min(Profiles.experienceProfile.GetNumFurnitures(x.index) / 10f, 1f) * 0.5f).ToList<Furniture>();
                        int num = Rando.Int(3);
                        Vincent.products.Clear();
                        VincentProduct vincentProduct = new VincentProduct()
                        {
                            type = VPType.Furniture,
                            furnitureData = list2.First<Furniture>()
                        };
                        vincentProduct.originalCost = vincentProduct.furnitureData.price;
                        vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 2.5);
                        if ((double)Rando.Float(1f) > 0.949999988079071)
                            vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 4.0);
                        if (vincentProduct.furnitureData.name == "ROUND TABLE" && (double)Rando.Float(1f) > 0.5)
                        {
                            Vincent.Add("|POINT|Oh baby, is that one of them ROUND tables??");
                            Vincent.Add("|CONCERNED| Can I buy it from you?|SHOW| I'm so tired of square tables...");
                        }
                        else
                        {
                            switch (num)
                            {
                                case 0:
                                    Vincent.Add("|POINT|I like the look of this,|SHOW| you wanna sell it?");
                                    break;
                                case 1:
                                    Vincent.Add("|POINT|This is pretty cool,|SHOW| ya still in love with it? Cause I am!");
                                    break;
                                case 2:
                                    Vincent.Add("|POINT|I haven't seen one of these in ages!|SHOW| Can I buy it from you?");
                                    break;
                                case 3:
                                    Vincent.Add("|POINT|I mean, why do you even have one of these!?|SHOW| I can take it off your hands..");
                                    break;
                            }
                        }
                        Vincent.products.Add(vincentProduct);
                    }
                    else
                    {
                        Vincent.Add("|CALM|Let's see what I could buy from you...");
                        Vincent.Add("|CONCERNED|Looks like you don't have anything I want, sorry!|SHOW|");
                    }
                    Vincent.hasKid = new List<int>()
          {
            4,
            8,
            10,
            22,
            45
          }.Contains(Profiles.experienceProfile.timesMetVincentSell);
                    ++Profiles.experienceProfile.timesMetVincentSell;
                    break;
                case DayType.HintDay:
                    Vincent.products.Clear();
                    List<List<string>> stringListList1 = new List<List<string>>()
          {
            new List<string>()
            {
              "|CALM|Hey, word is that there's a hat you don't have yet...",
              "|POINT|and I caught word of how to unlock it-"
            },
            new List<string>()
            {
              "|CALM|I just heard a rumour about how to unlock a new hat-"
            },
            new List<string>()
            {
              "|CONCERNED|Probably shouldn't be telling you this, but-",
              "|POINT|I heard of a way to unlock a new hat!"
            },
            new List<string>()
            {
              "|CONCERNED|PSST!",
              "|POINT|They say you can unlock a new hat if you just do this-"
            }
          };
                    if (Rando.Int(10) == 0)
                        stringListList1.Add(new List<string>()
            {
              "|CALM|Hats my man, that's what life's all about!",
              "|POINT|And I happen to know how you can get a new one-"
            });
                    List<List<string>> stringListList2 = new List<List<string>>()
          {
            new List<string>()
            {
              "|CONCERNED|Huh, Doesn't sound too hard...|SHOW|"
            },
            new List<string>()
            {
              "|CONCERNED|Now why would anybody wanna do that?|SHOW|"
            },
            new List<string>()
            {
              "|CONCERNED|Sounds like a real pain...|SHOW|"
            },
            new List<string>() { "|POINT|It's just that easy!|SHOW|" },
            new List<string>()
            {
              "|CONCERNED|Even I could do that...|SHOW|"
            },
            new List<string>()
            {
              "|CALM|That's just what I heard, anyway.|SHOW|"
            },
            new List<string>()
            {
              "|CALM|+ |DGBLUE|So easy!|WHITE| +|SHOW|"
            },
            new List<string>()
            {
              "|CONCERNED|Geez... Glad I don't have to do that.|SHOW|"
            }
          };
                    if (Rando.Int(10) == 0)
                    {
                        stringListList2.Add(new List<string>()
            {
              "|CALM|Whatever that means, right?|SHOW|"
            });
                        stringListList2.Add(new List<string>()
            {
              "|CALM|No problem, Eh?|SHOW|"
            });
                        if (Profiles.experienceProfile.timesMetVincent > 10)
                            stringListList2.Add(new List<string>()
              {
                "|POINT|See? Don't even need the wiki.|SHOW|"
              });
                    }
                    else if (Rando.Int(100) == 0)
                    {
                        stringListList2.Add(new List<string>()
            {
              "|CALM|Yep.. Good luck with that!|SHOW|"
            });
                        if (Profiles.experienceProfile.timesMetVincent > 15)
                            stringListList2.Add(new List<string>()
              {
                "|CALM|Hey, I'd do it for you if I had time...|SHOW|"
              });
                        stringListList2.Add(new List<string>()
            {
              "|CONCERNED|Hmm.. Do you really wanna do all that?|SHOW|"
            });
                    }
                    else if (Rando.Int(10000) == 0)
                    {
                        stringListList2.Add(new List<string>()
            {
              "|POINT|If you want my advice, just edit your save file..|SHOW|"
            });
                        stringListList2.Add(new List<string>()
            {
              "|POINT|Hmpf.. Nobody's got time for that!|SHOW|"
            });
                    }
                    if (Rando.Int(100) == 0)
                    {
                        stringListList1.Clear();
                        stringListList1.Add(new List<string>()
            {
              "|CALM|Hey, my fortune cookie gave me a hint on how to unlock a new hat-"
            });
                        stringListList2.Clear();
                        stringListList2.Add(new List<string>()
            {
              "|CONCERNED|That's all well and good but.. what's my fortune, then?|SHOW|"
            });
                    }
                    else if (Rando.Int(100) == 0 && Profiles.experienceProfile.timesMetVincent > 12)
                    {
                        stringListList1.Clear();
                        stringListList1.Add(new List<string>()
            {
              "|POINT|Since we're buds I'll let you in on the secret to unlocking a new hat:"
            });
                        stringListList2.Clear();
                        stringListList2.Add(new List<string>()
            {
              "|CONCERNED|That info's just between you and me, right?|SHOW|"
            });
                    }
                    else if (Rando.Int(30) == 0)
                    {
                        stringListList1.Clear();
                        stringListList1.Add(new List<string>()
            {
              "|POINT|Hey.. Wanna know how to unlock a new hat?",
              "|CALM|Someone scratched this hint into the wall of the crapper-"
            });
                        stringListList2.Clear();
                        stringListList2.Add(new List<string>()
            {
              "|CONCERNED|Huh? No way- I didn't scratch it in there, I use a permanent marker.|SHOW|"
            });
                    }
                    List<string> stringList = stringListList1[Rando.Int(stringListList1.Count - 1)];
                    List<Unlockable> lockedItems = Unlockables.lockedItems;
                    Global.data.unlockListIndex += 1 + Rando.Int(10);
                    if (lockedItems.Count > 0)
                    {
                        int index = Global.data.unlockListIndex % lockedItems.Count;
                        Unlockable unlockable = lockedItems[index];
                        stringList.Add("'|GREEN|" + unlockable.description + "|WHITE|'");
                    }
                    foreach (string str in stringListList2[Rando.Int(stringListList2.Count - 1)])
                        stringList.Add(str);
                    foreach (string line in stringList)
                        Vincent.Add(line);
                    Vincent.hasKid = new List<int>()
          {
            3,
            5,
            7,
            10,
            14,
            19,
            25,
            35,
            50,
            80,
            120,
            121,
            122,
            180,
            300
          }.Contains(Profiles.experienceProfile.timesMetVincentHint);
                    ++Profiles.experienceProfile.timesMetVincentHint;
                    break;
                default:
                    Vincent.hasKid = new List<int>()
          {
            1,
            3,
            5,
            9,
            11,
            16,
            19,
            31,
            45,
            50,
            51,
            52,
            60,
            62,
            64,
            66,
            68,
            70,
            90,
            110,
            150,
            200,
            300,
            500
          }.Contains(Profiles.experienceProfile.timesMetVincent);
                    if (Profiles.experienceProfile.timesMetVincent >= 19 && Profiles.experienceProfile.GetNumFurnitures(RoomEditor.GetFurniture("YOYO").index) <= 0)
                    {
                        Vincent._willGiveYoYo = true;
                        Vincent.Add("|CONCERNED|You know what? |CALM|You're my best customer.");
                        Vincent.Add("|POINT|Hands down. You are awesome");
                        Vincent.Add("|CONCERNED|I want you to have this...|2||GIVE|");
                        Vincent.Add("|POINT|Now then, |SHOW|Here's what I got!");
                        Vincent.hasKid = false;
                    }
                    else if (Profiles.experienceProfile.numLittleMen > 0 && Profiles.experienceProfile.GetNumFurnitures(RoomEditor.GetFurniture("VOODOO VINCENT").index) <= 0)
                    {
                        Vincent._willGiveVooDoo = true;
                        Vincent.Add("|CONCERNED|Hey, I've got something special for you today..");
                        Vincent.Add("|POINT|I'm required by Duck Game Law to give it to you.");
                        Vincent.Add("|CONCERNED|Lets see here, I know I've got it around here somewhere.. |2||GIVE|");
                        Vincent.Add("|POINT|If you put it in your room it'll give you the option to.. |CONCERNED|Er.. Well uh.. to make me go away.");
                        Vincent.Add("|CALM|You'll still get new Furni and XP if you skip the level up screen, but you'll miss my handsome mug.");
                        Vincent.Add("|POINT|Now then, |SHOW|Here's what I've got for today!");
                        Vincent.hasKid = false;
                    }
                    else if (UILevelBox.currentLevel > 2 && Profiles.experienceProfile.punished >= 10 && Profiles.experienceProfile.GetNumFurnitures(RoomEditor.GetFurniture("PERIMETER DEFENCE").index) <= 0)
                    {
                        Vincent._willGivePerimeterDefence = true;
                        Vincent.Add("|CONCERNED|Hey.. I noticed other Ducks have been comin' into your room and givin' you a hard time.");
                        Vincent.Add("|CONCERNED|That's not cool, you got a little man's safety to worry about!");
                        Vincent.Add("|POINT|I'm a bit of a pacifist, but I bought this thing on sale and I want you to have it.");
                        Vincent.Add("|CONCERNED|Let's see here...|3||GIVE|");
                        Vincent.Add("|CALM|That thing's just to protect the home room, okay? Be careful with it!");
                        Vincent.Add("|POINT|Now then, |SHOW|Here's what I've got for today!");
                        Vincent.hasKid = false;
                    }
                    else if (Profiles.experienceProfile.timesMetVincent == 0)
                    {
                        Vincent.Add("|POINT|Hey, I'm Vincent and I sell toys.");
                        Vincent.Add("|CALM|I'm around every |GREEN|Friday|WHITE| and have new stuff every week.");
                        Vincent.Add("|CONCERNED|My stuff is a sure thing too, no gambling here.|2|");
                        Vincent.Add("|POINT|SO! |SHOW|See anything you like?");
                    }
                    else if (Profiles.experienceProfile.timesMetVincent == 1)
                    {
                        Vincent.Add("|POINT|HEY!");
                        Vincent.Add("|CALM|This is my son, |DGBLUE|Mini Vinny|WHITE|.");
                        Vincent.Add("|CALM|He's helpin' me sell toys.");
                        Vincent.Add("|POINT|His mom|3||CONCERNED| would|1| rather he |RED|didn't|WHITE|.|3|");
                        Vincent.Add("|CONCERNED|But|2||CALM| this is |DGBLUE|our time|WHITE|. |SHOW|and ol' |DGBLUE|mini|WHITE| |GREEN|LOVES|WHITE| sellin' toys.");
                    }
                    else if (Profiles.experienceProfile.timesMetVincent == 7)
                        Vincent.Add("|CONCERNED|Here|SHOW| you go...");
                    else if (Profiles.experienceProfile.timesMetVincent == 8)
                    {
                        Vincent.Add("|POINT|Dang, It's good to see you.");
                        Vincent.Add("|CONCERNED|You're my best customer, |SHOW|you know that?");
                    }
                    else if (Profiles.experienceProfile.timesMetVincent == 13)
                    {
                        Vincent.Add("|CALM|Friday is my favourite day 'cause we get to hang out.");
                        Vincent.Add("|POINT|I'm not just sayin' that|SHOW| cause you buy stuff!");
                    }
                    else if (Profiles.experienceProfile.timesMetVincent == 22)
                    {
                        Vincent.Add("|CONCERNED|Today was a dang mess, but this makes it all worthwhile.");
                        Vincent.Add("|POINT|+Sellin' |SHOW|toys is THE BEST+");
                    }
                    else
                    {
                        List<List<string>> stringListList3 = new List<List<string>>()
            {
              new List<string>()
              {
                "|CONCERNED|I hope you're ready,|2| cause today I've got...|0|",
                "|POINT|The |SHOW||GREEN|GREATEST PRODUCT LINEUP IN VINCENT HISTORY|WHITE|."
              },
              new List<string>()
              {
                "|CALM|Just got back from the dump.",
                "|POINT|Hope you like |GREEN|VALUE|WHITE|.|SHOW|"
              },
              new List<string>()
              {
                "|CONCERNED|Where do I even find this stuff?",
                "|CALM|You just can't find stuff like this|2| |SHOW||POINT|ANY. WHERE. ELSE."
              },
              new List<string>()
              {
                "|CALM|Look at all this stuff, |CONCERENED|I don't even want to sell it.",
                "|POINT||SHOW|I wanna keep this stuff, It's just too cool."
              },
              new List<string>()
              {
                "|CALM|Look at that. |GREEN|4 GOOD REASONS|WHITE||2| |SHOW|why today is a good day."
              },
              new List<string>()
              {
                "|POINT|I just finished putting prices on everything.",
                "|CONCERNED|WHOOPS! Looks like I accidentally|SHOW| made this stuff way too cheap..."
              }
            };
                        if (Profiles.experienceProfile.timesMetVincent > 2 && Rando.Int(100) == 0)
                            stringListList3.Add(new List<string>()
              {
                "|CALM|+GONNA BUY MYSELF |SHOW|A GREY GUITAR+"
              });
                        if (Profiles.experienceProfile.timesMetVincent > 2 && Rando.Int(100) == 0)
                        {
                            stringListList3.Clear();
                            stringListList3.Add(new List<string>()
              {
                "|CALM|+BING AND BONG+                +|SHOW|THERE'S A TINY PLANET CALLING+"
              });
                        }
                        if ((Profiles.experienceProfile.timesMetVincent > 15 && Profiles.experienceProfile.timesMetVincent < 45 || Profiles.experienceProfile.timesMetVincent > 100 && Profiles.experienceProfile.timesMetVincent < 125) && Rando.Int(10000) == 0)
                            stringListList3.Add(new List<string>()
              {
                "|CALM|+I'M GOIN TO CALIFORNIA+ |SHOW|+GONNA LIVE THE LIFE+"
              });
                        if ((Profiles.experienceProfile.timesMetVincent > 30 && Profiles.experienceProfile.timesMetVincent < 45 || Profiles.experienceProfile.timesMetVincent > 100 && Profiles.experienceProfile.timesMetVincent < 125) && Rando.Int(10000) == 0)
                            stringListList3.Add(new List<string>()
              {
                "|CALM|+BACKSTREET'S BACK+ |SHOW|+ALRIGHT!!+"
              });
                        foreach (string line in stringListList3[Rando.Int(stringListList3.Count - 1)])
                            Vincent.Add(line);
                    }
                    ++Profiles.experienceProfile.timesMetVincent;
                    break;
            }
            if (Profiles.experienceProfile.timesMetVincent < 1)
                Vincent.hasKid = false;
            if (t == DayType.PawnDay || t == DayType.HintDay)
                return;
            Vincent.GenerateProducts();
        }

        public static void GenerateProducts()
        {
            Vincent.products.Clear();
            switch (Vincent.type)
            {
                case DayType.Shop:
                    using (List<Furniture>.Enumerator enumerator = UIGachaBox.GetRandomFurniture(Rarity.VeryRare, 4, 0.4f, numDupes: 1).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Furniture current = enumerator.Current;
                            VincentProduct vincentProduct = new VincentProduct
                            {
                                type = VPType.Furniture,
                                furnitureData = current
                            };
                            if (Rando.Int(120) == 0)
                            {
                                vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 0.5);
                                vincentProduct.originalCost = vincentProduct.furnitureData.price;
                            }
                            else
                                vincentProduct.cost = vincentProduct.originalCost = vincentProduct.furnitureData.price;
                            Vincent.products.Add(vincentProduct);
                        }
                        break;
                    }
                case DayType.Special:
                    IEnumerable<Team> source = new List<Team>()
          {
            Teams.GetTeam("CYCLOPS"),
            Teams.GetTeam("BIG ROBO"),
            Teams.GetTeam("TINCAN"),
            Teams.GetTeam("WELDERS"),
            Teams.GetTeam("PONYCAP"),
            Teams.GetTeam("TRICORNE"),
            Teams.GetTeam("TWINTAIL"),
            Teams.GetTeam("HIGHFIVES"),
            Teams.GetTeam("MOTHERS")
          }.Where<Team>(x => !Global.boughtHats.Contains(x.name));
                    if (source.Count<Team>() <= 0)
                    {
                        using (List<Furniture>.Enumerator enumerator = UIGachaBox.GetRandomFurniture(Rarity.VeryVeryRare, 1, 0.4f).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Furniture current = enumerator.Current;
                                VincentProduct vincentProduct = new VincentProduct()
                                {
                                    type = VPType.Furniture,
                                    furnitureData = current
                                };
                                vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 0.5);
                                vincentProduct.originalCost = vincentProduct.furnitureData.price;
                                Vincent.products.Add(vincentProduct);
                            }
                            break;
                        }
                    }
                    else
                    {
                        VincentProduct vincentProduct = new VincentProduct()
                        {
                            type = VPType.Hat
                        };
                        vincentProduct.cost = vincentProduct.originalCost = 150;
                        vincentProduct.teamData = source.ElementAt<Team>(Rando.Int(source.Count<Team>() - 1));
                        Vincent.products.Add(vincentProduct);
                        break;
                    }
                case DayType.SaleDay:
                    using (List<Furniture>.Enumerator enumerator = UIGachaBox.GetRandomFurniture(Rarity.Common, 4, 2f, rareDupesChance: true).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Furniture current = enumerator.Current;
                            VincentProduct vincentProduct = new VincentProduct()
                            {
                                type = VPType.Furniture,
                                furnitureData = current
                            };
                            vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 0.5);
                            vincentProduct.originalCost = vincentProduct.furnitureData.price;
                            Vincent.products.Add(vincentProduct);
                        }
                        break;
                    }
                case DayType.ImportDay:
                    IOrderedEnumerable<Furniture> orderedEnumerable = UIGachaBox.GetRandomFurniture(Rarity.VeryVeryRare, 8, 0.4f, avoidDupes: true).OrderBy<Furniture, int>(x => -x.rarity);
                    int num = 0;
                    using (IEnumerator<Furniture> enumerator = orderedEnumerable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Furniture current = enumerator.Current;
                            VincentProduct vincentProduct = new VincentProduct
                            {
                                type = VPType.Furniture,
                                furnitureData = current
                            };
                            if (Rando.Int(50) == 0)
                            {
                                vincentProduct.furnitureData = UIGachaBox.GetRandomFurniture(Rarity.SuperRare, 1, 0.3f)[0];
                                vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 2.0);
                                vincentProduct.originalCost = vincentProduct.furnitureData.price;
                            }
                            else
                            {
                                vincentProduct.cost = (int)(vincentProduct.furnitureData.price * 1.5);
                                vincentProduct.originalCost = vincentProduct.furnitureData.price;
                            }
                            Vincent.products.Add(vincentProduct);
                            ++num;
                            if (num == 4)
                                break;
                        }
                        break;
                    }
            }
        }

        public static void Initialize()
        {
            if (Vincent._tail != null)
                return;
            Vincent._dealer = new SpriteMap("vincent", 113, 106);
            Vincent._tail = new Sprite("arcade/bubbleTail");
            Vincent._font = new FancyBitmapFont("smallFont");
            Vincent._priceFont = new BitmapFont("biosFontSideways", 8);
            Vincent._priceFontCrossout = new BitmapFont("biosFontSidewaysCrossout", 8);
            Vincent._priceFontRightways = new BitmapFont("priceFontRightways", 8);
            Vincent._descriptionFont = new BitmapFont("biosFontDescriptions", 8);
            Vincent._photo = new Sprite("arcade/challengePhoto");
            Vincent._furniFrame = new Sprite("furniFrame");
            Vincent._furniFrame.CenterOrigin();
            Vincent._furniFill = new Sprite("furniFill");
            Vincent._furniFill.CenterOrigin();
            Vincent._furniHov = new Sprite("furniHov");
            Vincent._furniHov.CenterOrigin();
            Vincent._soldSprite = new Sprite("soldStamp");
            Vincent._soldSprite.CenterOrigin();
            Vincent._newSticker = new SpriteMap("newSticker", 29, 28);
            Vincent._newSticker.CenterOrigin();
            Vincent._rareSticker = new SpriteMap("rareSticker", 29, 28);
            Vincent._rareSticker.CenterOrigin();
            Vincent._furniTag = new SpriteMap("furniTag", 14, 51);
            Vincent._cheapTape = new Sprite("cheapTape");
            Vincent._cheapTape.CenterOrigin();
            Vincent._bigBanner = new Sprite("bigBanner");
            Vincent._fancyBanner = new Sprite("fancyBanner");
            Vincent._priceTargets.Add(new RenderTarget2D(64, 16));
            Vincent._priceTargets.Add(new RenderTarget2D(64, 16));
            Vincent._priceTargets.Add(new RenderTarget2D(64, 16));
            Vincent._priceTargets.Add(new RenderTarget2D(64, 16));
        }

        public static void ChangeSpeech()
        {
            Vincent.Clear();
            Vincent._selectDescIndex = Vincent._selectIndex;
            if (Vincent.products[Vincent._selectIndex].sold)
            {
                if (Vincent._soldSelectIndex == Vincent._selectIndex)
                {
                    if (Vincent.products.Where<VincentProduct>(x => !x.sold).Count<VincentProduct>() == 0)
                    {
                        Vincent.Add("|CONCERNED|WOAH...");
                        Vincent.Add("|POINT|YOU BOUGHT EVERYTHING!");
                    }
                    else
                    {
                        List<string> stringList = new List<string>()
            {
              "|CONCERNED|AND THERE YOU GO...|0|",
              "|CONCERNED|FINALLY GOT RID OF IT!",
              "|CALM|SOLD!"
            };
                        if (Profiles.experienceProfile.timesMetVincent > 10)
                        {
                            stringList.Add("|POINT|AND NOW ITS YOURS!");
                            stringList.Add("|CALM|SOLD!");
                            if (Rando.Int(10) == 0)
                                stringList.Add("|CONCERNED|GLAD SOMEONE LIKES THAT SORT OF THING..");
                        }
                        if (Profiles.experienceProfile.timesMetVincent > 30)
                        {
                            stringList.Add("|POINT|TAKE GOOD CARE OF IT!");
                            stringList.Add("|CONCERNED|HOPE YOU LIKE IT.");
                            stringList.Add("|POINT|NOW THAT'S GOOD TASTE!");
                        }
                        Vincent.Add(stringList[Rando.Int(stringList.Count - 1)]);
                    }
                    Vincent._soldSelectIndex = -1;
                }
                else
                {
                    Vincent.Add("|CONCERNED|GONNA MISS THAT ONE.");
                    Vincent._soldSelectIndex = -1;
                }
            }
            else
            {
                string str = Vincent.products[Vincent._selectIndex].description;
                if (str == "")
                    str = Vincent.products[Vincent._selectIndex].furnitureData != null ? "What a fine piece of furniture." : "What a fine hat.";
                int type = (int)Vincent.products[Vincent._selectIndex].type;
                Vincent.Add(str + "^|ORANGE|Part of the '" + Vincent.products[Vincent._selectIndex].group + "' line.|WHITE| " + "|DGGREEN|$" + Convert.ToString(Vincent.products[Vincent._selectIndex].cost));
            }
        }

        public static void Sold()
        {
            Vincent.products[FurniShopScreen.attemptBuyIndex].sold = true;
            Vincent._soldSelectIndex = FurniShopScreen.attemptBuyIndex;
        }

        public static void Update()
        {
            if (Vincent._hasYoYo && Profiles.experienceProfile.timesMetVincent > 19 && !FurniShopScreen.giveYoYo && !Vincent._willGiveYoYo)
            {
                Vincent._dealer = new SpriteMap("vincentNoYo", 113, 106);
                Vincent._hasYoYo = false;
            }
            else if (!Vincent._hasYoYo && Profiles.experienceProfile.timesMetVincent < 20)
            {
                Vincent._dealer = new SpriteMap("vincent", 113, 106);
                Vincent._hasYoYo = true;
            }
            Vincent.Initialize();
            if (UILevelBox.menuOpen)
                return;
            Vincent._showLerp = Lerp.FloatSmooth(Vincent._showLerp, Vincent.show ? 1f : 0.0f, 0.09f, 1.05f);
            bool flag1 = Vincent.lookingAtList && _challengeLerp < 0.300000011920929;
            bool flag2 = Vincent.lookingAtChallenge && _listLerp < 0.300000011920929;
            bool flag3 = FurniShopScreen.open && _listLerp < 0.300000011920929;
            Vincent._listLerp = Lerp.FloatSmooth(Vincent._listLerp, flag1 ? 1f : 0.0f, 0.2f, 1.05f);
            Vincent._challengeLerp = Lerp.FloatSmooth(Vincent._challengeLerp, flag2 ? 1f : 0.0f, 0.2f, 1.05f);
            Vincent._chancyLerp = Lerp.FloatSmooth(Vincent._chancyLerp, flag3 ? 1f : 0.0f, 0.2f, 1.05f);
            Vincent.alpha = !FurniShopScreen.open ? Lerp.Float(Vincent.alpha, 0.0f, 0.05f) : Lerp.Float(Vincent.alpha, 1f, 0.05f);
            if (!FurniShopScreen.open || Vincent.showingDay)
                return;
            if (!Vincent.openedCorners)
            {
                Vincent.openedCorners = true;
                HUD.ClearCorners();
                if (Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && !Vincent._willGiveVooDoo && !Vincent._willGiveYoYo && !Vincent._willGivePerimeterDefence)
                {
                    HUD.AddCornerControl(HUDCorner.TopLeft, "@START@SKIP");
                    HUD.AddCornerMessage(HUDCorner.BottomLeft, "@CANCEL@DITCH");
                }
            }
            if (Input.Pressed("START") && Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && !Vincent._willGiveVooDoo && !Vincent._willGiveYoYo && !Vincent._willGivePerimeterDefence)
            {
                DuckGame.Graphics.fadeAdd = 1f;
                SFX.skip = false;
                SFX.Play("dacBang");
                FurniShopScreen.close = true;
            }
            else
            {
                int num1;
                if (Vincent._allowMovement)
                {
                    if ((Input.Pressed("ANY") || Vincent.type == DayType.PawnDay && Vincent.products.Count > 0) && Vincent._selectIndex == -1)
                    {
                        Vincent._selectIndex = 0;
                        if (Vincent.products.Count != 0)
                            return;
                        FurniShopScreen.close = true;
                        return;
                    }
                    if (Input.Pressed("MENULEFT"))
                    {
                        switch (Vincent._selectIndex)
                        {
                            case -1:
                                Vincent._selectIndex = 0;
                                break;
                            case 1:
                                Vincent._selectIndex = 0;
                                SFX.Play("textLetter", 0.7f);
                                break;
                            case 3:
                                Vincent._selectIndex = 2;
                                SFX.Play("textLetter", 0.7f);
                                break;
                        }
                    }
                    if (Input.Pressed("MENURIGHT"))
                    {
                        switch (Vincent._selectIndex)
                        {
                            case -1:
                                Vincent._selectIndex = 0;
                                break;
                            case 0:
                                Vincent._selectIndex = 1;
                                SFX.Play("textLetter", 0.7f);
                                break;
                            case 2:
                                Vincent._selectIndex = 3;
                                SFX.Play("textLetter", 0.7f);
                                break;
                        }
                    }
                    if (Input.Pressed("MENUUP"))
                    {
                        switch (Vincent._selectIndex)
                        {
                            case -1:
                                Vincent._selectIndex = 0;
                                break;
                            case 2:
                                Vincent._selectIndex = 0;
                                SFX.Play("textLetter", 0.7f);
                                break;
                            case 3:
                                Vincent._selectIndex = 1;
                                SFX.Play("textLetter", 0.7f);
                                break;
                        }
                    }
                    if (Input.Pressed("MENUDOWN"))
                    {
                        switch (Vincent._selectIndex)
                        {
                            case -1:
                                Vincent._selectIndex = 0;
                                break;
                            case 0:
                                Vincent._selectIndex = 2;
                                SFX.Play("textLetter", 0.7f);
                                break;
                            case 1:
                                Vincent._selectIndex = 3;
                                SFX.Play("textLetter", 0.7f);
                                break;
                        }
                    }
                    if (Vincent._selectIndex >= Vincent.products.Count)
                        Vincent._selectIndex = Vincent.products.Count - 1;
                    if (Input.Pressed("SELECT") && Vincent._selectIndex != -1 && !Vincent.products[Vincent._selectIndex].sold && (Vincent.type == DayType.PawnDay || Vincent.products[Vincent._selectIndex].cost <= Profiles.experienceProfile.littleManBucks))
                    {
                        FurniShopScreen.attemptBuy = Vincent.products[Vincent._selectIndex];
                        Vincent._selectDescIndex = -1;
                        FurniShopScreen.attemptBuyIndex = Vincent._selectIndex;
                        HUD.CloseCorner(HUDCorner.BottomLeft);
                        HUD.AddCornerMessage(HUDCorner.BottomMiddle, "|YELLOW|$" + Profiles.experienceProfile.littleManBucks.ToString());
                        if (Vincent.products[Vincent._selectIndex].furnitureData == null)
                            return;
                        HUD.AddCornerMessage(HUDCorner.BottomRight, "|DGBLUE|HAVE " + Profiles.experienceProfile.GetNumFurnitures(products[_selectIndex].furnitureData.index).ToString());
                        return;
                    }
                    if (Vincent._selectDescIndex != Vincent._selectIndex)
                    {
                        if (Vincent._selectIndex == -1)
                        {
                            HUD.CloseAllCorners();
                            HUD.AddCornerMessage(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                            Vincent._selectDescIndex = Vincent._selectIndex;
                        }
                        else
                        {
                            if (Vincent.type != DayType.PawnDay)
                                Vincent.ChangeSpeech();
                            num1 = Profiles.experienceProfile.littleManBucks;
                            HUD.AddCornerMessage(HUDCorner.BottomMiddle, "|YELLOW|$" + num1.ToString());
                            int num2 = Vincent.products[Vincent._selectIndex].furnitureData != null ? Profiles.experienceProfile.GetNumFurnitures(products[_selectIndex].furnitureData.index) : 0;
                            if (Vincent.type == DayType.PawnDay)
                            {
                                if (Vincent.products[Vincent._selectIndex].sold)
                                {
                                    HUD.AddCornerMessage(HUDCorner.BottomRight, "SOLD");
                                }
                                else
                                {
                                    string text = "@SELECT@SELL";
                                    if (num2 > 1)
                                        text = text + "|DGBLUE|(HAVE " + num2.ToString() + ")";
                                    HUD.AddCornerMessage(HUDCorner.BottomRight, text);
                                }
                            }
                            else if (Vincent.products[Vincent._selectIndex].sold)
                            {
                                HUD.AddCornerMessage(HUDCorner.BottomRight, "BOUGHT");
                            }
                            else
                            {
                                string text = Vincent.products[Vincent._selectIndex].cost > Profiles.experienceProfile.littleManBucks ? "@SELECT@|RED|BUY" : "@SELECT@BUY";
                                if (num2 > 1)
                                    text = text + "|DGBLUE|(HAVE " + num2.ToString() + ")";
                                HUD.AddCornerMessage(HUDCorner.BottomRight, text);
                            }
                            HUD.AddCornerMessage(HUDCorner.BottomLeft, "@CANCEL@EXIT");
                            Vincent._selectDescIndex = Vincent._selectIndex;
                        }
                    }
                }
                bool flag4 = !Vincent._allowMovement && Input.Down("SELECT");
                if (Vincent._lines.Count > 0 && Vincent._currentLine == "")
                {
                    int num3 = _waitAfterLine <= 0.0 ? 1 : 0;
                    Vincent._waitAfterLine -= 0.045f;
                    if (flag4)
                        Vincent._waitAfterLine -= 0.045f;
                    if (Vincent.killSkip)
                        Vincent._waitAfterLine -= 0.1f;
                    Vincent._talkMove += 0.75f;
                    if (_talkMove > 1.0)
                    {
                        Vincent.frame = 0;
                        Vincent._talkMove = 0.0f;
                    }
                    if (num3 == 0 && _waitAfterLine <= 0.0)
                        HUD.AddCornerMessage(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                    if (Vincent._lineProgress.Count == 0 || Input.Pressed("SELECT"))
                    {
                        Vincent._lineProgress.Clear();
                        Vincent._currentLine = Vincent._lines[0];
                        Vincent._lines.RemoveAt(0);
                        Vincent._waitAfterLine = 1.3f;
                        if (Vincent.show)
                            Vincent._allowMovement = true;
                        Vincent.killSkip = false;
                    }
                }
                if (Vincent._currentLine != "")
                {
                    Vincent._waitLetter -= 0.9f;
                    if (flag4)
                        Vincent._waitLetter -= 1.8f;
                    if (_waitLetter < 0.0)
                    {
                        Vincent._talkMove += 0.75f;
                        if (_talkMove > 1.0)
                        {
                            Vincent.frame = Vincent._currentLine[0] == ' ' || Vincent.frame != 1 || _extraWait > 0.0 ? 1 : 2;
                            Vincent._talkMove = 0.0f;
                        }
                        Vincent._waitLetter = 1f;
                        char ch1;
                        while (Vincent._currentLine[0] == '@')
                        {
                            ch1 = Vincent._currentLine[0];
                            string str1 = ch1.ToString() ?? "";
                            for (Vincent._currentLine = Vincent._currentLine.Remove(0, 1); Vincent._currentLine[0] != '@' && Vincent._currentLine.Length > 0; Vincent._currentLine = Vincent._currentLine.Remove(0, 1))
                            {
                                string str2 = str1;
                                ch1 = Vincent._currentLine[0];
                                string str3 = ch1.ToString();
                                str1 = str2 + str3;
                            }
                            Vincent._currentLine = Vincent._currentLine.Remove(0, 1);
                            string val = str1 + "@";
                            Vincent._lineProgress[0].Add(val);
                            Vincent._waitLetter = 3f;
                            if (Vincent._currentLine.Length == 0)
                            {
                                Vincent._currentLine = "";
                                return;
                            }
                        }
                        float num4 = 0.0f;
                        while (Vincent._currentLine[0] == '|')
                        {
                            Vincent._currentLine = Vincent._currentLine.Remove(0, 1);
                            string str4 = "";
                            for (; Vincent._currentLine[0] != '|' && Vincent._currentLine.Length > 0; Vincent._currentLine = Vincent._currentLine.Remove(0, 1))
                            {
                                string str5 = str4;
                                ch1 = Vincent._currentLine[0];
                                string str6 = ch1.ToString();
                                str4 = str5 + str6;
                            }
                            bool flag5 = false;
                            if (Vincent._currentLine.Length <= 1)
                            {
                                Vincent._currentLine = "";
                                flag5 = true;
                            }
                            else
                                Vincent._currentLine = Vincent._currentLine.Remove(0, 1);
                            Color c = Color.White;
                            bool flag6 = false;
                            if (str4 == "RED")
                            {
                                flag6 = true;
                                c = Color.Red;
                            }
                            else if (str4 == "WHITE")
                            {
                                flag6 = true;
                                c = Color.White;
                            }
                            else if (str4 == "BLUE")
                            {
                                flag6 = true;
                                c = Color.Blue;
                            }
                            else if (str4 == "ORANGE")
                            {
                                flag6 = true;
                                c = new Color(235, 137, 51);
                            }
                            else if (str4 == "YELLOW")
                            {
                                flag6 = true;
                                c = new Color(247, 224, 90);
                            }
                            else if (str4 == "GREEN")
                            {
                                flag6 = true;
                                c = Color.LimeGreen;
                            }
                            else if (str4 == "CONCERNED")
                            {
                                Vincent._mood = DealerMood.Concerned;
                                num4 = 2f;
                            }
                            else if (str4 == "POINT")
                            {
                                Vincent._mood = DealerMood.Point;
                                num4 = 2f;
                            }
                            else if (str4 == "CALM")
                            {
                                Vincent._mood = DealerMood.Normal;
                                num4 = 2f;
                            }
                            else if (str4 == "SHOW")
                                Vincent.show = true;
                            else if (str4 == "GIVE")
                            {
                                if (Vincent._willGiveYoYo)
                                {
                                    Vincent._willGiveYoYo = false;
                                    FurniShopScreen.giveYoYo = true;
                                }
                                else if (Vincent._willGiveVooDoo)
                                {
                                    Vincent._willGiveVooDoo = false;
                                    FurniShopScreen.giveVooDoo = true;
                                }
                                else if (Vincent._willGivePerimeterDefence)
                                {
                                    Vincent._willGivePerimeterDefence = false;
                                    FurniShopScreen.givePerimeterDefence = true;
                                }
                            }
                            else if (str4 == "0")
                                Vincent.killSkip = true;
                            else if (str4 == "1")
                                num4 = 5f;
                            else if (str4 == "2")
                                num4 = 10f;
                            else if (str4 == "3")
                                num4 = 15f;
                            if (flag6)
                            {
                                if (Vincent._lineProgress.Count == 0)
                                    Vincent._lineProgress.Insert(0, new TextLine()
                                    {
                                        lineColor = c
                                    });
                                else
                                    Vincent._lineProgress[0].SwitchColor(c);
                            }
                            if (flag5)
                                return;
                        }
                        string str7 = "";
                        int index1 = 1;
                        if (Vincent._currentLine[0] == ' ')
                        {
                            while (index1 < Vincent._currentLine.Length && Vincent._currentLine[index1] != ' ' && Vincent._currentLine[index1] != '^')
                            {
                                if (Vincent._currentLine[index1] == '|')
                                {
                                    int index2 = index1 + 1;
                                    while (index2 < Vincent._currentLine.Length && Vincent._currentLine[index2] != '|')
                                        ++index2;
                                    index1 = index2 + 1;
                                }
                                else if (Vincent._currentLine[index1] == '@')
                                {
                                    int index3 = index1 + 1;
                                    while (index3 < Vincent._currentLine.Length && Vincent._currentLine[index3] != '@')
                                        ++index3;
                                    index1 = index3 + 1;
                                }
                                else
                                {
                                    string str8 = str7;
                                    ch1 = Vincent._currentLine[index1];
                                    string str9 = ch1.ToString();
                                    str7 = str8 + str9;
                                    ++index1;
                                }
                            }
                        }
                        if (Vincent._lineProgress.Count == 0 || Vincent._currentLine[0] == '^' || Vincent._currentLine[0] == ' ' && Vincent._lineProgress[0].Length() + str7.Length > 34)
                        {
                            Color color = Color.White;
                            if (Vincent._lineProgress.Count > 0)
                                color = Vincent._lineProgress[0].lineColor;
                            Vincent._lineProgress.Insert(0, new TextLine()
                            {
                                lineColor = color
                            });
                            if (Vincent._currentLine[0] == ' ' || Vincent._currentLine[0] == '^')
                                Vincent._currentLine = Vincent._currentLine.Remove(0, 1);
                        }
                        else
                        {
                            if (Vincent._currentLine[0] == '!' || Vincent._currentLine[0] == '?' || Vincent._currentLine[0] == '.')
                                Vincent._waitLetter = 5f;
                            else if (Vincent._currentLine[0] == ',')
                                Vincent._waitLetter = 3f;
                            Vincent._lineProgress[0].Add(Vincent._currentLine[0]);
                            ch1 = Vincent._currentLine[0];
                            char ch2 = ch1.ToString().ToLowerInvariant()[0];
                            if (Vincent.wait > 0)
                                --Vincent.wait;
                            if ((ch2 < 'a' || ch2 > 'z') && (ch2 < '0' || ch2 > '9') && ch2 != '\'' && Vincent.lastWord != "")
                            {
                                int num5 = (int)CRC32.Generate(Vincent.lastWord.Trim());
                                Vincent.lastWord = "";
                            }
                            else
                                Vincent.lastWord += ch2.ToString();
                            if (Vincent.wait > 0)
                            {
                                --Vincent.wait;
                            }
                            else
                            {
                                Vincent.wait = 2;
                                SFX.Play("tinyTick", 0.4f, 0.2f);
                            }
                            Vincent._currentLine = Vincent._currentLine.Remove(0, 1);
                        }
                        Vincent._waitLetter += num4;
                    }
                }
                else
                {
                    if (Vincent.show)
                    {
                        Vincent._afterShowWait += 0.12f;
                        if (_afterShowWait >= 1.0)
                            Vincent._allowMovement = true;
                    }
                    Vincent._talkMove += 0.75f;
                    if (_talkMove > 1.0)
                    {
                        Vincent.frame = 0;
                        Vincent._talkMove = 0.0f;
                    }
                }
                string str10 = "";
                for (int index = 0; index < Vincent.products.Count; ++index)
                {
                    string str11 = str10;
                    num1 = 9;
                    string str12 = num1.ToString();
                    str10 = str11 + str12;
                    Camera camera = new Camera(0.0f, 0.0f, 64f, 16f);
                    DuckGame.Graphics.SetRenderTarget(Vincent._priceTargets[index]);
                    DepthStencilState depthStencilState = new DepthStencilState()
                    {
                        StencilEnable = true,
                        StencilFunction = CompareFunction.Always,
                        StencilPass = StencilOperation.Replace,
                        ReferenceStencil = 1,
                        DepthBufferEnable = false
                    };
                    DuckGame.Graphics.Clear(new Color(0, 0, 0, 0));
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, null, camera.getMatrix());
                    num1 = Math.Min(Math.Max(Vincent.products[index].cost, 0), 9999);
                    string text = "$" + num1.ToString();
                    Vincent._furniTag.frame = text.Length - 1;
                    Vincent._priceFontRightways.Draw(text, new Vec2((float)((5 - text.Length) / 5.0 * 20.0), 0.0f), Vincent.products[index].cost > Profiles.experienceProfile.littleManBucks ? Colors.DGRed : Color.Black, (Depth)0.97f);
                    DuckGame.Graphics.screen.End();
                    DuckGame.Graphics.SetRenderTarget(null);
                }
            }
        }

        public static void Draw()
        {
            Vec2 vec2_1 = new Vec2((float)(_listLerp * 270.0 - 200.0), 20f);
            if (_challengeLerp < 0.00999999977648258 && _chancyLerp < 0.00999999977648258)
                return;
            Vec2 vec2_2 = new Vec2((float)(100.0 * (1.0 - _chancyLerp)), (float)(100.0 * (1.0 - _chancyLerp) - 4.0));
            Vec2 vec2_3 = new Vec2(280f, 30f);
            Vec2 vec2_4 = new Vec2(20f, 132f) + vec2_2;
            DuckGame.Graphics.DrawRect(vec2_4 + new Vec2(-2f, 0.0f), vec2_4 + vec2_3 + new Vec2(2f, 0.0f), Color.Black, (Depth)0.96f);
            int num = 0;
            for (int index1 = Vincent._lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float stringWidth = DuckGame.Graphics.GetStringWidth(Vincent._lineProgress[index1].text);
                float y = vec2_4.y + 2f + num * 9;
                float x = (float)(vec2_4.x + vec2_3.x / 2.0 - (double)stringWidth / 2.0);
                for (int index2 = Vincent._lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    Vincent._descriptionFont.Draw(Vincent._lineProgress[index1].segments[index2].text, new Vec2(x, y), Vincent._lineProgress[index1].segments[index2].color, (Depth)0.97f);
                    x += Vincent._lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num;
            }
            Vincent._tail.flipV = true;
            DuckGame.Graphics.Draw(Vincent._tail, 222f + vec2_2.x, 117f + vec2_2.y);
            if (Vincent.hasKid)
                Vincent._dealer.frame += 9;
            Vincent._dealer.depth = (Depth)0.96f;
            Vincent._dealer.alpha = Vincent.alpha;
            DuckGame.Graphics.Draw(_dealer, 200f + vec2_2.x, 26f + vec2_2.y);
            switch (Vincent.type)
            {
                case DayType.SaleDay:
                    Vincent._bigBanner.depth = (Depth)0.96f;
                    DuckGame.Graphics.Draw(Vincent._bigBanner, 22f, (float)(_showLerp * 100.0 - 80.0));
                    DuckGame.Graphics.Draw(Vincent._bigBanner, 194f, (float)(_showLerp * 100.0 - 80.0));
                    break;
                case DayType.ImportDay:
                    Vincent._fancyBanner.depth = (Depth)0.96f;
                    DuckGame.Graphics.Draw(Vincent._fancyBanner, 22f, (float)(_showLerp * 100.0 - 80.0));
                    DuckGame.Graphics.Draw(Vincent._fancyBanner, 194f, (float)(_showLerp * 100.0 - 80.0));
                    break;
            }
            Vincent._furniFrame.alpha = Vincent.alpha;
            Vincent._cheapTape.alpha = Vincent.alpha * 0.9f;
            Vincent._furniFill.alpha = Vincent.alpha;
            Vincent._furniHov.alpha = Vincent.alpha;
            Vincent._furniTag.alpha = Vincent.alpha;
            Vincent._newSticker.alpha = Vincent.alpha;
            Vincent._rareSticker.alpha = Vincent.alpha;
            Vincent._soldSprite.alpha = Vincent.alpha;
            Vec2 vec2_5 = new Vec2(84f, 46f);
            Vincent._cheapTape.depth = (Depth)0.968f;
            Vincent._furniFrame.depth = (Depth)0.96f;
            Vincent._furniFill.depth = (Depth)0.965f;
            Vincent._furniHov.depth = (Depth)0.965f;
            Vincent._furniTag.depth = (Depth)0.972f;
            Vincent._newSticker.depth = (Depth)0.972f;
            Vincent._rareSticker.depth = (Depth)0.972f;
            Vincent._soldSprite.depth = (Depth)0.975f;
            if (Vincent.products.Count > 0)
            {
                int index3 = 0;
                Vec2 pos = new Vec2(vec2_5.x - 200f + Math.Min(Vincent._showLerp * (200 + 40 * index3), 200f), vec2_5.y);
                if (Vincent.products.Count == 1)
                    pos = new Vec2(vec2_5.x - 200f + Math.Min(Vincent._showLerp * 275f, 240f), vec2_5.y + 30f);
                DuckGame.Graphics.Draw(Vincent._furniFrame, pos.x, pos.y);
                int val1_1 = Vincent.products[0].cost;
                bool flag1 = false;
                if (Vincent.products[0].cost != Vincent.products[0].originalCost)
                {
                    flag1 = true;
                    val1_1 = Vincent.products[0].originalCost;
                    DuckGame.Graphics.Draw(Vincent._priceTargets[0], new Vec2(pos.x - 13f, pos.y - 27f), new Rectangle?(), Color.White, 0.3f, Vec2.Zero, Vec2.One, SpriteEffects.None, (Depth)0.9685f);
                    DuckGame.Graphics.Draw(Vincent._cheapTape, pos.x, pos.y);
                }
                Vincent._furniFill.color = Vincent.products[index3].color;
                DuckGame.Graphics.Draw(Vincent._furniFill, pos.x, pos.y);
                Vincent.products[index3].Draw(pos, Vincent.alpha, 0.97f);
                if (index3 == Vincent._selectIndex)
                    DuckGame.Graphics.Draw(Vincent._furniHov, pos.x - 1f, pos.y);
                if (Vincent.products[index3].type == VPType.Furniture && Vincent.products[index3].furnitureData.rarity >= Rarity.SuperRare)
                {
                    Vincent._rareSticker.frame = index3 == Vincent._selectIndex ? 1 : 0;
                    DuckGame.Graphics.Draw(_rareSticker, pos.x - 23f, pos.y - 19f);
                }
                else if (Vincent.products[index3].type == VPType.Hat || !Vincent.products[index3].sold && Profiles.experienceProfile.GetNumFurnitures(products[index3].furnitureData.index) <= 0)
                {
                    Vincent._newSticker.frame = index3 == Vincent._selectIndex ? 1 : 0;
                    DuckGame.Graphics.Draw(_newSticker, pos.x - 23f, pos.y - 19f);
                }
                if (Vincent.products[index3].sold)
                {
                    DuckGame.Graphics.Draw(Vincent._soldSprite, pos.x, pos.y);
                }
                else
                {
                    string str = Math.Min(Math.Max(val1_1, 0), 9999).ToString();
                    Vincent._furniTag.frame = str.Length - 1;
                    DuckGame.Graphics.Draw(_furniTag, pos.x + 21f, pos.y - 25f);
                    string text = "$\n";
                    foreach (char ch in str)
                        text = text + ch.ToString() + "\n";
                    (!flag1 ? Vincent._priceFont : Vincent._priceFontCrossout).Draw(text, new Vec2(pos.x + 24f, pos.y - 16f), val1_1 > Profiles.experienceProfile.littleManBucks ? Colors.DGRed : (!flag1 ? Color.Black : Color.White), (Depth)(275f * (float)Math.PI / 887f));
                }
                if (Vincent.products.Count > 1)
                {
                    int index4 = 1;
                    pos = new Vec2((float)(vec2_5.x + 70.0 - 200.0) + Math.Min(Vincent._showLerp * (200 + 40 * index4), 200f), vec2_5.y);
                    DuckGame.Graphics.Draw(Vincent._furniFrame, pos.x, pos.y);
                    int val1_2 = Vincent.products[1].cost;
                    bool flag2 = false;
                    if (Vincent.products[1].cost != Vincent.products[1].originalCost)
                    {
                        flag2 = true;
                        val1_2 = Vincent.products[1].originalCost;
                        DuckGame.Graphics.Draw(Vincent._priceTargets[1], new Vec2(pos.x - 13f, pos.y - 27f), new Rectangle?(), Color.White, 0.3f, Vec2.Zero, Vec2.One, SpriteEffects.None, (Depth)0.9685f);
                        DuckGame.Graphics.Draw(Vincent._cheapTape, pos.x, pos.y);
                    }
                    Vincent._furniFill.color = Vincent.products[index4].color;
                    DuckGame.Graphics.Draw(Vincent._furniFill, pos.x, pos.y);
                    Vincent.products[index4].Draw(pos, Vincent.alpha, 0.97f);
                    if (index4 == Vincent._selectIndex)
                        DuckGame.Graphics.Draw(Vincent._furniHov, pos.x - 1f, pos.y);
                    if (Vincent.products[index4].type == VPType.Furniture && Vincent.products[index4].furnitureData.rarity >= Rarity.SuperRare)
                    {
                        Vincent._rareSticker.frame = index4 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_rareSticker, pos.x - 23f, pos.y - 19f);
                    }
                    else if (Profiles.experienceProfile.GetNumFurnitures(products[index4].furnitureData.index) <= 0)
                    {
                        Vincent._newSticker.frame = index4 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_newSticker, pos.x - 23f, pos.y - 19f);
                    }
                    if (Vincent.products[index4].sold)
                    {
                        DuckGame.Graphics.Draw(Vincent._soldSprite, pos.x, pos.y);
                    }
                    else
                    {
                        string str = Math.Min(Math.Max(val1_2, 0), 9999).ToString();
                        Vincent._furniTag.frame = str.Length - 1;
                        DuckGame.Graphics.Draw(_furniTag, pos.x + 21f, pos.y - 25f);
                        string text = "$\n";
                        foreach (char ch in str)
                            text = text + ch.ToString() + "\n";
                        (!flag2 ? Vincent._priceFont : Vincent._priceFontCrossout).Draw(text, new Vec2(pos.x + 24f, pos.y - 16f), val1_2 > Profiles.experienceProfile.littleManBucks ? Colors.DGRed : (!flag2 ? Color.Black : Color.White), (Depth)(275f * (float)Math.PI / 887f));
                    }
                }
                if (Vincent.products.Count > 2)
                {
                    int index5 = 2;
                    pos = new Vec2(vec2_5.x - 200f + Math.Min(Vincent._showLerp * (200 + 40 * index5), 200f), vec2_5.y + 54f);
                    DuckGame.Graphics.Draw(Vincent._furniFrame, pos.x, pos.y);
                    int val1_3 = Vincent.products[2].cost;
                    bool flag3 = false;
                    if (Vincent.products[2].cost != Vincent.products[2].originalCost)
                    {
                        flag3 = true;
                        val1_3 = Vincent.products[2].originalCost;
                        DuckGame.Graphics.Draw(Vincent._priceTargets[2], new Vec2(pos.x - 13f, pos.y - 27f), new Rectangle?(), Color.White, 0.3f, Vec2.Zero, Vec2.One, SpriteEffects.None, (Depth)0.9685f);
                        DuckGame.Graphics.Draw(Vincent._cheapTape, pos.x, pos.y);
                    }
                    Vincent._furniFill.color = Vincent.products[index5].color;
                    DuckGame.Graphics.Draw(Vincent._furniFill, pos.x, pos.y);
                    Vincent.products[index5].Draw(pos, Vincent.alpha, 0.97f);
                    if (index5 == Vincent._selectIndex)
                        DuckGame.Graphics.Draw(Vincent._furniHov, pos.x - 1f, pos.y);
                    if (Vincent.products[index5].type == VPType.Furniture && Vincent.products[index5].furnitureData.rarity >= Rarity.SuperRare)
                    {
                        Vincent._rareSticker.frame = index5 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_rareSticker, pos.x - 23f, pos.y - 19f);
                    }
                    else if (Profiles.experienceProfile.GetNumFurnitures(products[index5].furnitureData.index) <= 0)
                    {
                        Vincent._newSticker.frame = index5 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_newSticker, pos.x - 23f, pos.y - 19f);
                    }
                    if (Vincent.products[index5].sold)
                    {
                        DuckGame.Graphics.Draw(Vincent._soldSprite, pos.x, pos.y);
                    }
                    else
                    {
                        string str = Math.Min(Math.Max(val1_3, 0), 9999).ToString();
                        Vincent._furniTag.frame = str.Length - 1;
                        DuckGame.Graphics.Draw(_furniTag, pos.x + 21f, pos.y - 25f);
                        string text = "$\n";
                        foreach (char ch in str)
                            text = text + ch.ToString() + "\n";
                        (!flag3 ? Vincent._priceFont : Vincent._priceFontCrossout).Draw(text, new Vec2(pos.x + 24f, pos.y - 16f), val1_3 > Profiles.experienceProfile.littleManBucks ? Colors.DGRed : (!flag3 ? Color.Black : Color.White), (Depth)(275f * (float)Math.PI / 887f));
                    }
                }
                if (Vincent.products.Count > 3)
                {
                    int index6 = 3;
                    pos = new Vec2((float)(vec2_5.x + 70.0 - 200.0) + Math.Min(Vincent._showLerp * (200 + 40 * index6), 200f), vec2_5.y + 54f);
                    DuckGame.Graphics.Draw(Vincent._furniFrame, pos.x, pos.y);
                    int val1_4 = Vincent.products[3].cost;
                    bool flag4 = false;
                    if (Vincent.products[3].cost != Vincent.products[3].originalCost)
                    {
                        flag4 = true;
                        val1_4 = Vincent.products[3].originalCost;
                        DuckGame.Graphics.Draw(Vincent._priceTargets[3], new Vec2(pos.x - 13f, pos.y - 27f), new Rectangle?(), Color.White, 0.3f, Vec2.Zero, Vec2.One, SpriteEffects.None, (Depth)0.9685f);
                        DuckGame.Graphics.Draw(Vincent._cheapTape, pos.x, pos.y);
                    }
                    Vincent._furniFill.color = Vincent.products[index6].color;
                    DuckGame.Graphics.Draw(Vincent._furniFill, pos.x, pos.y);
                    Vincent.products[index6].Draw(pos, Vincent.alpha, 0.97f);
                    if (index6 == Vincent._selectIndex)
                        DuckGame.Graphics.Draw(Vincent._furniHov, pos.x - 1f, pos.y);
                    if (Vincent.products[index6].type == VPType.Furniture && Vincent.products[index6].furnitureData.rarity >= Rarity.SuperRare)
                    {
                        Vincent._rareSticker.frame = index6 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_rareSticker, pos.x - 23f, pos.y - 19f);
                    }
                    else if (Profiles.experienceProfile.GetNumFurnitures(products[index6].furnitureData.index) <= 0)
                    {
                        Vincent._newSticker.frame = index6 == Vincent._selectIndex ? 1 : 0;
                        DuckGame.Graphics.Draw(_newSticker, pos.x - 23f, pos.y - 19f);
                    }
                    if (Vincent.products[index6].sold)
                    {
                        DuckGame.Graphics.Draw(Vincent._soldSprite, pos.x, pos.y);
                    }
                    else
                    {
                        string str = Math.Min(Math.Max(val1_4, 0), 9999).ToString();
                        Vincent._furniTag.frame = str.Length - 1;
                        DuckGame.Graphics.Draw(_furniTag, pos.x + 21f, pos.y - 25f);
                        string text = "$\n";
                        foreach (char ch in str)
                            text = text + ch.ToString() + "\n";
                        (!flag4 ? Vincent._priceFont : Vincent._priceFontCrossout).Draw(text, new Vec2(pos.x + 24f, pos.y - 16f), val1_4 > Profiles.experienceProfile.littleManBucks ? Colors.DGRed : (!flag4 ? Color.Black : Color.White), (Depth)(275f * (float)Math.PI / 887f));
                    }
                }
            }
            if (Vincent.show && Vincent.products.Count > 0)
            {
                int index = 0;
                if (Vincent._selectIndex >= 0)
                    index = Vincent._selectIndex;
                Vec2 p1 = new Vec2(20f, 6f);
                Vec2 vec2_6 = new Vec2(226f, 11f);
                DuckGame.Graphics.DrawRect(p1, p1 + vec2_6, Color.Black, (Depth)0.96f);
                string name = Vincent.products[index].name;
                DuckGame.Graphics.DrawString(name, p1 + new Vec2((float)(vec2_6.x / 2.0 - (double)DuckGame.Graphics.GetStringWidth(name) / 2.0), 2f), new Color(163, 206, 39) * Vincent.alpha, (Depth)0.97f);
                Vincent._tail.depth = (Depth)0.5f;
                Vincent._tail.alpha = Vincent.alpha;
                Vincent._tail.flipH = false;
                Vincent._tail.flipV = false;
                DuckGame.Graphics.Draw(Vincent._tail, 222f, 17f);
            }
            if (!Vincent.hasKid)
                return;
            Vincent._dealer.frame -= 9;
        }
    }
}
