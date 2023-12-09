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

        public class EmojiData
        {
            public readonly string Emoji;
            public readonly string Name;
            public Zone Zone;

            public TickFunctionType TickFunction;

            public EmojiData(string InEmoji, string InName, Zone InZone, TickFunctionType InTickFunction = null)
            {
                Emoji = InEmoji;
                Name = InName;
                Zone = InZone;
                TickFunction = InTickFunction;
            }
        }

        public static EmojiData[] All = new EmojiData[] {
            new EmojiData(Emoji.PlaceBuilding.Bank.ToString(), "Bank", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.Building.ToString(), "Classical Building", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.Castle.ToString(), "Castle", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.ConvenienceStore.ToString(), "Convenience Store", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.DepartmentStore.ToString(), "Department Store", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.DerelictHouse.ToString(), "Derelict House", Zone.Residential),
            new EmojiData(Emoji.PlaceBuilding.Factory.ToString(), "Factory", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.Hospital.ToString(), "Hospital", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.Hotel.ToString(), "Hotel", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.House.ToString(), "House", Zone.Residential),
            new EmojiData(Emoji.PlaceBuilding.Houses.ToString(), "Houses", Zone.Residential),
            new EmojiData(Emoji.PlaceBuilding.HouseWithGarden.ToString(), "House With Garden", Zone.Residential),
            new EmojiData(Emoji.PlaceBuilding.JapaneseCastle.ToString(), "Castle", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.JapanesePostOffice.ToString(), "Post Office", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.LoveHotel.ToString(), "Love Hotel", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.Office.ToString(), "Office", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.PostOffice.ToString(), "Post Office", Zone.Commercial),
            new EmojiData(Emoji.PlaceBuilding.School.ToString(), "School", Zone.Residential),
            new EmojiData(Emoji.PlaceBuilding.Stadium.ToString(), "Stadium", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.StatueOfLiberty.ToString(), "Statue of Liberty", Zone.Tourism),
            new EmojiData(Emoji.PlaceBuilding.TokyoTower.ToString(), "Tokyo Tower", Zone.Tourism),

            new EmojiData(Emoji.PlaceGeographic.BeachWithUmbrella.ToString(), "Beach With Umbrella", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.Camping.ToString(), "Campsite", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.Desert.ToString(), "Desert", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.DesertIsland.ToString(), "Desert Island", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.Fuji.ToString(), "Mount Fuji", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.Mountain.ToString(), "Mountain", Zone.Commercial),
            new EmojiData(Emoji.PlaceGeographic.NationalPark.ToString(), "National Park", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.SnowCappedMountain.ToString(), "Snow-capped Mountain", Zone.Natural),
            new EmojiData(Emoji.PlaceGeographic.Volcano.ToString(), "Volcano", Zone.Natural, EmojiIndex.TickVolcano),

            new EmojiData(Emoji.PlaceOther.CircusTent.ToString(), "Circus", Zone.Tourism),
            new EmojiData(Emoji.PlaceOther.FerrisWheel.ToString(), "Ferris Wheel", Zone.Tourism),
            new EmojiData(Emoji.PlaceOther.Fountain.ToString(), "Fountain", Zone.Commercial),
            new EmojiData(Emoji.PlaceOther.RollerCoaster.ToString(), "Roller Coaster", Zone.Tourism),
            new EmojiData(Emoji.PlaceOther.Tent.ToString(), "Tent", Zone.Residential),

            new EmojiData(Emoji.Sport.FlagInHole.ToString(), "Golf Course", Zone.Commercial),

            new EmojiData(Emoji.SkyAndWeather.WaterWave.ToString(), "Water", Zone.Natural),

            new EmojiData(Emoji.Emotion.Hole.ToString(), "Hole", Zone.Commercial),

            new EmojiData(Emoji.Science.SatelliteAntenna.ToString(), "Satellite Antenna", Zone.Commercial),

            new EmojiData(Emoji.PlantOther.DeciduousTree.ToString(), "Deciduous Tree", Zone.Natural),
            new EmojiData(Emoji.PlantOther.EvergreenTree.ToString(), "Evergreen Tree", Zone.Natural),
            new EmojiData(Emoji.PlantOther.PalmTree.ToString(), "Palm Tree", Zone.Natural),

            new EmojiData(Emoji.SkyAndWeather.Fire.ToString(), "Fire", Zone.None),
        };

        public static List<EmojiData> GetRandomPollOptions(System.Random rand)
        {
            return CollectionUtils.RandomFirstN(4, EmojiIndex.All.Where(e => e.Zone != Zone.None), rand);
        }

        public static string TickVolcano(string oldGrid, int width, Program.Point location, System.Random rand) {
            int choice = rand.Next(8);
            Program.Point fireLocation = new Program.Point{ X = location.X, Y = location.Y};
            switch (choice) {
                case 0:
                    fireLocation.X += -1;
                    fireLocation.Y += -1;
                    break;
                case 1:
                    fireLocation.X += -1;
                    fireLocation.Y += 0;
                    break;
                case 2:
                    fireLocation.X += -1;
                    fireLocation.Y += 1;
                    break;
                case 3:
                    fireLocation.X += 0;
                    fireLocation.Y += -1;
                    break;
                case 4:
                    fireLocation.X += 0;
                    fireLocation.Y += 1;
                    break;
                case 5:
                    fireLocation.X += 1;
                    fireLocation.Y += -1;
                    break;
                case 6:
                    fireLocation.X += 1;
                    fireLocation.Y += 0;
                    break;
                default:
                    fireLocation.X += 1;
                    fireLocation.Y += 1;
                    break;
            }

            return Program.ReplaceElement(oldGrid, width, fireLocation.X, fireLocation.Y, Emoji.SkyAndWeather.Fire.ToString());
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