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
        }

        public class EmojiData
        {
            public readonly string Emoji;
            public readonly string Name;
            public readonly Zone Zone;

            public readonly Flags Flags;

            public TickFunctionType TickFunction;

            public EmojiData(string InEmoji, string InName, Zone InZone, Flags InFlags, TickFunctionType InTickFunction = null)
            {
                Emoji = InEmoji;
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
            new(Emoji.PlaceBuilding.Bank.ToString(), "Bank", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Building.ToString(), "Classical Building", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.Castle.ToString(), "Castle", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.ConvenienceStore.ToString(), "Convenience Store", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.DepartmentStore.ToString(), "Department Store", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.DerelictHouse.ToString(), "Derelict House", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Factory.ToString(), "Factory", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Hospital.ToString(), "Hospital", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Hotel.ToString(), "Hotel", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.House.ToString(), "House", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Houses.ToString(), "Houses", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.HouseWithGarden.ToString(), "House With Garden", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.JapaneseCastle.ToString(), "Castle", Zone.Tourism, Flags.Flammable|Flags.SpawnDragon),
            new(Emoji.PlaceBuilding.JapanesePostOffice.ToString(), "Post Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.LoveHotel.ToString(), "Love Hotel", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.Office.ToString(), "Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.PostOffice.ToString(), "Post Office", Zone.Commercial, Flags.Flammable),
            new(Emoji.PlaceBuilding.School.ToString(), "School", Zone.Residential, Flags.Flammable),
            new(Emoji.PlaceBuilding.Stadium.ToString(), "Stadium", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.StatueOfLiberty.ToString(), "Statue of Liberty", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceBuilding.TokyoTower.ToString(), "Tokyo Tower", Zone.Tourism, Flags.Flammable),

            new(Emoji.PlaceGeographic.BeachWithUmbrella.ToString(), "Beach With Umbrella", Zone.Natural, Flags.Water),
            new(Emoji.PlaceGeographic.Camping.ToString(), "Campsite", Zone.Natural, Flags.Flammable),
            new("🏜️", "Desert", Zone.Natural, Flags.None),
            new(Emoji.PlaceGeographic.DesertIsland.ToString(), "Desert Island", Zone.Natural, Flags.Water),
            new(Emoji.PlaceGeographic.Fuji.ToString(), "Mount Fuji", Zone.Natural, Flags.SpawnDragon),
            new("⛰️", "Mountain", Zone.Commercial, Flags.SpawnDragon),
            new(Emoji.PlaceGeographic.NationalPark.ToString(), "National Park", Zone.Natural, Flags.Flammable|Flags.Water),
            new(Emoji.PlaceGeographic.SnowCappedMountain.ToString(), "Snow-capped Mountain", Zone.Natural, Flags.SpawnDragon),

            new(Emoji.PlaceOther.CircusTent.ToString(), "Circus", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.FerrisWheel.ToString(), "Ferris Wheel", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.Fountain.ToString(), "Fountain", Zone.Commercial, Flags.Water),
            new(Emoji.PlaceOther.RollerCoaster.ToString(), "Roller Coaster", Zone.Tourism, Flags.Flammable),
            new(Emoji.PlaceOther.Tent.ToString(), "Tent", Zone.Residential, Flags.Flammable),

            new(Emoji.Sport.FlagInHole.ToString(), "Golf Course", Zone.Commercial, Flags.Flammable),

            new(Emoji.SkyAndWeather.WaterWave.ToString(), "Water", Zone.Natural, Flags.Water),

            new(Emoji.Emotion.Hole.ToString(), "Hole", Zone.Commercial, Flags.None),

            new(Emoji.Science.SatelliteAntenna.ToString(), "Satellite Antenna", Zone.Commercial, Flags.Flammable),

            new(Emoji.PlantOther.DeciduousTree.ToString(), "Deciduous Tree", Zone.Natural, Flags.Flammable),
            new(Emoji.PlantOther.EvergreenTree.ToString(), "Evergreen Tree", Zone.Natural, Flags.Flammable),
            new(Emoji.PlantOther.PalmTree.ToString(), "Palm Tree", Zone.Natural, Flags.Flammable),

            new(Emoji.SkyAndWeather.Fog.ToString(), "Fog", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction("🏜️")),
            new(Emoji.SkyAndWeather.Fire.ToString(), "Fire", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.SkyAndWeather.Fog.ToString())),
            new(Emoji.PlaceGeographic.Volcano.ToString(), "Volcano", Zone.Natural, Flags.SpawnDragon, TickVolcano),

            new(Emoji.AnimalBird.Eagle.ToString(), "Eagle", Zone.None, Flags.Flammable),
            new(Emoji.AnimalBird.Bird.ToString(), "Bird", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.Eagle.ToString())),
            new(Emoji.AnimalBird.BabyChick.ToString(), "Baby Chick", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.Bird.ToString())),
            new(Emoji.AnimalBird.HatchingChick.ToString(), "Hatching Chick", Zone.None, Flags.Flammable, MakeReplaceTickFunction(Emoji.AnimalBird.BabyChick.ToString())),

            new(Emoji.AnimalMarine.Octopus.ToString(), "Octopus", Zone.None, Flags.Flammable|Flags.Water),
            new(Emoji.AnimalMarine.Fish.ToString(), "Fish", Zone.None, Flags.Flammable|Flags.Water, MakeReplaceTickFunction(Emoji.AnimalMarine.Octopus.ToString())),

            new(Emoji.AnimalReptile.Dragon.ToString(), "Dragon", Zone.None, Flags.SpawnDragon),
            new(Emoji.AnimalReptile.Crocodile.ToString(), "Crocodile", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.AnimalReptile.Dragon.ToString())),
            new(Emoji.AnimalReptile.Lizard.ToString(), "Lizard", Zone.None, Flags.SpawnDragon, MakeReplaceTickFunction(Emoji.AnimalReptile.Crocodile.ToString())),
            new(Emoji.FoodPrepared.Egg.ToString(), "Egg", Zone.Natural, Flags.None, TickEgg),

            new(Emoji.OtherSymbols.Question.ToString(), "Question Mark", Zone.None, Flags.None),
        };

        public static IEnumerable<string> GetRandomPollOptions(System.Random rand)
        {
            List<EmojiData> datas = CollectionUtils.RandomFirstN(4, EmojiIndex.All.Where(e => e.Zone != Zone.None), rand);
            return datas.Select(d => d.Emoji + " " + d.Name);
        }

        public static string TickVolcano(string oldGrid, int width, Program.Point location, System.Random rand) {
            int choice = rand.Next(4);
            Program.Point fireLocation = new() { X = location.X, Y = location.Y};
            switch (choice) {
                case 0:
                    fireLocation.X += -1;
                    fireLocation.Y += 0;
                    break;
                case 1:
                    fireLocation.X += 0;
                    fireLocation.Y += -1;
                    break;
                case 2:
                    fireLocation.X += 0;
                    fireLocation.Y += 1;
                    break;
                default:
                    fireLocation.X += 1;
                    fireLocation.Y += 0;
                    break;
            }

            string targetElement = Program.GetElement(oldGrid, width, fireLocation);
            if (targetElement == null) {
                return oldGrid;
            }

            EmojiData targetData = GetData(targetElement);
            if (!targetData.CheckFlag(Flags.Flammable))
            {
                return oldGrid;
            }

            return Program.ReplaceElement(oldGrid, width, fireLocation, Emoji.SkyAndWeather.Fire.ToString());
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
            int numDragonNeighbors = neighbors.Where(d => d.CheckFlag(Flags.SpawnDragon)).Count();
            int halfNeighbors = neighbors.Count() / 2;

            if (numDragonNeighbors > halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalReptile.Lizard.ToString());
            }

            int numWaterNeighbors = neighbors.Where(d => d.CheckFlag(Flags.Water)).Count();
            if (numWaterNeighbors > halfNeighbors) {
                return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalMarine.Fish.ToString());
            }

            return Program.ReplaceElement(oldGrid, width, location, Emoji.AnimalBird.HatchingChick.ToString());
        }

        public static TickFunctionType MakeReplaceTickFunction(string newString) {
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