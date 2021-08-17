using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Configuration;

namespace ESJZoneReptile
{
    class Program
    {
        static Microsoft.Extensions.Configuration.IConfiguration Config { get; set; }

        static void Main(string[] args)
        {
            //TODO: 增加讀取json或是 console.Read 輸入url、輸出txt路徑、產生類型設定 (ex:全章節輸出一檔或每章節輸出一檔)

            //AsyncContext.Run(() => MainAsync(args));

            LoadConfig();

            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            //爬單一小說模式
            if (bool.Parse(Config["IsSingleMode"]))
            {

                HttpClient httpClient = new HttpClient();

                //string url = "https://www.esjzone.cc/detail/1579237880.html"; //惡役千金淪為庶民
                //string url = "https://www.esjzone.cc/detail/1546106901.html"; //想自由生活的魔王默示录
                //string url = "https://www.esjzone.cc/detail/1543301718.html"; //轉生成魔劍
                //string url = "https://www.esjzone.cc/detail/1544266666.html"; //無職的英雄
                //string url = "https://www.esjzone.cc/detail/1545207233.html"; //秒殺外掛太強了，異世界的傢伙們根本就不是對手
                //string url = "https://www.esjzone.cc/detail/1550308150.html"; //討厭第四次的死屬性魔術師
                //string url = "https://www.esjzone.cc/detail/1584472487.html"; //輪迴第7次的反派千金，在前敵國盡嘗自由自在的新娘生活
                //string url = "https://www.esjzone.cc/detail/1585997294.html";   //一覺醒來坐擁神裝和飛船，我決定以買一套獨門獨戶的房子為目標作為傭兵自由地活下去
                //string url = "https://www.esjzone.cc/detail/1543764686.html";   //進化之實踏上勝利的人生
                //string url = "https://www.esjzone.cc/detail/1599936477.html";   //我的母親是被流放的原反派千金
                //string url = "https://www.esjzone.cc/detail/1546060392.html";   //萬年d等級的中年冒險者
                //string url = "https://www.esjzone.cc/detail/1550158003.html";   //轉生賢者的異世界生活～取得副職業並成為世界最強
                //string url = "https://www.esjzone.cc/detail/1590909027.html";   //最弱的馴養師開啟的撿垃圾的旅途
                //string url = "https://www.esjzone.cc/detail/1578765913.html";   //輪迴轉生物語
                //string url = "https://www.esjzone.cc/detail/1543303989.html";   //為了扭轉沒落命運，邁向鍛冶工匠之路
                //string url = "https://www.esjzone.cc/detail/1609771641.html";   //萬能「村莊製作」外掛輕鬆慢生活　～這是村子，怎麼了？～
                //string url = "https://www.esjzone.cc/detail/1606545947.html";   //轉生者才能駕馭的極限天賦
                //string url = "https://www.esjzone.cc/detail/1617172474.html";   //借彈丸以魔眼擊穿這異世界！
                //string url = "https://www.esjzone.cc/detail/1543989242.html";   //以壓倒性的運氣在異世界成名
                //string url = "https://www.esjzone.cc/detail/1591721210.html";   //地獄模式~喜歡深入遊戲的玩家在高難度的異世界中無雙~
                string url = "https://www.esjzone.cc/detail/1619964712.html";   //轉生貴族胸懷大志

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
                            stringBuilder.Append("[[");
                            stringBuilder.Append(index);
                            stringBuilder.Append("]]");
                            stringBuilder.Append(" ");
                            //內文已有標題則此處不在加標題
                            stringBuilder.Append(chapterTitle);
                            stringBuilder.Append("\r\n\r\n");

                            string chapterHyperLink = element_A_Tag.Attributes["href"].Value;

                            if (!chapterHyperLink.Contains("esjzone"))
                            {
                                stringBuilder.Append("此章非esjzone站內文章 爬取失敗");
                                stringBuilder.Append("\r\n");

                                index++;
                                continue;
                            }

                            Console.Write(chapterHyperLink);
                            Console.WriteLine();

                            #region 小說內文

                            IDocument contentDom;
                            using (HttpResponseMessage contentHtml = await httpClient.GetAsync(chapterHyperLink))
                            {
                                //取得內容
                                string contentResponseResult = await contentHtml.Content.ReadAsStringAsync();

                                //增長request間隔時間 避免被誤認為ddos
                                //Thread.Sleep(1000);

                                //內文在 .forum-content children <p>裡面
                                contentDom = await htmlReaderContext.OpenAsync(res => res.Content(contentResponseResult));
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

            }
            else
            {
                HttpClient httpClient = new HttpClient();

                string[] urls = Config["Urls"].Split(",");

                foreach (string listUrl in urls)
                {
                    HttpResponseMessage html = await httpClient.GetAsync(listUrl);

                    if (html.StatusCode == HttpStatusCode.OK)
                    {
                        var config = AngleSharp.Configuration.Default;
                        using (var htmlReaderContext = BrowsingContext.New(config))
                        {
                            //取得內容
                            string responseResult = await html.Content.ReadAsStringAsync();

                            IDocument novelListDom = await htmlReaderContext.OpenAsync(res => res.Content(responseResult));

                            //釋放節省記憶體?
                            html.Dispose();

                            string thisNovelElement = string.Empty;

                            //小說那行超連結
                            string novelBlock = "section.container>div.row>div.col-xl-9.col-lg-8.p-r-30>div.row>div.col-lg-3.col-md-4.col-sm-3.col-xs-6>";
                            string novelcard = "div.card.mb-30>div.card-body>h5.card-title>a";

                            IHtmlCollection<IElement> novelsEle = novelListDom.QuerySelectorAll(novelBlock + novelcard);

                            bool continueFlag = false;

                            foreach (var novel in novelsEle)
                            {
                                Thread.Sleep(300);

                                string novelName = novel.TextContent;
                                string novelurl = Config["DomainUrl"] + novel.Attributes["href"].Value;

                                //繼續
                                if (!continueFlag)
                                {
                                    if (!novelName.Contains("在現代社會成為乙女遊戲的惡役千金有點不容易"))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        continueFlag = true;
                                    }
                                }

                                Console.WriteLine(novelName);
                                Console.WriteLine();

                                StringBuilder stringBuilder = new StringBuilder();

                                try
                                {
                                    IDocument chapterDom;
                                    using (HttpResponseMessage novelHtml = await httpClient.GetAsync(novelurl))
                                    {
                                        string novelPageResponse = await novelHtml.Content.ReadAsStringAsync();

                                        chapterDom = await htmlReaderContext.OpenAsync(res => res.Content(novelPageResponse));


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
                                            stringBuilder.Append("[[");
                                            stringBuilder.Append(index);
                                            stringBuilder.Append("]]");
                                            stringBuilder.Append(" ");
                                            //內文已有標題則此處不在加標題
                                            stringBuilder.Append(chapterTitle);
                                            stringBuilder.Append("\r\n\r\n");

                                            string chapterHyperLink = element_A_Tag.Attributes["href"].Value;

                                            if (!chapterHyperLink.Contains("esjzone"))
                                            {
                                                stringBuilder.Append("此章非esjzone站內文章 爬取失敗");
                                                stringBuilder.Append("\r\n");

                                                index++;
                                                continue;
                                            }

                                            Console.Write(chapterHyperLink);
                                            Console.WriteLine();

                                            #region 小說內文

                                            IDocument contentDom;
                                            using (HttpResponseMessage contentHtml = await httpClient.GetAsync(chapterHyperLink))
                                            {
                                                //取得內容
                                                string contentResponseResult = await contentHtml.Content.ReadAsStringAsync();

                                                //增長request間隔時間 避免被誤認為ddos
                                                Thread.Sleep(300);

                                                //內文在 .forum-content children <p>裡面
                                                contentDom = await htmlReaderContext.OpenAsync(res => res.Content(contentResponseResult));
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

                                        SaveFile(novelName, stringBuilder);
                                        //Directory.CreateDirectory("./" + novelName);
                                        //FileUtil.WriteFile("./" + novelName + "/" + novelName + " " + index + ".txt", stringBuilder.ToString());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SaveFile(novelName, stringBuilder);

                                    Console.WriteLine(novelName + "爬取失敗");
                                    Console.WriteLine(ex.Message);
                                    continue;
                                }

                            }

                        }
                    }
                }

            }
        }

        private static void SaveFile(string novelName, StringBuilder stringBuilder)
        {
            try
            {
                Directory.CreateDirectory("./" + novelName);
                FileUtil.WriteFile("./" + novelName + "/" + novelName + " 抓取未完成.txt", stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                if (novelName.Length > 1)
                {
                    string tempName = novelName.Substring(0, novelName.Length - 2);
                    SaveFile(tempName, stringBuilder);
                }
                else
                {
                    Console.WriteLine(novelName + "已有小說內容因檔案存取失敗遺失");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void LoadConfig()
        {
            Config = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .Build();

            //config.GetSection("path").Bind(new object());
            //string a = config["SampleReportPath"];
        }
    }
}
