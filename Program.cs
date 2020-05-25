﻿using System;
using System.IO;

using Tweetinvi;

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

            TwitterClient client = InitializeTwitterCredentials();

            //var tweet = Tweetinvi.Tweet.GetTweet(1264633640270651393);

            Tweetinvi.Models.ITweet tweet = client.Tweets.GetTweetAsync(1264633640270651393).GetAwaiter().GetResult();

            //GenerateQuoteAndTweet();
        }

        static void GenerateQuoteAndTweet()
        {
            InitializeTwitterCredentials();

            Tweet("🌊🌊🌊🌊🏝️🌊🌊🌊🌊🌊\n" +
                "🌊🌊🌊🌊🌊🌊🌊🌊🌴🌳\n" +
                "🌊🌴🌴🌴🌊🌊🌴🌴🌳🌳\n" +
                "🌳🌳🌳🌳🌳🌊🌳🌳🌳🌲\n" +
                "🌳🌳🌳🌳❓🌊🌊🌳🌲🌲\n" +
                "🌳🌳🌳🌳🌳🌳🌊🌲⛰⛰\n" +
                "🌳🌳🌳🌳🌳🌲🌊⛰⛰🏜\n" +
                "🌳🌲🌳🌳🌲🌲⛰⛰🏜🏜\n" +
                "🌲⛰🌲🌲🌲⛰🏔⛰🏜🏜\n" +
                "🌲🌲🌲🌲⛰🏔🏔⛰🏜🏜");
        }

        static TwitterClient InitializeTwitterCredentials()
        {
            string consumerKey = System.Environment.GetEnvironmentVariable("twitterConsumerKey");
            string consumerSecret = System.Environment.GetEnvironmentVariable("twitterConsumerSecret");
            string accessToken = System.Environment.GetEnvironmentVariable("twitterAccessToken");
            string accessTokenSecret = System.Environment.GetEnvironmentVariable("twitterAccessTokenSecret");

            if (consumerKey == null)
            {
                using (StreamReader fs = File.OpenText("localconfig/twitterKeys.txt"))
                {
                    consumerKey = fs.ReadLine();
                    consumerSecret = fs.ReadLine();
                    accessToken = fs.ReadLine();
                    accessTokenSecret = fs.ReadLine();
                }
            }

            return new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        static void Tweet(string quote)
        {
            Console.WriteLine("Publishing tweet: " + quote);
            //var tweet = Tweetinvi.Tweet.PublishTweet(quote);
        }
    }
}
