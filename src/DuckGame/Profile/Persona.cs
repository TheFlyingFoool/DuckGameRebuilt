// Decompiled with JetBrains decompiler
// Type: DuckGame.Persona
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class Persona
    {
        private static List<DuckPersona> _personasOriginalOrder = new List<DuckPersona>()
    {
      new DuckPersona(new Vec3((float) byte.MaxValue, (float) byte.MaxValue, (float) byte.MaxValue))
      {
        index = 0
      },
      new DuckPersona(new Vec3(125f, 125f, 125f)) { index = 1 },
      new DuckPersona(new Vec3(247f, 224f, 90f)) { index = 2 },
      new DuckPersona(new Vec3(205f, 107f, 29f)) { index = 3 },
      new DuckPersona(new Vec3(0.0f, 133f, 74f), new Vec3(0.0f, 102f, 57f), new Vec3(0.0f, 173f, 97f))
      {
        index = 4
      },
      new DuckPersona(new Vec3((float) byte.MaxValue, 105f, 117f), new Vec3(207f, 84f, 94f), new Vec3((float) byte.MaxValue, 158f, 166f))
      {
        index = 5
      },
      new DuckPersona(new Vec3(49f, 162f, 242f), new Vec3(13f, 123f, 181f), new Vec3(148f, 207f, 245f))
      {
        index = 6
      },
      new DuckPersona(new Vec3(175f, 85f, 221f), new Vec3(141f, 36f, 194f), new Vec3(213f, 165f, 238f))
      {
        index = 7
      }
    };
        private static List<DuckPersona> _personasShuffled;
        public static int seed;

        private static List<DuckPersona> _personas
        {
            get
            {
                if (Persona._personasShuffled == null)
                    Persona.Shuffle();
                return Persona._personasShuffled;
            }
        }

        public static DuckPersona Duck1 => Persona._personas[0];

        public static DuckPersona Duck2 => Persona._personas[1];

        public static DuckPersona Duck3 => Persona._personas[2];

        public static DuckPersona Duck4 => Persona._personas[3];

        public static DuckPersona Duck5 => Persona._personas[4];

        public static DuckPersona Duck6 => Persona._personas[5];

        public static DuckPersona Duck7 => Persona._personas[6];

        public static DuckPersona Duck8 => Persona._personas[7];

        public static IEnumerable<DuckPersona> all => (IEnumerable<DuckPersona>)Persona._personas;

        public static void Initialize()
        {
        }

        public static void Shuffle(int pSeed = -1)
        {
            Persona.seed = pSeed >= 0 ? pSeed : Rando.Int(2147483646);
            Random generator = Rando.generator;
            Rando.generator = new Random(Persona.seed);
            Persona._personasShuffled = Persona._personasOriginalOrder.ToList<DuckPersona>();
            Rando.generator = generator;
        }

        public static int Number(DuckPersona p) => Persona._personas.IndexOf(p);
    }
}
