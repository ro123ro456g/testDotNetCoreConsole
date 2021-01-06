using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace ESJZoneReptile
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: 增加讀取json或是 console.Read 輸入url、輸出txt路徑、產生類型設定 (ex:全章節輸出一檔或每章節輸出一檔)

            //AsyncContext.Run(() => MainAsync(args));

            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            string url = "https://www.esjzone.cc/detail/1579237880.html";

            HttpResponseMessage html = await httpClient.GetAsync(url);
            
            if (html.StatusCode == HttpStatusCode.OK)
            {
                var config = AngleSharp.Configuration.Default;
                using (var htmlReaderContext = BrowsingContext.New(config))
                {
                    //取得內容
                    string responseResult = await html.Content.ReadAsStringAsync();

                    IDocument chapterDom = await htmlReaderContext.OpenAsync(res => res.Content(responseResult));

                    //釋放節省記憶體?
                    html.Dispose();

                    StringBuilder stringBuilder = new StringBuilder();


                    IHtmlCollection<IElement> bookTitle = chapterDom.QuerySelectorAll("h2");

                    stringBuilder.Append(bookTitle[0].TextContent);
                    stringBuilder.Append("\r\n\r\n");

                    IHtmlCollection<IElement> chapterlist = chapterDom.QuerySelectorAll("#chapterList");
                    int index = 1;
                    foreach (IElement element_A_Tag in chapterlist.Children("a"))
                    {
                        //chapterlist.Children();

                        //讀取a元素(超聯結) 底下全部的字
                        Console.Write(index + " ");

                        string chapterTitle = element_A_Tag.TextContent;
                        Console.Write(chapterTitle);
                        stringBuilder.Append(index);
                        stringBuilder.Append(" ");
                        //內文已有標題則此處不在加標題
                        //stringBuilder.Append(chapterTitle);
                        //stringBuilder.Append("\r\n\r\n");

                        string chapterHyperLink = element_A_Tag.Attributes["href"].Value;

                        Console.Write(chapterHyperLink);
                        Console.WriteLine();

                        #region 小說內文

                        IDocument contentDom;
                        using (HttpClient contentHttpClient = new HttpClient())
                        {
                            using (HttpResponseMessage contentHtml = await contentHttpClient.GetAsync(chapterHyperLink))
                            {
                                //取得內容
                                string contentResponseResult = await contentHtml.Content.ReadAsStringAsync();

                                //增長request間隔時間 避免被誤認為ddos
                                Thread.Sleep(200);

                                //內文在 .forum-content children <p>裡面
                                contentDom = await htmlReaderContext.OpenAsync(res => res.Content(contentResponseResult));
                            }
                        }

                        IHtmlCollection<IElement> contentText = contentDom.QuerySelectorAll(".forum-content");

                        foreach (IElement content_P_Element in contentText.Children("p"))
                        {
                            string strContent = content_P_Element.TextContent;
                            if (strContent.Length > 0 && !string.IsNullOrEmpty(strContent))
                            {
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append(strContent);
                                stringBuilder.Append("\r\n");
                            }
                        }
                        #endregion

                        index++;
                    }
                    FileUtil.WriteFile("./Output.txt", stringBuilder.ToString());
                }
            }

            //Console.WriteLine("Hello World!");
        }

    }
}
