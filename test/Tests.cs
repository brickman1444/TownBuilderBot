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

            Assert.Equal(result, expectedResult);
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

            Assert.Equal(result, expectedResult);
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
    }
}