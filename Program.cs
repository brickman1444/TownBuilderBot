﻿using System;
using System.IO;
using System.Net;
using System.Text;

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

            CreateCard();
        }

        static void CreateCard()
        {
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("url");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().GetAwaiter().GetResult())
            {
                using (StreamReader readStream = new StreamReader(response.GetResponseStream(), encode))
                {
                    string responseText = readStream.ReadToEnd();
                }
            }
        }

        static void PullTwitterPoll()
        {
            TwitterClient client = InitializeTwitterCredentials();

            //var tweet = Tweetinvi.Tweet.GetTweet(1264633640270651393);

            //Tweetinvi.Models.ITweet tweet = client.Tweets.GetTweetAsync(1264633640270651393).GetAwaiter().GetResult();

            var result = client.Execute.RequestAsync(request =>
            {
                //request.Url = "https://api.twitter.com/labs/2/statuses/show/1264633640270651393.json";
                request.Url = "https://api.twitter.com/labs/2/tweets/1264633640270651393?expansions=attachments.poll_ids";
                //request.Url = "https://api.twitter.com/labs/2/tweets/1264633640270651393?expansions=attachments.media_keys&tweet.fields=created_at";
                request.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
            }).GetAwaiter().GetResult();

            Console.WriteLine(result.Content);

            // {"data":{"attachments":{"poll_ids":["1264633639914045440"]},"id":"1264633640270651393","text":"\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDFDD️\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\n\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF34\uD83C\uDF33\n\uD83C\uDF0A\uD83C\uDF34\uD83C\uDF34\uD83C\uDF34\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF34\uD83C\uDF34\uD83C\uDF33\uD83C\uDF33\n\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF0A\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF32\n\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33❓\uD83C\uDF0A\uD83C\uDF0A\uD83C\uDF33\uD83C\uDF32\uD83C\uDF32\n\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF0A\uD83C\uDF32⛰⛰\n\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF33\uD83C\uDF32\uD83C\uDF0A⛰⛰\uD83C\uDFDC\n\uD83C\uDF33\uD83C\uDF32\uD83C\uDF33\uD83C\uDF33\uD83C\uDF32\uD83C\uDF32⛰⛰\uD83C\uDFDC\uD83C\uDFDC\n\uD83C\uDF32⛰\uD83C\uDF32\uD83C\uDF32\uD83C\uDF32⛰\uD83C\uDFD4⛰\uD83C\uDFDC\uD83C\uDFDC\n\uD83C\uDF32\uD83C\uDF32\uD83C\uDF32\uD83C\uDF32⛰\uD83C\uDFD4\uD83C\uDFD4⛰\uD83C\uDFDC\uD83C\uDFDC"},"includes":{"polls":[{"id":"1264633639914045440","options":[{"position":1,"label":"House \uD83C\uDFE0","votes":0},{"position":2,"label":"Post Office \uD83C\uDFE4","votes":0},{"position":3,"label":"Castle \uD83C\uDFF0","votes":1}]}]}}
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
