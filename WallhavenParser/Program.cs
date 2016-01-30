using HtmlAgilityPack;
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

        private async Task<int> GetPageCount(string query)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(await _http.GetStringAsync($"http://alpha.wallhaven.cc/search?q={HttpUtility.UrlEncode(query)}"));
            var pageCountString = doc.DocumentNode.SelectSingleNode("//section[@class='thumb-listing-page']/header")?.InnerText?.Split(' ')?.LastOrDefault();
            return pageCountString == null ? 0 : int.Parse(pageCountString);
        }

        private async Task<int[]> GetImages(string query, int page)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(await _http.GetStringAsync($"http://alpha.wallhaven.cc/search?q={HttpUtility.UrlEncode(query)}&page={page}"));
            return doc.DocumentNode.SelectNodes("//a[@class='preview']").Select(a => int.Parse(a.Attributes["href"].Value.Split('/').Last())).ToArray();
        }

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
                    using (var download = await _http.GetStreamAsync($"http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-{image}.jpg"))
                    {
                        await download.CopyToAsync(file);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        }

        static void Main(string[] args)
        {
            AsyncContext.Run(() => new Program().MainAsync());
        }
    }
}
