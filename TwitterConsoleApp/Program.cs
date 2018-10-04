using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string CONSUMER_KEY;
            string CONSUMER_SECRET;
            string ACCESS_TOKEN;
            string ACCESS_TOKEN_SECRET;

            //reads Twitter API credentials from secret file
            using (StreamReader reader = new StreamReader(Path.Combine("secret.txt")))
            {
                string[] results = reader.ReadToEnd().Split('\n');
                CONSUMER_KEY = results[0].Trim();
                CONSUMER_SECRET = results[1].Trim();
                ACCESS_TOKEN = results[2].Trim();
                ACCESS_TOKEN_SECRET = results[3].Trim();
            }

            Auth.SetUserCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);

            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddTrack("MARVEL"); //sets the tag to be searched for on twitter by Tweetinvi
            stream.MatchingTweetReceived += (sender, theTweet) =>
            {
                Console.WriteLine($"\n{theTweet.Tweet}\n");
            };
            stream.StartStreamMatchingAllConditions();

        }

    }
}

