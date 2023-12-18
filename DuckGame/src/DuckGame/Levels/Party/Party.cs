using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Party
    {
        private static Dictionary<Profile, int> _drinks = new Dictionary<Profile, int>();
        private static Dictionary<Profile, List<PartyPerks>> _perks = new Dictionary<Profile, List<PartyPerks>>();

        public static void AddDrink(Profile p, int num)
        {
            if (!_drinks.ContainsKey(p))
                _drinks[p] = 0;
            _drinks[p] += num;
        }

        public static void AddPerk(Profile p, PartyPerks perk)
        {
            if (!_perks.ContainsKey(p))
                _perks[p] = new List<PartyPerks>();
            if (_perks[p].Contains(perk))
                return;
            _perks[p].Add(perk);
        }

        public static bool HasPerk(Profile p, PartyPerks perk) => TeamSelect2.partyMode && _perks.ContainsKey(p) && _perks[p].Contains(perk);

        public static void AddRandomPerk(Profile p)
        {
            IEnumerable<PartyPerks> source = Enum.GetValues(typeof(PartyPerks)).Cast<PartyPerks>();
            AddPerk(p, source.ElementAt(Rando.Int(source.Count() - 1)));
        }

        public static int GetDrinks(Profile p) => _drinks.ContainsKey(p) ? _drinks[p] : 0;

        public static List<PartyPerks> GetPerks(Profile p) => _perks.ContainsKey(p) ? _perks[p] : new List<PartyPerks>();

        public static void Clear()
        {
            List<Profile> profileList = new List<Profile>();
            foreach (KeyValuePair<Profile, int> drink in _drinks)
                profileList.Add(drink.Key);
            foreach (Profile key in profileList)
                _drinks[key] = 0;
            profileList.Clear();
            foreach (KeyValuePair<Profile, List<PartyPerks>> perk in _perks)
                profileList.Add(perk.Key);
            foreach (Profile key in profileList)
                _perks[key].Clear();
        }
    }
}
