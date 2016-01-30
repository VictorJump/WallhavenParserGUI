﻿using HtmlAgilityPack;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WallhavenParser
{
    class Program
    {
        private HttpClient _http = new HttpClient();
        private Random _random = new Random();
        private const string Query = "geometry";

        private async Task<HtmlNode> ParseDocumentFromURL(string url) =>
            await new Func<HtmlDocument, Task<HtmlNode>>(async (d) => { d.LoadHtml(await _http.GetStringAsync(url)); return d.DocumentNode; })(new HtmlDocument());

        private int TryParseInt(string s) => s == null ? 0 : int.Parse(s);

        private async Task<int> GetPageCount(string query) => TryParseInt((await ParseDocumentFromURL($"http://alpha.wallhaven.cc/search?q={HttpUtility.UrlEncode(query)}"))
            .SelectSingleNode("//section[@class='thumb-listing-page']/header")?.InnerText?.Split(' ')?.LastOrDefault());

        private async Task<int[]> GetImages(string query, int page) => (await ParseDocumentFromURL($"http://alpha.wallhaven.cc/search?q={HttpUtility.UrlEncode(query)}&page={page}"))
            .SelectNodes("//a[@class='preview']").Select(a => int.Parse(a.Attributes["href"].Value.Split('/').Last())).ToArray();

        private string FixScheme(string url) => new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps }.Where(s => s == new Uri(url).Scheme).Count() == 0 ? "http:" + url : url;

        private async Task<string> GetImage(int id) => FixScheme((await ParseDocumentFromURL($"http://alpha.wallhaven.cc/wallpaper/{id}"))
            .SelectSingleNode("//img[@id='wallpaper']").Attributes["src"].Value);

        private async Task MainAsync()
        {
            while (true)
            {
                var pageCount = await GetPageCount(Query);
                if (pageCount == 0)
                {
                    Console.WriteLine("Nobody here but us chickens!");
                    break;
                }
                var images = await GetImages(Query, _random.Next(1, pageCount + 1));
                if (images.Length > 0)
                {
                    var image = images[_random.Next(images.Length)];
                    using (var file = File.Open($"C:\\Wallpapers\\{image}.jpg", FileMode.Create))
                    using (var download = await _http.GetStreamAsync(await GetImage(image)))
                    {
                        await download.CopyToAsync(file);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        }

        static void Main(string[] args) => AsyncContext.Run(() => new Program().MainAsync());
    }
}
