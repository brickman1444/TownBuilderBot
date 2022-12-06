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

            string pollWinner = GetWinningOption(latestStatus.Poll);

            string questionMark = "❓";

            string newGridWithoutQuestionMark = latestStatusAsCharacters.Replace(questionMark, pollWinner);

            Random rand = new Random();

            int randomX = rand.Next(GridWidth);
            int randomY = rand.Next(GridWidth);

            string newGrid = ReplaceElement(newGridWithoutQuestionMark, GridWidth, randomX, randomY, questionMark);

            List<string> pollOptions = RandomFirstN(4, EmojiIndex.All, rand);

            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = pollOptions.ToArray(),
                ExpiresIn = System.TimeSpan.FromHours(1),
            };

            var _ = client.PublishStatus(newGrid, poll: poll).Result;
        }

        private static string ReplaceHTMLWithCharacters(string input)
        {
            return input.Replace("<p>", "").Replace("<br />", "\n").Replace("</p>", "");
        } 

        private static string GetWinningOption(Mastonet.Entities.Poll poll)
        {
            if (poll == null)
            {
                Console.WriteLine("Couldn't find poll. Default option.");
                return "🌳";
            }

            Mastonet.Entities.PollOption winningOption = poll.Options.OrderByDescending(o => o.VotesCount).First();

            return winningOption.Title;
        }

        public static List<string> RandomFirstN(int n, string[] inArray, Random rand)
        {
            List<string> list = new List<string>(inArray);

            List<string> output = new List<string>();

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
    }
}
