using System;
using System.Collections.Generic;
using System.Linq;

using Emoji = Centvrio.Emoji;

namespace TownBuilderBot
{
    using TickFunctionType = Func<string, int, Program.Point, System.Random, string>;
    static class EmojiIndex
    {
        public enum Zone
        {
            None,
            Natural,
            Residential,
            Commercial,
            Tourism,
        }

        [System.Flags]
        public enum Flags
        {
            None = 0,
            Flammable = 1,
            SpawnDragon = 2,
            Water = 4,
            SpawnFlyingSaucer = 8,
        }

        public class EmojiData
        {
            public readonly string Emoji;
            public readonly string Name;
            public readonly Zone Zone;

            public readonly Flags Flags;

            public TickFunctionType TickFunction;

            public EmojiData(Emoji.UnicodeString InEmoji, string InName, Zone InZone, Flags InFlags, TickFunctionType InTickFunction = null)
            {
                Emoji = (InEmoji + Centvrio.Emoji.VariationSelectors.VS16).ToString();
                Name = InName;
                Zone = InZone;
                Flags = InFlags;
                TickFunction = InTickFunction;
            }

            public bool CheckFlag(Flags flag) {
                return (Flags & flag) != Flags.None;
            }
        }

        public static EmojiData[] All = new EmojiData[] {
            new(Emoji.PlaceBuilding.Bank, "Bank", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Building, "Classical Building", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.Castle, "Castle", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.ConvenienceStore, "Convenience Store", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.DepartmentStore, "Department Store", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.DerelictHouse, "Derelict House", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Factory, "Factory", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Hospital, "Hospital", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Hotel, "Hotel", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.House, "House", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Houses, "Houses", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.HouseWithGarden, "House With Garden", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.JapaneseCastle, "Castle", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.JapanesePostOffice, "Post Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.LoveHotel, "Love Hotel", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Office, "Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.PostOffice, "Post Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.School, "School", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Stadium, "Stadium", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.StatueOfLiberty, "Statue of Liberty", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.TokyoTower, "Tokyo Tower", Zone.Tourism, Flags.Flammable|Flags.SpawnFlyingSaucer, TickFlyingSaucerSpawner),

            new(Emoji.PlaceGeographic.BeachWithUmbrella, "Beach With Umbrella", Zone.Natural, Flags.Water),
            new(Emoji.PlaceGeographic.Camping, "Campsite", Zone.Natural, Flags.Flammable),
            new(Emoji.PlaceGeographic.Desert, "Desert", Zone.Natural, Flags.None),
            new(Emoji.PlaceGeographic.DesertIsland, "Desert Island", Zone.Natural, Flags.Water),
            new(Emoji.PlaceGeographic.Fuji, "Mount Fuji", Zone.Natural, Flags.SpawnDragon),
            new(Emoji.PlaceGeographic.Mountain, "Mountain", Zone.Commercial, Flags.SpawnDragon),
            new(Emoji.PlaceGeographic.NationalPark, "National Park", Zone.Natural, Flags.Flammable|Flags.Water),
            new(Emoji.PlaceGeographic.SnowCappedMountain, "Snow-capped Mountain", Zone.Natural, Flags.SpawnDragon),

            new(Emoji.PlaceOther.CircusTent, "Circus", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.FerrisWheel, "Ferris Wheel", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.Fountain, "Fountain", Zone.Commercial, Flags.Water),
            new(Emoji.PlaceOther.RollerCoaster, "Roller Coaster", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.Tent, "Tent", Zone.Residential, Flags.Flammable),
            new(Emoji.OtherObjects.Moai, "Moai", Zone.Tourism, Flags.Flammable),

            new(Emoji.Sport.FlagInHole, "Golf Course", Zone.Commercial, Flags.Flammable),

            new(Emoji.SkyAndWeather.WaterWave, "Water", Zone.Natural, Flags.Water),

            new(Emoji.Emotion.Hole, "Hole", Zone.Commercial, Flags.None),

            new(Emoji.Science.SatelliteAntenna, "Satellite Antenna", Zone.Commercial, Flags.Flammable|Flags.SpawnFlyingSaucer, TickFlyingSaucerSpawner),

            new(Emoji.PlantOther.DeciduousTree, "Deciduous Tree", Zone.Natural, Flags.Flammable),
            new(Emoji.PlantOther.EvergreenTree, "Evergreen Tree", Zone.Natural, Flags.Flammable),
            new(Emoji.PlantOther.PalmTree, "Palm Tree", Zone.Natural, Flags.Flammable),

            new(Emoji.SkyAndWeather.Fog, "Fog", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.PlaceGeographic.Desert)),
            new(Emoji.SkyAndWeather.Fire, "Fire", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.SkyAndWeather.Fog)),
            new(Emoji.PlaceGeographic.Volcano, "Volcano", Zone.Natural, Flags.SpawnDragon, TickVolcano),

            new(Emoji.TransportAir.FlyingSaucer, "Flying Saucer", Zone.None, Flags.None),

            new(Emoji.AnimalBird.Eagle, "Eagle", Zone.None, Flags.Flammable),
            new(Emoji.AnimalBird.Bird, "Bird", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.Eagle)),
            new(Emoji.AnimalBird.BabyChick, "Baby Chick", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.Bird)),
            new(Emoji.AnimalBird.HatchingChick, "Hatching Chick", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.BabyChick)),

            new(Emoji.AnimalMarine.Octopus, "Octopus", Zone.None, Flags.Flammable|Flags.Water),
            new(Emoji.AnimalMarine.Fish, "Fish", Zone.None, Flags.Flammable|Flags.Water, MakeReplaceTickFunction(Emoji.AnimalMarine.Octopus)),

            new(Emoji.AnimalMammal.Tiger, "Tiger", Zone.None, Flags.Flammable),
            new(Emoji.AnimalMammal.Cat, "Cat", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalMammal.Tiger)),

            new(Emoji.AnimalMammal.Rat, "Rat", Zone.None, Flags.Flammable),

            new(Emoji.AnimalReptile.Dragon, "Dragon", Zone.None, Flags.SpawnDragon),
            new(Emoji.AnimalReptile.Crocodile, "Crocodile", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.AnimalReptile.Dragon)),
            new(Emoji.AnimalReptile.Lizard, "Lizard", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.AnimalReptile.Crocodile)),
            new(Emoji.FoodPrepared.Egg, "Egg", Zone.Natural, Flags.None, TickEgg),

            new(Emoji.OtherSymbols.Question, "Question Mark", Zone.None, Flags.None),
        };

        public static IEnumerable<string> GetRandomPollOptions(System.Random rand)
        {
            List<EmojiData> datas = CollectionUtils.RandomFirstN(4, All.Where(e => e.Zone != Zone.None), rand);
            return datas.Select(d => d.Emoji + " " + d.Name);
        }

        private class VolcanoData {
            public Program.Point location;
            public string display;
            public EmojiData emojiData;
        };

        private static VolcanoData GetVolcanoData(string grid, int width, Program.Point location) {
            string display = Program.GetElement(grid, width, location);
            if (display == null) {
                return null;
            }

            EmojiData emojiData = GetData(display);
            if (emojiData == null) {
                return null;
            }

            return new VolcanoData(){ location = location, display = display, emojiData = emojiData };
        }

        public static string TickVolcano(string oldGrid, int width, Program.Point location, System.Random rand) {

            Program.Point[] neighborLocations = new Program.Point[] {
                new() { X = location.X, Y = location.Y + 1 },
                new() { X = location.X + 1, Y = location.Y },
                new() { X = location.X, Y = location.Y - 1 },
                new() { X = location.X - 1, Y = location.Y },
            };

            IEnumerable<VolcanoData> neighbors = neighborLocations.Select(l => GetVolcanoData(oldGrid, width, l)).Where(d => d != null);
            IEnumerable<VolcanoData> flammableNeighbors = neighbors.Where(d => d.emojiData.CheckFlag(Flags.Flammable));
            VolcanoData neighborToLight = flammableNeighbors.FirstOrDefault();

            /*
            waterwave
            desertisland
            BeachWithUmbrella
            desert
            mountain
            SnowCappedMountain
            fuji
            */

            if (neighborToLight == null) {
                return oldGrid;
            }

            return Program.ReplaceElement(oldGrid, width, neighborToLight.location, Emoji.SkyAndWeather.Fire);
        }

        private static string TickEgg(string oldGrid, int width, Program.Point location, System.Random rand) {
            Program.Point[] neighborLocations = new Program.Point[] {
                new() { X = location.X + 1, Y = location.Y + 1 },
                new() { X = location.X + 1, Y = location.Y },
                new() { X = location.X + 1, Y = location.Y - 1 },
                new() { X = location.X, Y = location.Y + 1 },
                new() { X = location.X, Y = location.Y - 1 },
                new() { X = location.X - 1, Y = location.Y + 1 },
                new() { X = location.X - 1, Y = location.Y },
                new() { X = location.X - 1, Y = location.Y - 1 },
            };

            IEnumerable<string> neighborElements = neighborLocations.Select(l => Program.GetElement(oldGrid, width, l)).Where(e => e != null);
            IEnumerable<EmojiData> neighbors = neighborElements.Select(e => GetData(e)).Where(d => d != null);
            int halfNeighbors = neighbors.Count() / 2;

            int numDragonNeighbors = neighbors.Where(d => d.CheckFlag(Flags.SpawnDragon)).Count();
            if (numDragonNeighbors >= halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalReptile.Lizard);
            }

            int numWaterNeighbors = neighbors.Where(d => d.CheckFlag(Flags.Water)).Count();
            if (numWaterNeighbors >= halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalMarine.Fish);
            }

            int numNaturalNeighbors = neighbors.Where(d => d.Zone == Zone.Natural).Count();
            if (numNaturalNeighbors > halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalBird.HatchingChick);
            }

            int numTourismNeighbors = neighbors.Where(d => d.Zone == Zone.Tourism).Count();
            if (numTourismNeighbors > halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalMammal.Cat);
            }

            return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalMammal.Rat);
        }

        private static string TickFlyingSaucerSpawner(string oldGrid, int width, Program.Point location, System.Random rand) {
            Program.Point[] neighborLocations = new Program.Point[] {
                new() { X = location.X + 1, Y = location.Y + 1 },
                new() { X = location.X + 1, Y = location.Y },
                new() { X = location.X + 1, Y = location.Y - 1 },
                new() { X = location.X, Y = location.Y + 1 },
                new() { X = location.X, Y = location.Y - 1 },
                new() { X = location.X - 1, Y = location.Y + 1 },
                new() { X = location.X - 1, Y = location.Y },
                new() { X = location.X - 1, Y = location.Y - 1 },
            };

            IEnumerable<string> neighborElements = neighborLocations.Select(l => Program.GetElement(oldGrid, width, l)).Where(e => e != null);
            IEnumerable<EmojiData> neighbors = neighborElements.Select(e => GetData(e)).Where(d => d != null);
            IEnumerable<EmojiData> flyingSaucerSpawners = neighbors.Where(d => d.CheckFlag(Flags.SpawnFlyingSaucer));
            if (flyingSaucerSpawners.Count() >= 2)
            {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.TransportAir.FlyingSaucer);
            }

            return oldGrid;
        }

        public static TickFunctionType MakeReplaceTickFunction(Emoji.UnicodeString newString) {
            return (string oldGrid, int width, Program.Point location, System.Random rand) => {
                return Program.ReplaceElement(oldGrid, width, location, newString);
            };
        }

        public static string GetZoneEmoji(Zone InZone) {
            switch (InZone) {
                case Zone.Commercial: return "🟡";
                case Zone.Residential: return "🟦";
                case Zone.Natural: return "💚";
                case Zone.Tourism: return "🔶";
                default: return "❌";
            }
        }

        public static EmojiData GetData(string emoji) {
            return All.Where(x => x.Emoji == emoji).First();
        }
    }
}