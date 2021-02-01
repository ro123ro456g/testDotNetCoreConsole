using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadNTPAttach
{
    class Program
    {
        static readonly HttpClient Diversion0 = new HttpClient();
        static readonly HttpClient Diversion1 = new HttpClient();
        static readonly HttpClient Diversion2 = new HttpClient();
        static readonly HttpClient Diversion3 = new HttpClient();
        static readonly HttpClient Diversion4 = new HttpClient();
        static readonly HttpClient Diversion5 = new HttpClient();
        static readonly HttpClient Diversion6 = new HttpClient();
        static readonly HttpClient Diversion7 = new HttpClient();
        static readonly HttpClient Diversion8 = new HttpClient();
        static readonly HttpClient Diversion9 = new HttpClient();
        static HttpClient[] httpClients = new HttpClient[]
        {
            Diversion0,
            Diversion1,
            Diversion2,
            Diversion3,
            Diversion4,
            Diversion5,
            Diversion6,
            Diversion7,
            Diversion8,
            Diversion9
        };
        static string httpClient = "Diversion";
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            DateTime processStartTime = DateTime.Now;

            secretData secretData = new secretData();

            //轉失敗
            ConcurrentBag<string> lostAttachId = new ConcurrentBag<string>();

            List<Bulletin_Attach> bulletinAttachs = Bulletin_Attach.MapBulletinAttach(secretData.attachUrlList);

            StringBuilder sqlScript = new StringBuilder();

            StringBuilder reportNote = new StringBuilder();

            ConcurrentBag<StringBuilder> tempReportNote = new ConcurrentBag<StringBuilder>();

            ConcurrentBag<StringBuilder> tempSqlScript = new ConcurrentBag<StringBuilder>();

            //轉出數量
            int attachInsertCount = 0;


            var parallelLoop = Parallel.ForEach(bulletinAttachs, (bulletinAttach) =>
            {
                int retryCount = 0;
                HttpClient currentHttpClient = httpClients[retryCount];
                float fileLength = 0;
                string baId = bulletinAttach.BA_ID;
                string filePath = "";
                var attDate = DateTime.Parse(bulletinAttach.MODIFY_DTM).ToString("yyyyMMdd");
                try
                {
                    var httpclientsync = currentHttpClient.GetAsync(bulletinAttach.url);
                    Task.WaitAny(httpclientsync);
                    using (HttpResponseMessage response = httpclientsync.Result)
                    {
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine($"附件開始下載 {bulletinAttach.BA_ID} , {bulletinAttach.HCF_ID}");
                        //圖片
                        var readStreamsync = response.Content.ReadAsStreamAsync();
                        Task.WaitAny(readStreamsync);
                        using (var inputStream = readStreamsync.Result)
                        {
                            if (!Directory.Exists("./AttFile/BULLETIN/" + attDate))
                                Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate);
                            if (!Directory.Exists("./AttFile/BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID))
                                Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID);

                            filePath = "BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID + "/";

                            using (var fileSave = File.Create("./AttFile/" + filePath + baId))
                            {
                                inputStream.Seek(0, SeekOrigin.Begin);
                                inputStream.CopyTo(fileSave);
                                fileLength = fileSave.Length / 1024 / 1024;
                                if (fileLength < 0.01)
                                    fileLength = 0.01f;
                            }
                        }
                    }
                }
                catch 
                {
                Retry:
                    if (retryCount < 9)
                    {//切換分流重試
                        retryCount++;
                        Console.WriteLine($"附件下載失敗 重新嘗試第{retryCount + 1}次 {bulletinAttach.BA_ID} , {bulletinAttach.HCF_ID}");
                        currentHttpClient = httpClients[retryCount];
                        try
                        {
                            var httpclientsync = currentHttpClient.GetAsync(bulletinAttach.url);
                            Task.WaitAny(httpclientsync);
                            using (HttpResponseMessage response = httpclientsync.Result)
                            {
                                response.EnsureSuccessStatusCode();
                                Console.WriteLine($"附件開始下載 {bulletinAttach.BA_ID} , {bulletinAttach.HCF_ID}");
                                //圖片
                                var readStreamsync = response.Content.ReadAsStreamAsync();
                                Task.WaitAny(readStreamsync);
                                using (var inputStream = readStreamsync.Result)
                                {
                                    if (!Directory.Exists("./AttFile/BULLETIN/" + attDate))
                                        Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate);
                                    if (!Directory.Exists("./AttFile/BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID))
                                        Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID);

                                    filePath = "BULLETIN/" + attDate + "/" + bulletinAttach.BM_ID + "/";

                                    using (var fileSave = File.Create("./AttFile/" + filePath + baId))
                                    {
                                        inputStream.Seek(0, SeekOrigin.Begin);
                                        inputStream.CopyTo(fileSave);
                                        fileLength = fileSave.Length / 1024 / 1024;
                                        if (fileLength < 0.01)
                                            fileLength = 0.01f;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            goto Retry;
                        }
                    }
                    else
                    {
                        //附件失效
                        string a = $"公告附件 檔案抓取失敗 對應 HCF_ID = {bulletinAttach.HCF_ID}\r\n";
                        tempReportNote.Add(new StringBuilder(a));
                        lostAttachId.Add(bulletinAttach.HCF_ID);

                        return;
                    }
                }

                #region Attach Sqlscript

                //var filenameIndex = bulletinAttach.HCF_NAME.LastIndexOf('.');
                Console.WriteLine($"附件下載成功 {bulletinAttach.BA_ID} , {bulletinAttach.HCF_ID}");
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("INSERT INTO [dbo].[BULLETIN_ATTCH] ([BA_ID], [BM_ID], [FILEPATH], [ORIGIN_FILENAME], [FILENAME], [FILENAME_EXT], [CREATE_USER], [CREATE_DTM], [MODIFY_USER], [MODIFY_DTM], [FILE_SIZE])");
                stringBuilder.Append("VALUES ( '");
                stringBuilder.Append(baId);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.BM_ID);
                stringBuilder.Append("', '");
                stringBuilder.Append(filePath);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.ORIGIN_FILENAME);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.FILEPATH);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.FILENAME_EXT);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.CREATE_USER);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.CREATE_DTM);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.MODIFY_USER);
                stringBuilder.Append("', '");
                stringBuilder.Append(bulletinAttach.MODIFY_DTM);
                stringBuilder.Append("', '");
                stringBuilder.Append(fileLength.ToString("f2"));
                stringBuilder.Append("'");

                stringBuilder.Append(")");
                stringBuilder.Append("\r\n");

                tempSqlScript.Add(stringBuilder);

                #endregion
            });

            Task.WaitAll();
            while (!parallelLoop.IsCompleted)
            {
                Thread.Sleep(10);
            }

            foreach (var str in tempSqlScript)
            {
                sqlScript.Append(str);
                attachInsertCount++;
            }


            File.WriteAllText("./sqlScript.txt", sqlScript.ToString());

            foreach(var str in tempReportNote)
            {
                reportNote.Append(str);
            }

            var attachCount = bulletinAttachs.Count;

            //附件數
            reportNote.Append("附件 總筆數 : ");
            reportNote.Append(attachCount);
            reportNote.Append("\r\n");

            reportNote.Append("轉出附件 筆數 : ");
            reportNote.Append(attachInsertCount);
            reportNote.Append("\r\n");

            //丟失的附件資料 
            var lostAttachCount = lostAttachId.Count;
            reportNote.Append("丟失的附件資料 (下載不到) :  ");
            reportNote.Append(lostAttachCount);
            reportNote.Append("\r\n");


            var a = (DateTime.Now - processStartTime);
            reportNote.Append("程式執行時間 未包含附件的下載");
            reportNote.Append(a.Hours + ":" + a.Minutes + ":" + a.Seconds);
            reportNote.Append(" 秒");
            reportNote.Append("\r\n");


            File.WriteAllText("./LogReport.txt", reportNote.ToString());
        }
    }
}
