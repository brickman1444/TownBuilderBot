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

            var statuses = client.GetAccountStatuses(account.Id, new ArrayOptions(){ Limit = 1 }).Result;

            Mastonet.Entities.Status latestStatus = statuses.First();

            // TODO
            if (latestStatus.Poll

            var startingGrid = "🌊🌊🌊🌊🏝️🌊🌊🌊🌊🌊\n" +
                "🌊🌊🌊🌊🌊🌊🌊🌊🌴🌳\n" +
                "🌊🌴🌴🌴🌊🌊🌴🌴🌳🌳\n" +
                "🌳🌳🌳🌳🌳🌊🌳🌳🌳🌲\n" +
                "🌳🌳🌳🌳🌳🌊🌊🌳🌲🌲\n" +
                "🌳🌳🌳🌳🌳🌳🌊🌲⛰⛰\n" +
                "🌳🌳🌳🌳🌳🌲🌊⛰⛰🏜\n" +
                "🌳🌲🌳🌳🌲🌲⛰⛰🏜🏜\n" +
                "🌲⛰🌲🌲🌲⛰🏔⛰🏜🏜\n" +
                "🌲🌲🌲🌲⛰🏔🏔⛰🏜🏜";

            string questionMark = "❓";

            Random rand = new Random();

            int randomX = rand.Next(GridWidth);
            int randomY = rand.Next(GridWidth);

            string newGrid = ReplaceElement(startingGrid, GridWidth, randomX, randomY, questionMark);

            List<string> pollOptions = RandomFirstN(4, EmojiIndex.All, rand);

            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = pollOptions.ToArray(),
                ExpiresIn = System.TimeSpan.FromHours(1),
            };

            var _ = client.PublishStatus(newGrid, poll: poll).Result;
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
