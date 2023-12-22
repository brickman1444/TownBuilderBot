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
        }

        public static EmojiData[] All = new EmojiData[] {
            new EmojiData(Emoji.PlaceBuilding.Bank.ToString(), "Bank", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Building.ToString(), "Classical Building", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Castle.ToString(), "Castle", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.ConvenienceStore.ToString(), "Convenience Store", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.DepartmentStore.ToString(), "Department Store", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.DerelictHouse.ToString(), "Derelict House", Zone.Residential, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Factory.ToString(), "Factory", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Hospital.ToString(), "Hospital", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Hotel.ToString(), "Hotel", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.House.ToString(), "House", Zone.Residential, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Houses.ToString(), "Houses", Zone.Residential, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.HouseWithGarden.ToString(), "House With Garden", Zone.Residential, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.JapaneseCastle.ToString(), "Castle", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.JapanesePostOffice.ToString(), "Post Office", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.LoveHotel.ToString(), "Love Hotel", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Office.ToString(), "Office", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.PostOffice.ToString(), "Post Office", Zone.Commercial, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.School.ToString(), "School", Zone.Residential, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.Stadium.ToString(), "Stadium", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.StatueOfLiberty.ToString(), "Statue of Liberty", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceBuilding.TokyoTower.ToString(), "Tokyo Tower", Zone.Tourism, Flags.Flammable),

            new EmojiData(Emoji.PlaceGeographic.BeachWithUmbrella.ToString(), "Beach With Umbrella", Zone.Natural, Flags.None),
            new EmojiData(Emoji.PlaceGeographic.Camping.ToString(), "Campsite", Zone.Natural, Flags.Flammable),
            new EmojiData("🏜️", "Desert", Zone.Natural, Flags.None),
            new EmojiData(Emoji.PlaceGeographic.DesertIsland.ToString(), "Desert Island", Zone.Natural, Flags.None),
            new EmojiData(Emoji.PlaceGeographic.Fuji.ToString(), "Mount Fuji", Zone.Natural, Flags.None),
            new EmojiData(Emoji.PlaceGeographic.Mountain.ToString(), "Mountain", Zone.Commercial, Flags.None),
            new EmojiData(Emoji.PlaceGeographic.NationalPark.ToString(), "National Park", Zone.Natural, Flags.Flammable),
            new EmojiData(Emoji.PlaceGeographic.SnowCappedMountain.ToString(), "Snow-capped Mountain", Zone.Natural, Flags.None),

            new EmojiData(Emoji.PlaceOther.CircusTent.ToString(), "Circus", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceOther.FerrisWheel.ToString(), "Ferris Wheel", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceOther.Fountain.ToString(), "Fountain", Zone.Commercial, Flags.None),
            new EmojiData(Emoji.PlaceOther.RollerCoaster.ToString(), "Roller Coaster", Zone.Tourism, Flags.Flammable),
            new EmojiData(Emoji.PlaceOther.Tent.ToString(), "Tent", Zone.Residential, Flags.Flammable),

            new EmojiData(Emoji.Sport.FlagInHole.ToString(), "Golf Course", Zone.Commercial, Flags.Flammable),

            new EmojiData(Emoji.SkyAndWeather.WaterWave.ToString(), "Water", Zone.Natural, Flags.None),

            new EmojiData(Emoji.Emotion.Hole.ToString(), "Hole", Zone.Commercial, Flags.None),

            new EmojiData(Emoji.Science.SatelliteAntenna.ToString(), "Satellite Antenna", Zone.Commercial, Flags.Flammable),

            new EmojiData(Emoji.PlantOther.DeciduousTree.ToString(), "Deciduous Tree", Zone.Natural, Flags.Flammable),
            new EmojiData(Emoji.PlantOther.EvergreenTree.ToString(), "Evergreen Tree", Zone.Natural, Flags.Flammable),
            new EmojiData(Emoji.PlantOther.PalmTree.ToString(), "Palm Tree", Zone.Natural, Flags.Flammable),

            new EmojiData(Emoji.SkyAndWeather.Fog.ToString(), "Fog", Zone.None, Flags.None, MakeReplaceTickFunction("🏜️")),
            new EmojiData(Emoji.SkyAndWeather.Fire.ToString(), "Fire", Zone.None, Flags.None, MakeReplaceTickFunction(Emoji.SkyAndWeather.Fog.ToString())),
            new EmojiData(Emoji.PlaceGeographic.Volcano.ToString(), "Volcano", Zone.Natural, Flags.None, TickVolcano),

            new EmojiData(Emoji.AnimalReptile.Dragon.ToString(), "Dragon", Zone.None, Flags.None),
            new EmojiData(Emoji.AnimalReptile.Crocodile.ToString(), "Crocodile", Zone.None, Flags.None, MakeReplaceTickFunction(Emoji.AnimalReptile.Dragon.ToString())),
            new EmojiData(Emoji.AnimalReptile.Lizard.ToString(), "Lizard", Zone.None, Flags.None, MakeReplaceTickFunction(Emoji.AnimalReptile.Crocodile.ToString())),
            new EmojiData(Emoji.FoodPrepared.Egg.ToString(), "Egg", Zone.Natural, Flags.None, MakeReplaceTickFunction(Emoji.AnimalReptile.Lizard.ToString())),

            new EmojiData(Emoji.OtherSymbols.Question.ToString(), "Question Mark", Zone.None, Flags.None),
        };

        public static IEnumerable<string> GetRandomPollOptions(System.Random rand)
        {
            List<EmojiData> datas = CollectionUtils.RandomFirstN(4, EmojiIndex.All.Where(e => e.Zone != Zone.None), rand);
            return datas.Select(d => d.Emoji + " " + d.Name);
        }

        public static string TickVolcano(string oldGrid, int width, Program.Point location, System.Random rand) {
            int choice = rand.Next(4);
            Program.Point fireLocation = new Program.Point{ X = location.X, Y = location.Y};
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
            if ((targetData.Flags & Flags.Flammable) == Flags.None)
            {
                return oldGrid;
            }

            return Program.ReplaceElement(oldGrid, width, fireLocation.X, fireLocation.Y, Emoji.SkyAndWeather.Fire.ToString());
        }

        public static TickFunctionType MakeReplaceTickFunction(string newString) {
            return (string oldGrid, int width, Program.Point location, System.Random rand) => {
                return Program.ReplaceElement(oldGrid, width, location.X, location.Y, newString);
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