using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using System.Threading;

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

            stream.AddTrack(track);//sets the passed in parameter as the hastag to search
            stream.MatchingTweetReceived += (sender, theTweet) =>
            {
                Console.WriteLine($"\n{theTweet.Tweet}");

                Tweet.PublishTweet(theTweet.Tweet.ToString());
            };
            stream.StartStreamMatchingAllConditions();

        }
    }
}
