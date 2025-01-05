using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace TownBuilderBot
{
    public class Tests
    {
        private readonly ITestOutputHelper output;
        public Tests(ITestOutputHelper outputHelper)
        {
            output = outputHelper;
        }

        private void StringEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual))
            {
                System.Globalization.StringInfo expectedInfo = new System.Globalization.StringInfo(expected);
                System.Globalization.StringInfo actualInfo = new System.Globalization.StringInfo(actual);

                output.WriteLine("  Index Expected  Actual");
                output.WriteLine("-------------------------");
                int maxLen = Math.Max(actualInfo.LengthInTextElements, expectedInfo.LengthInTextElements);
                int minLen = Math.Min(actualInfo.LengthInTextElements, expectedInfo.LengthInTextElements);
                for (int i = 0; i < maxLen; i++)
                {
                    output.WriteLine("{0} {1,-3} {2,-4} {3,-3}  {4,-4} {5,-3}",
                        i < minLen && expectedInfo.SubstringByTextElements(i, 1) == actualInfo.SubstringByTextElements(i, 1) ? " " : "*", // put a mark beside a differing row
                        i, // the index
                        i < expectedInfo.LengthInTextElements ? PrintableCharacterArray(expectedInfo.SubstringByTextElements(i, 1)) : "", // character decimal value
                        i < expectedInfo.LengthInTextElements ? ToLiteral(expectedInfo.SubstringByTextElements(i, 1)) : "", // character safe string
                        i < actualInfo.LengthInTextElements ? PrintableCharacterArray(actualInfo.SubstringByTextElements(i, 1)) : "", // character decimal value
                        i < actualInfo.LengthInTextElements ? ToLiteral(actualInfo.SubstringByTextElements(i, 1)) : "" // character safe string
                    );
                }
            }

            Assert.Equal(expected, actual);
        }

        private static string ToLiteral(string value)
        {
            return Regex.Escape(value);
        }

        private static string PrintableCharacterArray(string value)
        {
            char[] chars = value.ToCharArray();
            IEnumerable<string> strings = value.ToCharArray().Select(c => string.Format("{0:X}", (int)c));
            return string.Join(", ", strings);
        }

        [Fact]
        public static void ReplaceElement_OnlyChangesTargetElement()
        {
            string grid = "ABCD\n"
                        + "EFGH\n"
                        + "IJKL\n"
                        + "MNOP";

            string result = Program.ReplaceElement(grid, 4, 1, 2, "X");

            string expectedResult = "ABCD\n"
                                  + "EFGH\n"
                                  + "IXKL\n"
                                  + "MNOP";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void ReplaceElement_WorksInBottomRightCorner()
        {
            string grid = "ABCD\n"
                        + "EFGH\n"
                        + "IJKL\n"
                        + "MNOP";

            string result = Program.ReplaceElement(grid, 4, 3, 3, "X");

            string expectedResult = "ABCD\n"
                                  + "EFGH\n"
                                  + "IJKL\n"
                                  + "MNOX";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void ReplaceElement_WorksWithEmoji()
        {
            string grid = "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔";

            string result = Program.ReplaceElement(grid, 4, 1, 2, "X");

            string expectedResult = "🏔🏔🏔🏔\n"
                                  + "🏔🏔🏔🏔\n"
                                  + "🏔X🏔🏔\n"
                                  + "🏔🏔🏔🏔";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void GetPollWinningElement_PicksObviousWinner()
        {
            Mastonet.Entities.Poll poll = new Mastonet.Entities.Poll() {
                Options = new Mastonet.Entities.PollOption[] {
                    new Mastonet.Entities.PollOption() {
                        Title = "A",
                        VotesCount = 10,
                    },
                    new Mastonet.Entities.PollOption() {
                        Title = "B",
                        VotesCount = 1,
                    },
                    new Mastonet.Entities.PollOption() {
                        Title = "C",
                        VotesCount = 1,
                    },
                }
            };

            string winner = Program.GetPollWinningElement(poll, new System.Random());
            Assert.Equal("A", winner);
        }

        [Fact]
        public static void GetPollWinningElement_RandomlyBreaksTies()
        {
            Mastonet.Entities.Poll poll = new Mastonet.Entities.Poll() {
                Options = new Mastonet.Entities.PollOption[] {
                    new Mastonet.Entities.PollOption() {
                        Title = "A",
                        VotesCount = 1,
                    },
                    new Mastonet.Entities.PollOption() {
                        Title = "B",
                        VotesCount = 10,
                    },
                    new Mastonet.Entities.PollOption() {
                        Title = "C",
                        VotesCount = 10,
                    },
                    new Mastonet.Entities.PollOption() {
                        Title = "D",
                        VotesCount = 1,
                    },
                }
            };

            Dictionary<string, int> wins = new Dictionary<string, int>(){
                { "A", 0 },
                { "B", 0 },
                { "C", 0 },
                { "D", 0 },
            };

            for (int i = 0; i < 1000; ++i)
            {
                System.Random rand = new System.Random();
                string winner = Program.GetPollWinningElement(poll, rand);
                wins[winner]++;
            }

            Assert.Equal(0, wins["A"]);
            Assert.InRange(wins["B"], 400, 600);
            Assert.InRange(wins["C"], 400, 600);
            Assert.Equal(0, wins["D"]);
        }

        [Fact]
        public static void GetPollWinningElement_WorksWhenPollOptionsAreMoreThanJustAnEmoji()
        {
            Mastonet.Entities.Poll poll = new Mastonet.Entities.Poll() {
                Options = new Mastonet.Entities.PollOption[] {
                    new Mastonet.Entities.PollOption() {
                        Title = "🐵 Monkey",
                        VotesCount = 1,
                    },
                }
            };

            System.Random rand = new System.Random();

            string resultGrid = Program.GetPollWinningElement(poll, rand);

            Assert.Equal("🐵", resultGrid);
        }

        [Fact]
        public static void MakeNormalPost_HandlesPostWithoutHashTags()
        {
            Mastonet.Entities.Status status = new Mastonet.Entities.Status() {
                Content = "❓🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔",
                Poll = new Mastonet.Entities.Poll() {
                    Options = [
                        new Mastonet.Entities.PollOption() {
                            Title = "A",
                            VotesCount = 10,
                        },
                        new Mastonet.Entities.PollOption() {
                            Title = "B",
                            VotesCount = 1,
                        },
                    ]
                }
            };

            Random rand = new Random(0);
            Program.Post post = Program.MakeNormalPost(status, 4, rand);

            string expected = "A🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️❓\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "#hachybots #bot";

            Assert.Equal(expected, post.body);
        }

        [Fact]
        public void MakeNormalPost_HandlesPostWithHashTags()
        {
            Mastonet.Entities.Status status = new Mastonet.Entities.Status() {
                Content = "❓🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "#hachybots #bot",
                Poll = new Mastonet.Entities.Poll() {
                    Options = [
                        new Mastonet.Entities.PollOption() {
                            Title = "A",
                            VotesCount = 10,
                        },
                        new Mastonet.Entities.PollOption() {
                            Title = "B",
                            VotesCount = 1,
                        },
                    ]
                }
            };

            Random rand = new Random(0);
            Program.Post post = Program.MakeNormalPost(status, 4, rand);

            string expected = "A🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️❓\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "#hachybots #bot";

            StringEqual(expected, post.body);
        }

        [Fact]
        public void RemoveHashTags_HandlesPostWithHashtags()
        {
            string input = "❓🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "#hashtag";
            string expected = "❓🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️";
            Assert.Equal(expected, Program.RemoveHashTags(input));
        }

        [Fact]
        public void RemoveHashTags_HandlesPostWithoutHashtags()
        {
            string input = "❓🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️";
            string expected = "❓🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️\n"
                        + "🏔️🏔️🏔️🏔️";
            Assert.Equal(expected, Program.RemoveHashTags(input));
        }

        [Fact]
        public static void GetGridCoordinates_FindsStartCoordinate()
        {
            string targetElement = "X";

            string grid = "X🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔";

            int width = 4;

            Program.Point location = Program.GetGridCoordinates(grid, width, targetElement);

            Assert.Equal(0, location.Y);
            Assert.Equal(0, location.X);
        }

        [Fact]
        public static void GetGridCoordinates_FindsEndCoordinate()
        {
            string targetElement = "X";

            string grid = "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔X";

            int width = 4;

            Program.Point location = Program.GetGridCoordinates(grid, width, targetElement);

            Assert.Equal(3, location.Y);
            Assert.Equal(3, location.X);
        }

        [Fact]
        public static void GetGridCoordinates_FindsMiddleCoordinate()
        {
            string targetElement = "X";

            string grid = "🏔🏔🏔🏔\n"
                        + "🏔🏔X🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔";

            int width = 4;

            Program.Point location = Program.GetGridCoordinates(grid, width, targetElement);

            Assert.Equal(1, location.Y);
            Assert.Equal(2, location.X);
        }

        [Fact]
        public static void GetPossibleTargetLocations_ReturnsCorrectly()
        {
            Program.Point pointToExclude = new Program.Point{X = 1, Y = 2};

            int width = 3;

            List<Program.Point> locations = Program.GetPossibleTargetLocations(width, pointToExclude);

            Program.Point[] expectedLocations = new Program.Point[]{
                new Program.Point{ X = 0, Y = 0 },
                new Program.Point{ X = 0, Y = 1 },
                new Program.Point{ X = 0, Y = 2 },
                new Program.Point{ X = 1, Y = 0 },
                new Program.Point{ X = 1, Y = 1 },
                new Program.Point{ X = 2, Y = 0 },
                new Program.Point{ X = 2, Y = 1 },
                new Program.Point{ X = 2, Y = 2 },
            };

            Assert.Equal(expectedLocations, locations.ToArray());
        }

        [Fact]
        public static void GetRandomPollOptions_DoesntReturnFire() {
            System.Random rand = new System.Random();
            for (int i = 0; i < 1000; i++) {
                IEnumerable<string> options = EmojiIndex.GetRandomPollOptions(rand);
                Assert.DoesNotContain("🔥️ Fire", options);
            }
        }

        [Fact]
        public void TickVolcano_WorksInTopLeftCorner() {
            string grid = "🌋🌳️🌳️\n"
                        + "🌳️🌳️🌳️\n"
                        + "🌳️🌳️🌳️";
            int width = 3;
            Program.Point volcanoLocation = new Program.Point{ X = 0, Y = 0};

            string fullFire = "🌋🔥️🌳️\n"
                            + "🔥️🌳️🌳️\n"
                            + "🌳️🌳️🌳️";

            grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation);
            grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation);

            StringEqual(fullFire, grid);
        }

        [Fact]
        public static void TickVolcano_WorksInBottomRightCorner() {
            string grid = Program.NormalizeEmojiRepresentation("🌳️🌳️🌳️\n"
                        + "🌳️🌳️🌳️\n"
                        + "🌳️🌳️🌋");
            int width = 3;
            Program.Point volcanoLocation = new Program.Point{ X = 2, Y = 2};

            string expectedResult = Program.NormalizeEmojiRepresentation("🌳️🌳️🌳️\n"
                                  + "🌳️🌳️🔥️\n"
                                  + "🌳️🔥️🌋");

            grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation);
            grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation);

            Assert.Equal(expectedResult, grid);
        }

        [Fact]
        public void TickGrid_UpdatesVolcano() {
            string grid = "🌳️🌳️🌳️\n"
                        + "🌳️🌋🌳️\n"
                        + "🌳️🌳️🌳️";
            int width = 3;
            grid = Program.TickGridElements(grid, width);
            string expected = Program.NormalizeEmojiRepresentation(
                "🌳️🌳️🌳️\n"
                + "🌳️🌋🌳️\n"
                + "🌳️🔥️🌳️");
            StringEqual(expected, grid);

            grid = Program.TickGridElements(grid, width);
            expected = Program.NormalizeEmojiRepresentation(
                "🌳️🌳️🌳️\n"
                + "🌳️🌋🔥️\n"
                + "🌳️🌫️🌳️");
            StringEqual(expected, grid);

            grid = Program.TickGridElements(grid, width);
            expected = Program.NormalizeEmojiRepresentation(
                "🌳️🔥️🌳️\n"
                + "🌳️🌋🌫️\n"
                + "🌳️🏜️🌳️");
            StringEqual(expected, grid);
        }

        [Fact]
        public static void TickGrid_VolcanoDoesntLightNonFlammableElementsOnFire() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                  "🐉🐉🐉\n"
                                + "🐉🌋🐉\n"
                                + "🐉🐉🐉");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            Assert.Equal(originalGrid, tickedGrid);

            for (int i = 0; i < 1000; i++) {
                tickedGrid = Program.TickGridElements(tickedGrid, width);
            }

            Assert.Equal(originalGrid, tickedGrid);
        }

        [Fact]
        public static void TickGrid_VolcanoDoesntLightTreesWhenAdjacentToHole() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                "🌳️🌳️🌳️\n"
                                + "🌳️🌋🕳️\n"
                                + "🌳️🌳️🌳️");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            Assert.Equal(originalGrid, tickedGrid);
        }

        [Fact]
        public static void TickGrid_VolcanoTurnsWaterToIsland() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                "🌊️🌊️🌊️\n"
                                + "🌊️🌋🌊️\n"
                                + "🌊️🌊️🌊️");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            string expected = Program.NormalizeEmojiRepresentation(
                            "🌊️🌊️🌊️\n"
                            + "🌊️🌋🌊️\n"
                            + "🌊️🏝️🌊️");
            Assert.Equal(expected, tickedGrid);
        }

        [Fact]
        public static void TickGrid_VolcanoTurnsIslandToBeach() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                "🌊️🏝️🌊️\n"
                                + "🏝️🌋🏝️\n"
                                + "🌊️🏝️🌊️");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            string expected = Program.NormalizeEmojiRepresentation(
                            "🌊️🏝️🌊️\n"
                            + "🏝️🌋🏝️\n"
                            + "🌊️🏖️🌊️");
            Assert.Equal(expected, tickedGrid);
        }

        [Fact]
        public static void TickGrid_VolcanoTurnsBeachToDesert() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                "🌊️🏖️🌊️\n"
                                + "🏖️🌋🏖️\n"
                                + "🌊️🏖️🌊️");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            string expected = Program.NormalizeEmojiRepresentation(
                            "🌊️🏖️🌊️\n"
                            + "🏖️🌋🏖️\n"
                            + "🌊️🏜️🌊️");
            Assert.Equal(expected, tickedGrid);
        }

        [Fact]
        public static void TickGrid_VolcanoTurnsDesertToMountain() {
            string originalGrid = Program.NormalizeEmojiRepresentation(
                                "🌊️🏜️🌊️\n"
                                + "🏜️🌋🏜️\n"
                                + "🌊️🏜️🌊️");
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width);

            string expected = Program.NormalizeEmojiRepresentation(
                            "🌊️🏜️🌊️\n"
                            + "🏜️🌋🏜️\n"
                            + "🌊️⛰️🌊️");
            Assert.Equal(expected, tickedGrid);
        }

        [Fact]
        public static void GetZoneGrid_Works() {
            string elementsGrid = "🌳️🏠️\n"
                                + "🎢️🏬️";
            int width = 2;
            string zoneGrid = Program.GetZoneGrid(elementsGrid, width);

            string expectedZoneGrid = "💚🟦\n"
                                    + "🔶🟡";

            Assert.Equal(expectedZoneGrid, zoneGrid);
        }

        [Fact]
        public void Eggs_SurroundedByMountains_HatchToDragons() {
            string elementsGrid = "⛰️🌋\n"
                                + "⛰️🥚️";
            int width = 2;
            string zoneGrid = Program.TickGridElements(elementsGrid, width);

            string lizardGrid = Program.NormalizeEmojiRepresentation("⛰️🌋\n"
                              + "⛰️🦎");

            StringEqual(lizardGrid, zoneGrid);

            zoneGrid = Program.TickGridElements(zoneGrid, width);

            string gatorGrid = Program.NormalizeEmojiRepresentation("⛰️🌋\n"
                             + "⛰️🐊");

            StringEqual(gatorGrid, zoneGrid);

            zoneGrid = Program.TickGridElements(zoneGrid, width);

            string dragonGrid = Program.NormalizeEmojiRepresentation("⛰️🌋\n"
                              + "⛰️🐉");

            StringEqual(dragonGrid, zoneGrid);
        }

        [Fact]
        public void Eggs_SurroundedByTrees_HatchToEagles() {
            string elementsGrid = Program.NormalizeEmojiRepresentation("🌳🌳\n"
                                + "🌳🥚️");
            int width = 2;
            string actualGrid = Program.TickGridElements(elementsGrid, width);

            string expectedGrid = Program.NormalizeEmojiRepresentation("🌳🌳\n"
                              + "🌳🐣️");

            StringEqual(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width);

            expectedGrid = Program.NormalizeEmojiRepresentation("🌳🌳\n"
                         + "🌳🐤️");

            StringEqual(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width);

            expectedGrid = Program.NormalizeEmojiRepresentation("🌳🌳\n"
                         + "🌳🐦️");

            StringEqual(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width);
            
            expectedGrid = Program.NormalizeEmojiRepresentation("🌳🌳\n"
                         + "🌳🦅️");

            StringEqual(expectedGrid, actualGrid);
        }

        
        [Fact]
        public void Eggs_SurroundedByWater_HatchToOctopus() {
            string elementsGrid = "🌊️🌊️\n"
                                + "🌊️🥚️";
            int width = 2;
            string actualGrid = Program.TickGridElements(elementsGrid, width);

            string expectedGrid = "🌊️🌊️\n"
                              + "🌊️🐟️";

            StringEqual(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width);

            expectedGrid = "🌊️🌊️\n"
                         + "🌊️🐙️";

            StringEqual(expectedGrid, actualGrid);
        }

        [Fact]
        public void Eggs_SurroundedByBuilding_HatchToRat() {
            string elementsGrid = "🏢🏢\n"
                                + "🏢🥚️";
            int width = 2;
            string actualGrid = Program.TickGridElements(elementsGrid, width);

            string expectedGrid = Program.NormalizeEmojiRepresentation("🏢🏢\n"
                              + "🏢🐀");

            StringEqual(expectedGrid, actualGrid);
        }

        [Fact]
        public void Eggs_SurroundedByTourism_HatchToTiger() {
            string elementsGrid = "🎪️🎪️\n"
                                + "🎪️🥚️";
            int width = 2;
            string actualGrid = Program.TickGridElements(elementsGrid, width);

            string expectedGrid = "🎪️🎪️\n"
                                + "🎪️🐈️";

            StringEqual(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width);

            expectedGrid = "🎪️🎪️\n"
                         + "🎪️🐅️";

            StringEqual(expectedGrid, actualGrid);
        }

        [Fact]
        public static void TickElements_HandlesMixedEmojiRepresentation()
        {
            string grid = "🌲🏛🌊⛰⛲🏕🎡🌳🏜️🏜️\n🏡🏚🏝🌊🌳🏩🥚🏖🌋🏜️\n🕳🌊🏝⛰🏖🏖🏜️🏜🕳🏜️\n🗻🏫🏜📡🏜️🌋🏜️🏝🏚🌳\n❓📡📡🌴⛰🏜️🏜️🌴🏔🌴\n🎢🕳🌳🕳🌴🌳🌲🏕🏞🏝\n🎡🗻🏞⛲🏕🌲⛺🎢🏯🕳\n🌊🏔🏕🌳🏞🏕🏩🌊🎡🏖\n🏔🕳🏔🕳🏜️🏜️🏜️🌲🏞🌴\n🐉⛰🏜️🌋🏜️🌋🏔🏜⛺🐤";
            Program.TickGridElements(grid, 10);
        }

        [Fact]
        public static void NormalizeEmojiRepresentation_TurnsTextIntoEmoji()
        {
            Assert.Equal("🏜️", Program.NormalizeEmojiRepresentation("🏜"));
        }

        [Fact]
        public static void NormalizeEmojiRepresentation_LeavesEmojiAsIs()
        {
            Assert.Equal("🏜️", Program.NormalizeEmojiRepresentation("🏜️"));
        }

        [Fact]
        public void NormalizeEmojiRepresentation_TurnsMultipleTextCharactersIntoEmoji()
        {
            string expected = "🏜️🏜️\n"
                            + "🏜️🏜️";
            string actual = Program.NormalizeEmojiRepresentation("🏜🏜\n"
                                                               + "🏜🏜");

            StringEqual(expected, actual);
        }

        [Fact]
        public void SatelliteDishes_SpawnAlien()
        {
            string initial = "🏜️🏜️🏜️🏜️\n"
                           + "🏜️🏜️🏜️📡️\n"
                           + "📡️📡️📡️🏜️\n"
                           + "🏜️🏜️🏜️🏜️";
            int width = 4;
            string actual = Program.TickGridElements(initial, width);

            string expected = "🏜️🏜️🏜️🏜️\n"
                            + "🏜️🏜️🏜️📡️\n"
                            + "📡️🛸️📡️🏜️\n"
                            + "🏜️🏜️🏜️🏜️";

            StringEqual(expected, actual);
        }
    }
}