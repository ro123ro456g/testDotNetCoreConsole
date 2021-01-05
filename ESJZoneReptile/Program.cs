using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace ESJZoneReptile
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            string url = "https://www.esjzone.cc/detail/1579237880.html";

            Task<HttpResponseMessage> asyncHtml = httpClient.GetAsync(url);

            Task.WaitAll();

            HttpResponseMessage html = asyncHtml.Result;

            if (html.StatusCode == HttpStatusCode.OK)
            {
                var config = AngleSharp.Configuration.Default;
                using (var context = BrowsingContext.New(config))
                {
                    string responseResult = html.Content.ReadAsStringAsync().Result;//取得內容

                    Task<IDocument> loadDom = context.OpenAsync(res => res.Content(responseResult));

                    Task.WaitAll();

                    IDocument dom = loadDom.Result;

                    IHtmlCollection<IElement> chapterlist = dom.QuerySelectorAll("#chapterList");

                    //chapterlist.Children("a");

                    foreach (IElement a in chapterlist.Children("a"))
                    {
                        //chapterlist.Children();

                        //讀取a元素(超聯結) 底下全部的字
                        Console.Write(a.TextContent);

                        Console.Write(a.Attributes["href"].Value);
                        Console.WriteLine();

                        //foreach (var b in a.ele)
                        //{
                        //    Console.Write(b.TextContent);

                        //    Console.Write(b.Attributes["href"]);
                        //    Console.WriteLine();

                        //    if (b.TagName == "a")
                        //    {
                                
                        //    }
                        //}

                        
                    }

                }

            }

            //Console.WriteLine("Hello World!");
        }
    }
}
