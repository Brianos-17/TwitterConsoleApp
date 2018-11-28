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

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("\n Please select an option:");
            Console.WriteLine(" 1) Follow hashtag");
            Console.WriteLine(" 2) Follow user");
            Console.WriteLine(" 3) Listen for interactions");
            Console.WriteLine(" 4) Send a Tweet");
            Console.WriteLine("\n--------------------------------------------------------");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Please enter your search term: ");
                    var track = Console.ReadLine();
                    AddTrack(track);
                    break;
                case "2":
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

        private static void AddTrack(string track)
        {
            var stream = Tweetinvi.Stream.CreateFilteredStream();

            stream.AddTrack("#" + track);//sets the passed in parameter as the hastag to search
            stream.MatchingTweetReceived += (sender, theTweet) =>
            {
                Console.WriteLine($"\n{Regex.Replace(theTweet.Tweet.ToString(), @"amp;|\n", "")}");
                Thread.Sleep(5000);

                //Tweet.PublishTweet(theTweet.Tweet.ToString()); 
            };
            stream.StartStreamMatchingAllConditions();

        }

    //    var stream = Tweetinvi.Stream.CreateFilteredStream();

    //    var user = User.GetUserFromScreenName("@realDonaldTrump");
    //    stream.AddFollow(user); //Follow user

    //        stream.AddTrack("potus"); //sets the tag to be searched for on twitter by Tweetinvi

    //        stream.AddLocation(new Coordinates(38, 77), new Coordinates(38, 77)); //searches for tweets from a given location

    //        stream.MatchingTweetReceived += (sender, theTweet) =>
    //        {
    //            Console.WriteLine($"\n{theTweet.Tweet}");

    //            Tweet.PublishTweet(theTweet.Tweet.ToString());
    //        };
    //stream.StartStreamMatchingAllConditions();


        public static void ReplyToTweet()
        {
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddTrack("@devosullivanb"); //Search for tweets @ the bot

            stream.MatchingTweetReceived += (sender, theTweet) =>
            {
                if (theTweet.Tweet.UserMentions.Any((x) => x.Id == 1047486196602081281)) //bots twitter ID
                {
                    var replyTweet = GenerateRandomTweet();
                    var tweetToReplyTo = Tweet.GetTweet(theTweet.Tweet.Id);

                    Tweet.PublishTweetInReplyTo(replyTweet, tweetToReplyTo);
                    stream.StopStream(); //End stream after publishing reply
                }

            };
            stream.StartStreamMatchingAllConditions();
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
            Console.WriteLine("Tell us what's on your mind:");

            var tweet = Console.ReadLine();

            Tweet.PublishTweet(tweet);
        }
    }
}
