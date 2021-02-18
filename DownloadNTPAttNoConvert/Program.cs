using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DownloadNTPAttNoConvert.Bulletin_Attach;

namespace DownloadNTPAttNoConvert
{
    class Program
    {
        //static List<INFO_MAIN> old_bulletins = new List<INFO_MAIN>();
        static List<Info_AttMain> listAtt = new List<Info_AttMain>();
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
        static readonly HttpClient Diversion10 = new HttpClient();
        static readonly HttpClient Diversion11 = new HttpClient();
        static readonly HttpClient Diversion12 = new HttpClient();
        static readonly HttpClient Diversion13 = new HttpClient();
        static readonly HttpClient Diversion14 = new HttpClient();
        static readonly HttpClient Diversion15 = new HttpClient();
        static readonly HttpClient Diversion16 = new HttpClient();
        static readonly HttpClient Diversion17 = new HttpClient();
        static readonly HttpClient Diversion18 = new HttpClient();
        static readonly HttpClient Diversion19 = new HttpClient();
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
            Diversion9,
            Diversion10,
            Diversion11,
            Diversion12,
            Diversion13,
            Diversion14,
            Diversion15,
            Diversion16,
            Diversion17,
            Diversion18,
            Diversion19,
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


            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();

            //建立資料夾
            if (!Directory.Exists("./AttFile"))
                Directory.CreateDirectory("./AttFile");

            string orgNTPDbConnectString = config.GetConnectionString("OriginDatabase");

            //抓取復健url (db)
            using (var conn = new SqlConnection(orgNTPDbConnectString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(@$"select ia.HCF_FILE_PATH, ia.HCF_NAME , im.H_TITLE , d.DEP_Name
                                                        from[dbo].[Info_Attachment] as ia
                                                        join[dbo].[INFO_MAIN] as im

                                                            on ia.HCF_H_ID = im.H_ID
                                                        join[dbo].[Department] as d

                                                            on d.DEP_ID = im.H_DEP_ID
                                                        where im.H_ST_ID = 4
                                                        and ia.HCF_NAME != ''", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MapInfoAttData(reader);
                        }
                    }
                }
            }


            //轉失敗
            ConcurrentBag<string> lostAttachId = new ConcurrentBag<string>();

            StringBuilder sqlScript = new StringBuilder();

            StringBuilder reportNote = new StringBuilder();

            ConcurrentBag<StringBuilder> tempReportNote = new ConcurrentBag<StringBuilder>();

            ConcurrentBag<StringBuilder> tempSqlScript = new ConcurrentBag<StringBuilder>();

            //轉出數量
            int attachInsertCount = 0;


            var parallelLoop = Parallel.ForEach(listAtt, (bulletinAttach) =>
            {
                int retryCount = 0;
                HttpClient currentHttpClient = httpClients[retryCount];
                //string baId = bulletinAttach.HCF_NAME;
                string filePath = "";
                try
                {
                    var httpclientsync = currentHttpClient.GetAsync(bulletinAttach.HCF_FILE_PATH);
                    Task.WaitAny(httpclientsync);
                    using (HttpResponseMessage response = httpclientsync.Result)
                    {
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine($"附件開始下載 {bulletinAttach.HCF_NAME} ");
                        //圖片
                        var readStreamsync = response.Content.ReadAsStreamAsync();
                        Task.WaitAny(readStreamsync);
                        using (var inputStream = readStreamsync.Result)
                        {
                            if (!Directory.Exists("./AttFile/" + bulletinAttach.DEP_Name))
                                Directory.CreateDirectory("./AttFile/" + bulletinAttach.DEP_Name);
                            if (!Directory.Exists("./AttFile/" + bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE))
                                Directory.CreateDirectory("./AttFile/" + bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE);

                            filePath = bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE + "/";

                            using (var fileSave = File.Create("./AttFile/" + filePath + bulletinAttach.HCF_NAME))
                            {
                                inputStream.Seek(0, SeekOrigin.Begin);
                                inputStream.CopyTo(fileSave);
                            }
                        }
                    }
                }
                catch
                {
                Retry:
                    if (retryCount < 19)
                    {//切換分流重試
                        retryCount++;
                        Console.WriteLine($"附件下載失敗 重新嘗試第{retryCount + 1}次 {bulletinAttach.HCF_NAME}");
                        currentHttpClient = httpClients[retryCount];
                        try
                        {
                            var httpclientsync = currentHttpClient.GetAsync(bulletinAttach.HCF_FILE_PATH);
                            Task.WaitAny(httpclientsync);
                            using (HttpResponseMessage response = httpclientsync.Result)
                            {
                                response.EnsureSuccessStatusCode();
                                Console.WriteLine($"附件開始下載 {bulletinAttach.HCF_NAME}");
                                //圖片
                                var readStreamsync = response.Content.ReadAsStreamAsync();
                                Task.WaitAny(readStreamsync);
                                using (var inputStream = readStreamsync.Result)
                                {
                                    if (!Directory.Exists("./AttFile/" + bulletinAttach.DEP_Name))
                                        Directory.CreateDirectory("./AttFile/" + bulletinAttach.DEP_Name);
                                    if (!Directory.Exists("./AttFile/" + bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE))
                                        Directory.CreateDirectory("./AttFile/" + bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE);

                                    filePath = bulletinAttach.DEP_Name + "/" + bulletinAttach.H_TITLE + "/";

                                    using (var fileSave = File.Create("./AttFile/" + filePath + bulletinAttach.HCF_NAME))
                                    {
                                        inputStream.Seek(0, SeekOrigin.Begin);
                                        inputStream.CopyTo(fileSave);
                                    }
                                }
                            }
                            goto RetrySuccess;
                        }
                        catch
                        {
                            goto Retry;
                        }
                    }
                    else
                    {
                        //附件失效
                        string a = $"公告附件 檔案抓取失敗 對應 {bulletinAttach.HCF_NAME}\r\n";
                        tempReportNote.Add(new StringBuilder(a));
                        //lostAttachId.Add(bulletinAttach.HCF_ID);

                        return;
                    }
                }

            RetrySuccess:
                #region Attach Sqlscript

                //var filenameIndex = bulletinAttach.HCF_NAME.LastIndexOf('.');
                Console.WriteLine($"附件下載成功 {bulletinAttach.HCF_NAME}");
                //StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.Append("INSERT INTO [dbo].[BULLETIN_ATTCH] ([BA_ID], [BM_ID], [FILEPATH], [ORIGIN_FILENAME], [FILENAME], [FILENAME_EXT], [CREATE_USER], [CREATE_DTM], [MODIFY_USER], [MODIFY_DTM], [FILE_SIZE])");
                //stringBuilder.Append("VALUES ( '");
                //stringBuilder.Append(baId);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.BM_ID);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(filePath);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.ORIGIN_FILENAME);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.FILEPATH);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.FILENAME_EXT);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.CREATE_USER);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.CREATE_DTM);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.MODIFY_USER);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(bulletinAttach.MODIFY_DTM);
                //stringBuilder.Append("', '");
                //stringBuilder.Append(fileLength.ToString("f2"));
                //stringBuilder.Append("'");

                //stringBuilder.Append(")");
                //stringBuilder.Append("\r\n");

                //tempSqlScript.Add(stringBuilder);

                #endregion
            });

            Task.WaitAll();
            while (!parallelLoop.IsCompleted)
            {
                Thread.Sleep(10);
            }

            //foreach (var str in tempSqlScript)
            //{
            //    sqlScript.Append(str);
            //    attachInsertCount++;
            //}


            //File.WriteAllText("./ATTsqlScript.txt", sqlScript.ToString());

            foreach (var str in tempReportNote)
            {
                reportNote.Append(str);
            }

            ////附件數
            //reportNote.Append("附件 總筆數 : ");
            //reportNote.Append(attachCount);
            //reportNote.Append("\r\n");

            //reportNote.Append("轉出附件 筆數 : ");
            //reportNote.Append(attachInsertCount);
            //reportNote.Append("\r\n");

            ////丟失的附件資料 
            //var lostAttachCount = lostAttachId.Count;
            //reportNote.Append("丟失的附件資料 (下載不到) :  ");
            //reportNote.Append(lostAttachCount);
            //reportNote.Append("\r\n");


            var a = (DateTime.Now - processStartTime);
            reportNote.Append("\r\n");
            reportNote.Append("程式執行時間 未包含附件的下載");
            reportNote.Append(a.Hours + ":" + a.Minutes + ":" + a.Seconds);
            reportNote.Append(" 秒");
            reportNote.Append("\r\n");


            File.WriteAllText("./ATTLogReport.txt", reportNote.ToString());
        }

        private static void MapInfoAttData(SqlDataReader reader)
        {
            Info_AttMain info_Attachment = new Info_AttMain();
            Type infoAttachmentType = typeof(Info_AttMain);

            foreach (var property in infoAttachmentType.GetProperties())
            {
                string pName = property.Name;
                if (reader[pName] is DateTime)
                {
                    infoAttachmentType.GetProperty(pName).SetValue(info_Attachment, ((DateTime)reader[pName]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else
                {
                    infoAttachmentType.GetProperty(pName).SetValue(info_Attachment, reader[pName].ToString());
                }
            }

            listAtt.Add(info_Attachment);
        }

        private static string converArray(string target, string[] array, int needIndex, params string[] otherParam)
        {
            if (otherParam.Length == 0)
            {
                foreach (var item in array)
                {
                    if (item.Contains(target))
                    {
                        return item.Split(",")[needIndex];
                    }
                }
            }
            else
            {
                foreach (var item in array)
                {
                    if (item.Contains(target) && item.Contains(otherParam[0]))
                    {
                        return item.Split(",")[needIndex];
                    }
                }
            }

            throw new Exception("找不到群組");
        }
    }
}
