using CSharpDiscordWebhook.NET.Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Drawing;
using Console = Colorful.Console;

namespace MemeGen
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = Color.Green;
            Console.Title = "Meme Gen";
            string answer = "Y";
            string webhook = "";
            while (answer != "N" && answer == "Y")
            {

                string hexCode5 = "#F65BE3";
                string hexCode4 = "#F679E5";
                string hexCode3 = "#F497DA";
                string hexCode2 = "#F8BDC4";
                string hexCode1 = "#DEF6CA";
                Color color5 = ColorTranslator.FromHtml(hexCode5);
                Color color4 = ColorTranslator.FromHtml(hexCode4);
                Color color3 = ColorTranslator.FromHtml(hexCode3);
                Color color2 = ColorTranslator.FromHtml(hexCode2);
                Color color1 = ColorTranslator.FromHtml(hexCode1);
                Console.WriteLine(
                    @"
                    $$\      $$\                                          $$$$$$\                      
                    $$$\    $$$ |                                        $$  __$$\                     
                    $$$$\  $$$$ | $$$$$$\  $$$$$$\$$$$\   $$$$$$\        $$ /  \__| $$$$$$\  $$$$$$$\  
                    $$\$$\$$ $$ |$$  __$$\ $$  _$$  _$$\ $$  __$$\       $$ |$$$$\ $$  __$$\ $$  __$$\ 
                    $$ \$$$  $$ |$$$$$$$$ |$$ / $$ / $$ |$$$$$$$$ |      $$ |\_$$ |$$$$$$$$ |$$ |  $$ |
                    $$ |\$  /$$ |$$   ____|$$ | $$ | $$ |$$   ____|      $$ |  $$ |$$   ____|$$ |  $$ |
                    $$ | \_/ $$ |\$$$$$$$\ $$ | $$ | $$ |\$$$$$$$\       \$$$$$$  |\$$$$$$$\ $$ |  $$ |
                    \__|     \__| \_______|\__| \__| \__| \_______|       \______/  \_______|\__|  \__|

", color5);


                Console.Write("Enter your discord webhook: ", color4);
                webhook = Console.ReadLine();
                bool iswebhook = true;
                if (webhook.Contains("https://discord.com/api/webhooks"))
                {
                    iswebhook = true;
                }
                else
                {
                    iswebhook = false;
                }
                while (iswebhook == false)
                {
                    Console.Write("Ivalid input (valid example input: https://discord.com/api/webhooks...), renter your discord webhook: ", Color.Red);
                    Console.WriteLine();
                    Console.Write("Enter your discord webhook: ", color4);
                    webhook = Console.ReadLine();
                }
                Console.Write("Enter subreddit: ", color3);
                string subreddit = Console.ReadLine();

                Console.Write("Enter amount: ", color2);
                int amount = int.Parse(Console.ReadLine());
                try
                {
                    List<Meme> memes = GetMemesFromReddit(subreddit, amount);
                    await SendMemesToDiscordWebhook(memes, webhook);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("you are being rate limited try requesting lower amount of posts the posts wont send to the webhook", Color.Red);

                }
                Console.Write("do you want to scrap again? (N/Y): ", Color.Green);
                answer = Console.ReadLine();
                Console.Clear();
            }
        }

        public static List<Meme> GetMemesFromReddit(string subreddit, int amount)
        {
            List<Meme> memes = new List<Meme>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36");
                var endpoint = $"https://www.reddit.com/r/{subreddit}/.json?limit={amount}";
                var response = client.GetAsync(endpoint).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var redditResponse = JsonConvert.DeserializeObject<RedditResponse>(result);

                    foreach (var child in redditResponse.Data.Children)
                    {
                        Thread.Sleep(50);
                        var meme = new Meme
                        {
                            Title = child.Data.Title,
                            ImageUrl = child.Data.Url,
                            Source = child.Data.Permalink

                        };
                        string hexCode1 = "#DEF6CA";
                        Color color1 = ColorTranslator.FromHtml(hexCode1);
                        Console.WriteLine(meme.ImageUrl, color1);
                        string imageurl = meme.ImageUrl;
                        memes.Add(meme);

                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }

            return memes;
        }

        static async Task SendMemesToDiscordWebhook(List<Meme> memes, string webhook)
        {
            string webhookUrl = webhook;
            var hook = new DiscordWebhook();
            hook.Uri = new Uri(webhookUrl);
            foreach (var meme in memes)
            {
                DiscordMessage message = new DiscordMessage();
                message.Content = $" {meme.Title} {meme.ImageUrl}";
                DiscordEmbed embed = new DiscordEmbed();
                embed.Title = meme.Title;
                embed.Image = new EmbedMedia() { Url = new Uri(meme.ImageUrl) };
                await hook.SendAsync(message);
            }
        }
    }

    public class RedditResponse
    {
        public RedditData Data { get; set; }
    }

    public class RedditData
    {
        public List<RedditChild> Children { get; set; }
    }

    public class RedditChild
    {
        public RedditPostData Data { get; set; }
    }

    public class RedditPostData
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Permalink { get; set; }
    }

    public class Meme
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Source { get; set; }
    }
}
