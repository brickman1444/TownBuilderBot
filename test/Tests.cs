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
        public static void WinningOption_PicksObviousWinner()
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

            string winner = Program.GetWinningOption(poll, new System.Random());
            Assert.Equal("A", winner);
        }

        [Fact]
        public static void WinningOption_RandomlyBreaksTies()
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
                string winner = Program.GetWinningOption(poll, rand);
                wins[winner]++;
            }

            Assert.Equal(0, wins["A"]);
            Assert.InRange(wins["B"], 400, 600);
            Assert.InRange(wins["C"], 400, 600);
            Assert.Equal(0, wins["D"]);
        }

        [Fact]
        public static void ReplaceTargetWithPollWinner_WorksWhenPollOptionsAreMoreThanJustAnEmoji()
        {
            Mastonet.Entities.Poll poll = new Mastonet.Entities.Poll() {
                Options = new Mastonet.Entities.PollOption[] {
                    new Mastonet.Entities.PollOption() {
                        Title = "🐵 Monkey",
                        VotesCount = 1,
                    },
                }
            };

            string targetElement = "X";

            string grid = "🏔🏔🏔🏔\n"
                        + "🏔🏔X🏔\n"
                        + "🏔🏔🏔🏔\n"
                        + "🏔🏔🏔🏔";

            System.Random rand = new System.Random();

            string resultGrid = Program.ReplaceTargetWithPollWinner(poll, targetElement, grid, rand);

            string expectedResult = "🏔🏔🏔🏔\n"
                                  + "🏔🏔🐵🏔\n"
                                  + "🏔🏔🏔🏔\n"
                                  + "🏔🏔🏔🏔";

            Assert.Equal(expectedResult, resultGrid);
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
                List<EmojiIndex.EmojiData> options = EmojiIndex.GetRandomPollOptions(rand);
                Assert.DoesNotContain(options, o => o.Name == "Fire");
            }
        }
    }
}