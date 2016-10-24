using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCrawlerDotNet
{
    internal class Program
    {
        public static Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex linkParser2 = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static List<string> links = new List<string>();
        public static int currentthreads = 0;
        public static int currentindex = 0;

        public static void Main(string[] args)
        {
            Console.WriteLine("[Main Thread] Bootstrap Application.");
            Console.WriteLine("[Main Thread] Processors: " + Environment.ProcessorCount);
            Console.WriteLine("[Main Thread] Please enter start URL:");
            Console.Write("> ");
            string starturl = Console.ReadLine();
            Console.WriteLine(starturl);
            links.Add(starturl);
            Task.Run(() => manageThreads());
            Console.WriteLine("Presse a key to leave.");
            Console.ReadLine();
        }

        public static void manageThreads()
        {
            while (true)
            {
                if (currentthreads < 16 * Environment.ProcessorCount && currentindex <= links.Count - 1)
                {
                    Console.WriteLine("[Manage Thread] Start new request Thread. Start new Thread at Index: " +
                                      currentindex);
                    int x = currentindex;
                    currentthreads++;
                    int y = currentthreads;
                    Task.Run(() => requestThread(x, y));
                    currentindex++;
                }
            }
        }


        public static async void requestThread(int index, int thisthread)
        {
            try
            {
                string url = links[index];
                Console.WriteLine("[Request Thread "+thisthread+"] URL: " + url);
                HttpClient client = new HttpClient();
                Uri uri = new Uri(url);
                client.BaseAddress = uri;
                var response = await client.GetAsync("");
                string sresponse = await response.Content.ReadAsStringAsync();
                foreach (Match m in Regex.Matches(sresponse, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
                {
                    if (!links.Contains(m.Value)) links.Add(m.Value);
                }
                currentthreads = currentthreads - 1;
            }
            catch (Exception)
            {
                currentthreads = currentthreads - 1;
                Console.WriteLine("[Request Thread "+thisthread+"] Error.");
            }
        }
    }
}


