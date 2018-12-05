using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using System.Threading;
using System.Text.RegularExpressions;

namespace TwitterConsoleApp
{
    class Program
    {
        static string CONSUMER_KEY;
        static string CONSUMER_SECRET;
        static string ACCESS_TOKEN;
        static string ACCESS_TOKEN_SECRET;

        static void Main(string[] args)
        {
            Run();
        }

        public static void Run()
        {
            //reads Twitter API credentials from secret file
            using (StreamReader reader = new StreamReader(Path.Combine("secret.txt")))
            {
                string[] results = reader.ReadToEnd().Split('\n');
                CONSUMER_KEY = results[0].Trim();
                CONSUMER_SECRET = results[1].Trim();
                ACCESS_TOKEN = results[2].Trim();
                ACCESS_TOKEN_SECRET = results[3].Trim();
            }
            //Sets the credentials read from secret file to the oauth process on tewwtinvi
            Auth.SetUserCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);


            Console.Clear();

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("           Welcome to the Twitter Console App           ");
            Console.WriteLine("\n This is a simple console application built using the \n TweetInvi library for accessing Twitter's API services");
            Console.WriteLine("\n\n Please select an option:");
            Console.WriteLine(" 1) Show me some Tweets");
            Console.WriteLine("\n--------------------------------------------------------");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    DisplayTweetsMenu();
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    Thread.Sleep(2000);
                    Run();
                    break;
            }

        }

        public static void DisplayTweetsMenu()
        {
            Console.Clear();

            Console.WriteLine("\n--------------------------------------------------------");
            Console.WriteLine("\n Please select an option:\n");
            Console.WriteLine(" 1) Follow a Hashtag");
            Console.WriteLine(" 2) Follow a User");
            Console.WriteLine(" 3) Listen for interactions");
            Console.WriteLine(" 4) Send a Tweet");
            Console.WriteLine("\n--------------------------------------------------------");
            Console.WriteLine("\n 0) Exit\n");

            var option = Console.ReadLine();

            while (option != "0")
            {
                switch (option)
                {
                    case "1":
                        Console.WriteLine("Please enter your search term: ");
                        var track = Console.ReadLine();
                        AddTrack(track);
                        break;
                    case "2":
                        Console.WriteLine("Who are you looking to follow? ");
                        var screenName = Console.ReadLine();
                        FollowUser(screenName);
                        break;
                    case "3":
                        ReplyToTweet();
                        break;
                    case "4":
                        SendTweet();
                        break;
                    default:
                        Console.WriteLine("Invalid Selection");
                        Thread.Sleep(2000);
                        DisplayTweetsMenu();
                        break;
                }
            }
        }

        private static void AddTrack(string track)
        {
            int counter = 0;
            var stream = Tweetinvi.Stream.CreateFilteredStream();

            stream.AddTrack("#" + track);//sets the passed in parameter as the hastag to search
            while (counter < 10)
            {
                stream.MatchingTweetReceived += (sender, theTweet) =>
                {
                    Console.WriteLine($"\n{Regex.Replace(theTweet.Tweet.ToString(), @"amp;|\n", "")}");
                    Thread.Sleep(3000);
                    counter++;
                    //Tweet.PublishTweet(theTweet.Tweet.ToString()); 
                };
                stream.StartStreamMatchingAllConditions();
            }
           
            DisplayTweetsMenu();
        }

        public static void FollowUser(string screenName)
        {
            var user = User.GetUserFromScreenName(screenName);
            if (user != null)
            {
                foreach (var tweet in Timeline.GetUserTimeline(user, 10)) //Only get the lastest 10 tweets
                {
                    Console.WriteLine(tweet + "\n");
                }
            }
            else
            {
                Console.WriteLine("There doesn't seem to be any user by the name");
            }

            DisplayTweetsMenu(); //Go back to menu
        }

        public static void ReplyToTweet()
        {
            Console.WriteLine("\nSetting up our listeners now!");
            Thread.Sleep(2000);

            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddTrack("@devosullivanb"); //Search for tweets @ the bot

            stream.MatchingTweetReceived += (sender, theTweet) =>
            {
                if (theTweet.Tweet.UserMentions.Any((x) => x.Id == 1047486196602081281)) //bots twitter ID
                {
                    var tweetToReplyTo = Tweet.GetTweet(theTweet.Tweet.Id);
                    var replyTweet = "@" + tweetToReplyTo.CreatedBy.ScreenName + " " + GenerateRandomTweet();

                    Console.WriteLine("Just one second while we tweet you back...");
                    Thread.Sleep(2000);
                    Tweet.PublishTweetInReplyTo(replyTweet, tweetToReplyTo);
                    Console.WriteLine("There we go, check your feed!\n");
                    Thread.Sleep(2000);
                    stream.StopStream();
                }

            };
            stream.StartStreamMatchingAllConditions();

            DisplayTweetsMenu();
        }

        public static string GenerateRandomTweet()
        {
            Random random = new Random();
            int randomInt = random.Next(1,5);

            switch (randomInt)
            {
                case 1:
                    return "Hello";
                case 2:
                    return "Hey, how's it going?";
                case 3:
                    return "Hope you're having a nice day!";
                case 4:
                    return "Sorry, I'm a little busy at the moment... I'll get back to you later";
                case 5:
                    return "What's up?";
                default:
                    return "";
            }
        }

        public static void SendTweet()
        {
            Console.WriteLine("Tell us what's on your mind: \n");

            var tweet = Console.ReadLine();

            Tweet.PublishTweet(tweet);
            Console.WriteLine("\nGreat stuff, publishing your Tweet now...");
            Thread.Sleep(2000);

            DisplayTweetsMenu();
        }
    }
}
