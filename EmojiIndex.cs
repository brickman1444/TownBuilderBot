using System.Collections.Generic;
using System.Linq;

using Emoji = Centvrio.Emoji;

namespace TownBuilderBot
{
    static class EmojiIndex
    {
        public class EmojiData
        {
            public readonly string Emoji;
            public readonly string Name;
            public bool IsRandomPollOption = true;

            public EmojiData(string InEmoji, string InName, bool InIsRandomPollOption = true)
            {
                Emoji = InEmoji;
                Name = InName;
                IsRandomPollOption = InIsRandomPollOption;
            }
        }

        public static EmojiData[] All = new EmojiData[] {
            new EmojiData(Emoji.PlaceBuilding.Bank.ToString(), "Bank"),
            new EmojiData(Emoji.PlaceBuilding.Building.ToString(), "Classical Building"),
            new EmojiData(Emoji.PlaceBuilding.Castle.ToString(), "Castle"),
            new EmojiData(Emoji.PlaceBuilding.ConvenienceStore.ToString(), "Convenience Store"),
            new EmojiData(Emoji.PlaceBuilding.DepartmentStore.ToString(), "Department Store"),
            new EmojiData(Emoji.PlaceBuilding.DerelictHouse.ToString(), "Derelict House"),
            new EmojiData(Emoji.PlaceBuilding.Factory.ToString(), "Factory"),
            new EmojiData(Emoji.PlaceBuilding.Hospital.ToString(), "Hospital"),
            new EmojiData(Emoji.PlaceBuilding.Hotel.ToString(), "Hotel"),
            new EmojiData(Emoji.PlaceBuilding.House.ToString(), "House"),
            new EmojiData(Emoji.PlaceBuilding.Houses.ToString(), "Houses"),
            new EmojiData(Emoji.PlaceBuilding.HouseWithGarden.ToString(), "House With Garden"),
            new EmojiData(Emoji.PlaceBuilding.JapaneseCastle.ToString(), "Castle"),
            new EmojiData(Emoji.PlaceBuilding.JapanesePostOffice.ToString(), "Post Office"),
            new EmojiData(Emoji.PlaceBuilding.LoveHotel.ToString(), "Love Hotel"),
            new EmojiData(Emoji.PlaceBuilding.Office.ToString(), "Office"),
            new EmojiData(Emoji.PlaceBuilding.PostOffice.ToString(), "Post Office"),
            new EmojiData(Emoji.PlaceBuilding.School.ToString(), "School"),
            new EmojiData(Emoji.PlaceBuilding.Stadium.ToString(), "Stadium"),
            new EmojiData(Emoji.PlaceBuilding.StatueOfLiberty.ToString(), "Statue of Liberty"),
            new EmojiData(Emoji.PlaceBuilding.TokyoTower.ToString(), "Tokyo Tower"),

            new EmojiData(Emoji.PlaceGeographic.BeachWithUmbrella.ToString(), "Beach With Umbrella"),
            new EmojiData(Emoji.PlaceGeographic.Camping.ToString(), "Campsite"),
            new EmojiData(Emoji.PlaceGeographic.Desert.ToString(), "Desert"),
            new EmojiData(Emoji.PlaceGeographic.DesertIsland.ToString(), "Desert Island"),
            new EmojiData(Emoji.PlaceGeographic.Fuji.ToString(), "Mount Fuji"),
            new EmojiData(Emoji.PlaceGeographic.Mountain.ToString(), "Mountain"),
            new EmojiData(Emoji.PlaceGeographic.NationalPark.ToString(), "National Park"),
            new EmojiData(Emoji.PlaceGeographic.SnowCappedMountain.ToString(), "Snow-capped Mountain"),
            new EmojiData(Emoji.PlaceGeographic.Volcano.ToString(), "Volcano"),

            new EmojiData(Emoji.PlaceOther.CircusTent.ToString(), "Circus"),
            new EmojiData(Emoji.PlaceOther.FerrisWheel.ToString(), "Ferris Wheel"),
            new EmojiData(Emoji.PlaceOther.Fountain.ToString(), "Fountain"),
            new EmojiData(Emoji.PlaceOther.RollerCoaster.ToString(), "Roller Coaster"),
            new EmojiData(Emoji.PlaceOther.Tent.ToString(), "Tent"),

            new EmojiData(Emoji.Sport.FlagInHole.ToString(), "Golf Course"),

            new EmojiData(Emoji.SkyAndWeather.WaterWave.ToString(), "Water"),

            new EmojiData(Emoji.Emotion.Hole.ToString(), "Hole"),

            new EmojiData(Emoji.Science.SatelliteAntenna.ToString(), "Satellite Antenna"),

            new EmojiData(Emoji.PlantOther.DeciduousTree.ToString(), "Deciduous Tree"),
            new EmojiData(Emoji.PlantOther.EvergreenTree.ToString(), "Evergreen Tree"),
            new EmojiData(Emoji.PlantOther.PalmTree.ToString(), "Palm Tree"),

            new EmojiData(Emoji.SkyAndWeather.Fire.ToString(), "Fire", false),
        };

        public static List<EmojiData> GetRandomPollOptions(System.Random rand)
        {
            return CollectionUtils.RandomFirstN(4, EmojiIndex.All.Where(e => e.IsRandomPollOption), rand);
        }

        public static string TickVolcano(string oldGrid, int width, Program.Point location, System.Random rand) {
            int choice = rand.Next(4);
            Program.Point fireLocation = new Program.Point{ X = location.X, Y = location.Y};
            switch (choice) {
                case 0:
                    fireLocation.X += 1;
                    break;
                case 1:
                    fireLocation.X -= 1;
                    break;
                case 2:
                    fireLocation.Y += 1;
                    break;
                default:
                    fireLocation.Y -= 1;
                    break;
            }

            return Program.ReplaceElement(oldGrid, width, fireLocation.X, fireLocation.Y, Emoji.SkyAndWeather.Fire.ToString());
        }
    }
}