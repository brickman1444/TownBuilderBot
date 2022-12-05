﻿using System;
using Mastonet;

namespace TownBuilderBot
{
    static class Program
    {
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

            // string instance = Environment.GetEnvironmentVariable("mastodonInstance");
            // string accessToken = Environment.GetEnvironmentVariable("mastodonAccessToken");
            // MastodonClient client = new MastodonClient(instance, accessToken);

            var status = "🌊🌊🌊🌊🏝️🌊🌊🌊🌊🌊\n" +
                "🌊🌊🌊🌊🌊🌊🌊🌊🌴🌳\n" +
                "🌊🌴🌴🌴🌊🌊🌴🌴🌳🌳\n" +
                "🌳🌳🌳🌳🌳🌊🌳🌳🌳🌲\n" +
                "🌳🌳🌳🌳❓🌊🌊🌳🌲🌲\n" +
                "🌳🌳🌳🌳🌳🌳🌊🌲⛰⛰\n" +
                "🌳🌳🌳🌳🌳🌲🌊⛰⛰🏜\n" +
                "🌳🌲🌳🌳🌲🌲⛰⛰🏜🏜\n" +
                "🌲⛰🌲🌲🌲⛰🏔⛰🏜🏜\n" +
                "🌲🌲🌲🌲⛰🏔🏔⛰🏜🏜";

            var stringInfo = new System.Globalization.StringInfo(status);

            for (int element = 0; element < stringInfo.LengthInTextElements; element++) {
                Console.WriteLine(String.Format(
                "Text element {0} is '{1}'",
                element, stringInfo.SubstringByTextElements(element, 1)));
            }

            Mastonet.Entities.PollParameters poll = new Mastonet.Entities.PollParameters()
            {
                Options = new string[] { "🏠", "🏤", "🏰" },
                ExpiresIn = System.TimeSpan.FromDays(1),
            };

            //client.PublishStatus(status, poll: poll);
        }
    }
}
