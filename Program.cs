﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mastonet;
using dotenv.net;

namespace TownBuilderBot
{
    static class Program
    {
        const int GridWidth = 10;

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

            string instance = Environment.GetEnvironmentVariable("mastodonInstance");
            string accessToken = Environment.GetEnvironmentVariable("mastodonAccessToken");
            MastodonClient client = new MastodonClient(instance, accessToken);

            Mastonet.Entities.Account account = client.GetCurrentUser().Result;

            string accountId = Environment.GetEnvironmentVariable("mastodonAccountId");
            var statuses = client.GetAccountStatuses(accountId, new ArrayOptions(){ Limit = 1 }).Result;

            Mastonet.Entities.Status latestStatus = statuses.First();

            string latestStatusAsCharacters = ReplaceHTMLWithCharacters(latestStatus.Content);

            Random rand = new Random();

            string questionMark = "❓";

            string newGridWithoutQuestionMark = ReplaceTargetWithPollWinner(latestStatus.Poll, questionMark, latestStatusAsCharacters, rand);

            int randomX = rand.Next(GridWidth);
            int randomY = rand.Next(GridWidth);

            string newGrid = ReplaceElement(newGridWithoutQuestionMark, GridWidth, randomX, randomY, questionMark);

            List<EmojiIndex.EmojiData> pollOptions = RandomFirstN(4, EmojiIndex.All, rand);

            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = pollOptions.Select(d => d.Emoji + " " + d.Name),
                ExpiresIn = System.TimeSpan.FromDays(1),
            };

            var _ = client.PublishStatus(newGrid, poll: poll).Result;
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

        public static List<T> RandomFirstN<T>(int n, T[] inArray, Random rand)
        {
            List<T> list = new List<T>(inArray);

            List<T> output = new List<T>();

            for (int i = 0; i < n; i++)
            {
                int randomIndex = rand.Next(list.Count);
                output.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }

            return output;
        }

        public static string ReplaceElement(string inGrid, int width, int x, int y, string newString)
        {
            int index = y * (width + 1) + x;

            System.Globalization.StringInfo stringInfo = new System.Globalization.StringInfo(inGrid);

            string prefix = stringInfo.SubstringByTextElements(0, index);

            string suffix = stringInfo.SubstringByTextElements(index + 1);

            return prefix + newString + suffix;
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
