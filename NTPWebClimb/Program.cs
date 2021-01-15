using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;

namespace NTPWebClimb
{
    class Program
    {
        static void Main(string[] args)
        {
            //AsyncContext.Run(() => MainAsync(args));

            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            string baseurl = "https://www.ntp.gov.tw/content/list/";

            string url = "https://www.ntp.gov.tw/content/list/list02.aspx";

            HttpResponseMessage html = await httpClient.GetAsync(url);

            var config = AngleSharp.Configuration.Default.WithXPath();

            StringBuilder stringBuilder = new StringBuilder();

            using (var htmlReaderContext = BrowsingContext.New(config))
            {
                string responseResult = await html.Content.ReadAsStringAsync();

                IDocument pdom = await htmlReaderContext.OpenAsync(res => res.Content(responseResult));

                html.Dispose();

                for (int i = 1; i < 13; i++)
                {
                    string xPath = "*[xpath>'//*[@id=\"main\"]/table/tbody/tr/td/table/tbody/tr[4]/td/table[1]/tbody/tr[2]/td[2]/table/tbody/tr[" + i + "]/td[2]']";

                    var tr = pdom.QuerySelectorAll(xPath);
                    //"*[xpath>'//td[2]'"
                    //var everyP = tr.QuerySelectorAll("xpath>'/td[2]'");

                    foreach (var a in tr.Children("a"))
                    {
                        string Pname = a.TextContent;
                        Console.WriteLine("議員 :" + Pname);


                        string gethref = a.Attributes["href"].Value;
                        Console.WriteLine(gethref);

                        IDocument contentDom;
                        using (HttpClient contentHttpClient = new HttpClient())
                        {
                            using (HttpResponseMessage contentHtml = await contentHttpClient.GetAsync(baseurl + gethref))
                            {
                                //取得內容
                                string contentResponseResult = await contentHtml.Content.ReadAsStringAsync();

                                //增長request間隔時間 避免被誤認為ddos
                                //Thread.Sleep(10);

                                contentDom = await htmlReaderContext.OpenAsync(res => res.Content(contentResponseResult));
                            }
                        }

                        var PPneedData = contentDom.QuerySelector("*[xpath>'//*[@id=\"main\"]/table/tbody/tr/td/table/tbody/tr[1]/td/table/tbody/tr/td[3]/table[2]/tbody/tr[3]/td/table/tbody/tr/td[3]']");

                        string PPName = PPneedData.TextContent;

                        Console.WriteLine(PPName);

                        //"update [dbo].[USER_MAIN] set PPM_ID = '', USER_PHOTO_PATH = ''where UM_ID = ''"
                        var pid = covertName(Pname);
                        var ppid = covertPPM(PPName);
                        stringBuilder.Append("update [dbo].[USER_MAIN] set PPM_ID = '");
                        stringBuilder.Append(ppid);
                        stringBuilder.Append("', USER_PHOTO_PATH = '");
                        stringBuilder.Append(pid + ".jpg");

                        stringBuilder.Append("' where UM_ID = '");
                        stringBuilder.Append(pid + "'");
                        stringBuilder.Append("\r\n");

                        //爬圖片
                        //var imageData = contentDom.QuerySelector("*[xpath>'//*[@id=\"main\"]/table/tbody/tr/td/table/tbody/tr[1]/td/table/tbody/tr/td[2]/img']");

                        //string imagehref = imageData.Attributes["src"].Value;

                        //using (HttpClient contentHttpClient = new HttpClient())
                        //{
                        //    using (HttpResponseMessage response = await contentHttpClient.GetAsync(baseurl + imagehref))
                        //    {
                        //        response.EnsureSuccessStatusCode();
                        //        //圖片
                        //        using (var inputStream = await response.Content.ReadAsStreamAsync())
                        //        {
                        //            using (var imgSave = File.Create("./" + pid + ".jpg"))
                        //            {
                        //                inputStream.Seek(0, SeekOrigin.Begin);
                        //                inputStream.CopyTo(imgSave);
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
                File.WriteAllText("./output.txt", stringBuilder.ToString());
            }
        }

        static string covertPPM(string PPname)
        {
            string[] pp = secretData.ppArray;
            for (int i = 0; i < pp.Length; i++)
            {
                if (pp[i].IndexOf(PPname) > 0)
                {
                    return pp[i].Split(",")[0];
                }
            }

            return "";
        }

        static string covertName(string pName)
        {
            string[] pNameArray = secretData.pNameArray;

            for (int i = 0; i < pNameArray.Length; i++)
            {
                if (pNameArray[i].IndexOf(pName) > 0)
                {
                    return pNameArray[i].Split(",")[0];
                }
            }

            return "";
        }

    }
}
