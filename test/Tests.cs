using System.Collections.Generic;
using Xunit;

namespace TownBuilderBot
{
    public static class Tests
    {
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

            Assert.Equal(expectedLocations, locations);
        }

        [Fact]
        public static void GetRandomPollOptions_DoesntReturnFire() {
            System.Random rand = new System.Random();
            for (int i = 0; i < 1000; i++) {
                IEnumerable<string> options = EmojiIndex.GetRandomPollOptions(rand);
                Assert.DoesNotContain("🔥 Fire", options);
            }
        }

        [Fact]
        public static void TickVolcano_WorksInTopLeftCorner() {
            System.Random rand = new System.Random();
            string grid = "🌋🌳🌳\n"
                        + "🌳🌳🌳\n"
                        + "🌳🌳🌳";
            int width = 3;
            Program.Point volcanoLocation = new Program.Point{ X = 0, Y = 0};

            string fullFire = "🌋🔥🌳\n"
                            + "🔥🌳🌳\n"
                            + "🌳🌳🌳";

            for (int i = 0; i < 1000; i++) {
                grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation, rand);
            }

            Assert.Equal(fullFire, grid);
        }

        [Fact]
        public static void TickVolcano_WorksInBottomRightCorner() {
            System.Random rand = new System.Random();
            string grid = "🌳🌳🌳\n"
                        + "🌳🌳🌳\n"
                        + "🌳🌳🌋";
            int width = 3;
            Program.Point volcanoLocation = new Program.Point{ X = 2, Y = 2};

            string expectedResult = "🌳🌳🌳\n"
                                  + "🌳🌳🔥\n"
                                  + "🌳🔥🌋";

            for (int i = 0; i < 1000; i++) {
                grid = EmojiIndex.TickVolcano(grid, width, volcanoLocation, rand);
            }

            Assert.Equal(expectedResult, grid);
        }

        [Fact]
        public static void TickGrid_UpdatesVolcano() {
            System.Random rand = new System.Random();
            string grid = "🌳🌳🌳\n"
                        + "🌳🌋🌳\n"
                        + "🌳🌳🌳";
            int width = 3;
            grid = Program.TickGridElements(grid, width, rand);
            Assert.Contains("🔥", grid);

            grid = Program.TickGridElements(grid, width, rand);
            Assert.Contains("🌫️", grid);

            grid = Program.TickGridElements(grid, width, rand);
            Assert.Contains("🏜️", grid);

            string expectedResult = "🌳🏜️🌳\n"
                                  + "🏜️🌋🏜️\n"
                                  + "🌳🏜️🌳";
            Assert.NotEqual(expectedResult, grid);

            for (int i = 0; i < 1000; i++) {
                grid = Program.TickGridElements(grid, width, rand);
            }

            Assert.Equal(expectedResult, grid);
        }

        [Fact]
        public static void TickGrid_VolcanoDoesntLightNonFlammableElementsOnFire() {
            System.Random rand = new System.Random();
            string originalGrid = "🌊🌊🌊\n"
                                + "🌊🌋🌊\n"
                                + "🌊🌊🌊";
            int width = 3;
            string tickedGrid = Program.TickGridElements(originalGrid, width, rand);

            Assert.Equal(originalGrid, tickedGrid);

            for (int i = 0; i < 1000; i++) {
                tickedGrid = Program.TickGridElements(tickedGrid, width, rand);
            }

            Assert.Equal(originalGrid, tickedGrid);
        }

        [Fact]
        public static void GetZoneGrid_Works() {
            string elementsGrid = "🌳🏠\n"
                                + "🎢🏬";
            int width = 2;
            string zoneGrid = Program.GetZoneGrid(elementsGrid, width);

            string expectedZoneGrid = "💚🟦\n"
                                    + "🔶🟡";

            Assert.Equal(expectedZoneGrid, zoneGrid);
        }

        [Fact]
        public static void Eggs_SurroundedByMountains_HatchToDragons() {
            System.Random rand = new System.Random();
            string elementsGrid = "⛰️🌋\n"
                                + "⛰️🥚";
            int width = 2;
            string zoneGrid = Program.TickGridElements(elementsGrid, width, rand);

            string lizardGrid = "⛰️🌋\n"
                              + "⛰️🦎";

            Assert.Equal(lizardGrid, zoneGrid);

            zoneGrid = Program.TickGridElements(zoneGrid, width, rand);

            string gatorGrid = "⛰️🌋\n"
                             + "⛰️🐊";

            Assert.Equal(gatorGrid, zoneGrid);

            zoneGrid = Program.TickGridElements(zoneGrid, width, rand);

            string dragonGrid = "⛰️🌋\n"
                              + "⛰️🐉";

            Assert.Equal(dragonGrid, zoneGrid);
        }

        [Fact]
        public static void Eggs_SurroundedByTrees_HatchToEagles() {
            System.Random rand = new System.Random();
            string elementsGrid = "🌳🌳\n"
                                + "🌳🥚";
            int width = 2;
            string actualGrid = Program.TickGridElements(elementsGrid, width, rand);

            string expectedGrid = "🌳🌳\n"
                              + "🌳🐣";

            Assert.Equal(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width, rand);

            expectedGrid = "🌳🌳\n"
                         + "🌳🐤";

            Assert.Equal(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width, rand);

            expectedGrid = "🌳🌳\n"
                         + "🌳🐦";

            Assert.Equal(expectedGrid, actualGrid);

            actualGrid = Program.TickGridElements(actualGrid, width, rand);
            
            expectedGrid = "🌳🌳\n"
                         + "🌳🦅";

            Assert.Equal(expectedGrid, actualGrid);
        }
    }
}