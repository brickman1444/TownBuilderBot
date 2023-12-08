﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mastonet;
using dotenv.net;

namespace TownBuilderBot
{
    static class Program
    {
        public class Point : IEquatable<Point>{
            public int X;
            public int Y;

            bool IEquatable<Point>.Equals(Point other)
            {
                return X == other.X && Y == other.Y;
            }
        }

        const string QuestionMark = "❓";

        public static System.IO.Stream awsLambdaHandler(System.IO.Stream inputStream)
        {
            Console.WriteLine("starting via lambda");
            Main(new string[0]);
            return inputStream;
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            DotEnv.Load();

            MastodonClient client = MakeClient();

            Mastonet.Entities.Status latestStatus = GetLatestStatus(client);

            string latestStatusAsCharacters = ReplaceHTMLWithCharacters(latestStatus.Content);

            Random rand = new Random();

            string newGrid = UpdateGrid(latestStatusAsCharacters, latestStatus.Poll, 10, rand);

            List<EmojiIndex.EmojiData> pollOptions = EmojiIndex.GetRandomPollOptions(rand);

            PublishPoll(client, newGrid, pollOptions);
        }

        private static MastodonClient MakeClient()
        {
            string instance = Environment.GetEnvironmentVariable("mastodonInstance");
            string accessToken = Environment.GetEnvironmentVariable("mastodonAccessToken");
            return new MastodonClient(instance, accessToken);
        }

        private static Mastonet.Entities.Status GetLatestStatus(MastodonClient client)
        {
            Mastonet.Entities.Account account = client.GetCurrentUser().Result;

            string accountId = Environment.GetEnvironmentVariable("mastodonAccountId");
            var statuses = client.GetAccountStatuses(accountId, new ArrayOptions(){ Limit = 1 }).Result;

            return statuses.First();
        }

        private static void PublishPoll(MastodonClient client, string newGrid, List<EmojiIndex.EmojiData> pollOptions)
        {
            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = pollOptions.Select(d => d.Emoji + " " + d.Name),
                ExpiresIn = System.TimeSpan.FromDays(1),
            };

            var _ = client.PublishStatus(newGrid, poll: poll).Result;
        }

        private static string UpdateGrid(string oldGrid, Mastonet.Entities.Poll poll, int gridWidth, Random rand)
        {
            Point oldQuestionMarkLocation = GetGridCoordinates(oldGrid, gridWidth, QuestionMark);

            string newGridWithoutQuestionMark = ReplaceTargetWithPollWinner(poll, QuestionMark, oldGrid, rand);

            List<Point> possibleLocations = GetPossibleTargetLocations(gridWidth, oldQuestionMarkLocation);

            Point newTargetLocation = possibleLocations[rand.Next(possibleLocations.Count)];

            return ReplaceElement(newGridWithoutQuestionMark, gridWidth, newTargetLocation.X, newTargetLocation.Y, QuestionMark);
        }

        public static string TickGridElements(string grid, int gridWidth, Random rand) {
            for (int x = 0; x < gridWidth; x++) {
                for (int y = 0; y < gridWidth; y++) {
                    int index = y * (gridWidth + 1) + x;
                    System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(grid);
                    string element = stringInfo.SubstringByTextElements(index, 1);

                    EmojiIndex.EmojiData elementData = EmojiIndex.All.Where(x => x.Emoji == element).First();
                    if (elementData.TickFunction != null) {
                        grid = elementData.TickFunction(grid, gridWidth, new Point(){X = x, Y = y}, rand);
                    }
                }
            }

            return grid;
        }

        private static string ReplaceHTMLWithCharacters(string input)
        {
            return input.Replace("<p>", "").Replace("<br />", "\n").Replace("</p>", "");
        } 

        public static string GetWinningOption(Mastonet.Entities.Poll poll, Random rand)
        {
            if (poll == null)
            {
                Console.WriteLine("Couldn't find poll. Default option.");
                return "🌳";
            }

            int maxVotes = poll.Options.Max(o => o.VotesCount ?? 0);

            IEnumerable<Mastonet.Entities.PollOption> winningOptions = poll.Options.Where(o => o.VotesCount == maxVotes);

            int randIndex = rand.Next(winningOptions.Count());

            Mastonet.Entities.PollOption winningOption = winningOptions.ElementAt(randIndex);

            return winningOption.Title;
        }

        public static string ReplaceElement(string inGrid, int width, int x, int y, string newString)
        {
            if (x < 0 || y < 0 || x >= width || y >= width) {
                return inGrid;
            }

            int index = y * (width + 1) + x;

            System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(inGrid);

            string prefix = stringInfo.SubstringByTextElements(0, index);

            string suffix;
            if (stringInfo.LengthInTextElements == index + 1) {
                suffix = "";
            } else {
               suffix = stringInfo.SubstringByTextElements(index + 1);
            }

            return prefix + newString + suffix;
        }

        public static Point GetGridCoordinates(string inGrid, int width, string target)
        {
            var enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(inGrid);
            
            for (int textElementIndex = 0; enumerator.MoveNext(); textElementIndex++)
            {
                if (enumerator.GetTextElement() == target)
                {
                    return new Point
                    {
                        X = textElementIndex % (width + 1),
                        Y = textElementIndex / (width + 1)
                    };
                }
            }

            return null;
        }

        public static List<Point> GetPossibleTargetLocations(int width, Point locationToExclude)
        {
            List<Point> possibleLocations = new List<Point>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (!(x == locationToExclude.X && y == locationToExclude.Y))
                    {
                        possibleLocations.Add(new Point{ X = x, Y = y});
                    }
                }
            }
            return possibleLocations;
        }

        public static string ReplaceTargetWithPollWinner(Mastonet.Entities.Poll poll, string targetElement, string grid, Random rand)
        {
            string fullPollWinner = GetWinningOption(poll, rand);

            System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(fullPollWinner);

            string winnerEmoji = stringInfo.SubstringByTextElements(0, 1);

            return grid.Replace(targetElement, winnerEmoji);
        }
    }
}
