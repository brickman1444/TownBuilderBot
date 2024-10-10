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

        public class Post
        {
            public string body;
            public IEnumerable<string> pollOptions;
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

            bool isReadOnly = args.Contains("readonly");

            bool forceZoningMode = args.Contains("zoning");

            DotEnv.Load();

            MastodonClient client = MakeClient();

            Post post = MakeNormalPost(client, 10);

            if (isReadOnly) {
                PrintPost(post);
            } else {
                PublishPoll(client, post);
            }
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

        private static void PublishPoll(MastodonClient client, Post post)
        {
            Console.WriteLine("Publishing Post");
            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = post.pollOptions,
                ExpiresIn = System.TimeSpan.FromDays(1),
            };

            var _ = client.PublishStatus(post.body, poll: poll).Result;
        }

        private static void PrintPost(Post post)
        {
            Console.WriteLine(post.body);
            foreach (string option in post.pollOptions)
            {
                Console.WriteLine(option);
            }
        }

        private static Post MakeNormalPost(MastodonClient client, int gridWidth)
        {
            Mastonet.Entities.Status latestStatus = GetLatestStatus(client);

            string latestStatusAsCharacters = ReplaceHTMLWithCharacters(latestStatus.Content);

            Random rand = new Random();

            string newGrid = UpdateGrid(latestStatusAsCharacters, latestStatus.Poll, gridWidth, rand);

            IEnumerable<string> pollOptions = EmojiIndex.GetRandomPollOptions(rand);

            return new Post(){ body = newGrid, pollOptions = pollOptions };
        }

        private static string UpdateGrid(string oldGrid, Mastonet.Entities.Poll poll, int gridWidth, Random rand)
        {
            Point oldQuestionMarkLocation = GetGridCoordinates(oldGrid, gridWidth, QuestionMark);
            string winningEmoji = GetPollWinningElement(poll, rand);

            string tickedGrid = TickGridElements(oldGrid, gridWidth, rand);

            string newGridWithoutQuestionMark = ReplaceElement(tickedGrid, gridWidth, oldQuestionMarkLocation, winningEmoji);

            List<Point> possibleLocations = GetPossibleTargetLocations(gridWidth, oldQuestionMarkLocation);

            Point newTargetLocation = possibleLocations[rand.Next(possibleLocations.Count)];

            return ReplaceElement(newGridWithoutQuestionMark, gridWidth, newTargetLocation, QuestionMark);
        }

        public static string TickGridElements(string grid, int gridWidth, Random rand) {
            grid = NormalizeEmojiRepresentation(grid);
            foreach (EmojiIndex.EmojiData elementData in EmojiIndex.All) {
                if (elementData.TickFunction == null) {
                    continue;
                }

                for (int x = 0; x < gridWidth; x++) {
                    for (int y = 0; y < gridWidth; y++) {
                        int index = y * (gridWidth + 1) + x;
                        System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(grid);
                        string element = stringInfo.SubstringByTextElements(index, 1);

                        if (element == elementData.Emoji) {
                            grid = elementData.TickFunction(grid, gridWidth, new Point(){X = x, Y = y});
                        }
                    }
                }
            }

            return grid;
        }

        public static string GetElement(string grid, int width, Point location)
        {
            if (location.X < 0 || width <= location.X || location.Y < 0 || width <= location.Y)
            {
                return null;
            }

            int index = location.Y * (width + 1) + location.X;
            System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(grid);
            return stringInfo.SubstringByTextElements(index, 1);
        }

        private static string ReplaceHTMLWithCharacters(string input)
        {
            return input.Replace("<p>", "").Replace("<br />", "\n").Replace("</p>", "");
        }

        public static string NormalizeEmojiRepresentation(string input)
        {
            string output = "";
            char emojiSelector = Centvrio.Emoji.VariationSelectors.VS16.ToCharArray().Last();

            var enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(input);
            while (enumerator.MoveNext())
            {
                string inputElement = enumerator.GetTextElement();
                if (inputElement == "\n" || inputElement == "\r" || inputElement.ToCharArray().Last() == emojiSelector) {
                    output += inputElement;
                }
                else
                {
                    output += inputElement + emojiSelector;
                }
            }

            return output;
        }

        public static string ReplaceElement(string inGrid, int width, Point location, Centvrio.Emoji.UnicodeString newString)
        {
            return ReplaceElement(inGrid, width, location.X, location.Y, (newString + Centvrio.Emoji.VariationSelectors.VS16).ToString());
        }

        public static string ReplaceElement(string inGrid, int width, Point location, string newString)
        {
            return ReplaceElement(inGrid, width, location.X, location.Y, newString);
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

        public static string GetPollWinningElement(Mastonet.Entities.Poll poll, Random rand) {
            if (poll == null)
            {
                Console.WriteLine("Couldn't find poll. Default option.");
                return "🌳";
            }

            int maxVotes = poll.Options.Max(o => o.VotesCount ?? 0);
            IEnumerable<Mastonet.Entities.PollOption> winningOptions = poll.Options.Where(o => o.VotesCount == maxVotes);

            int randIndex = rand.Next(winningOptions.Count());
            Mastonet.Entities.PollOption winningOption = winningOptions.ElementAt(randIndex);

            System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(winningOption.Title);
            return stringInfo.SubstringByTextElements(0, 1);
        }

        public static string GetZoneGrid(string elementsGrid, int gridWidth)
        {
            string zoneGrid = elementsGrid;
            for (int x = 0; x < gridWidth; x++) {
                for (int y = 0; y < gridWidth; y++) {
                    string element = GetElement(elementsGrid, gridWidth, new Point{X = x, Y = y});

                    EmojiIndex.EmojiData data = EmojiIndex.GetData(element);

                    zoneGrid = ReplaceElement(zoneGrid, gridWidth, x, y, EmojiIndex.GetZoneEmoji(data.Zone));
                }
            }
            return zoneGrid;
        }
    }
}
