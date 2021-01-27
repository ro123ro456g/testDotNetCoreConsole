using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace NTPBulletinClimb
{
    class Program
    {
        static List<INFO_MAIN> old_bulletins = new List<INFO_MAIN>();
        static List<Info_Attachment> old_att = new List<Info_Attachment>();
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();

            //建立資料夾
            if (!Directory.Exists("./AttFile"))
                Directory.CreateDirectory("./AttFile");
            if (!Directory.Exists("./AttFile/BULLETIN"))
                Directory.CreateDirectory("./AttFile/BULLETIN");

            string orgNTPDbConnectString = config.GetConnectionString("OriginDatabase");

            StringBuilder outputNote = new StringBuilder();

            using (var conn = new SqlConnection(orgNTPDbConnectString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("select top(30)*from [NTPM].[dbo].[INFO_MAIN]", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MapInfoMainData(reader);
                        }
                    }
                }

                using (var cmd = new SqlCommand("select*from[dbo].[Info_Attachment]", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MapInfoAttachmentData(reader);
                        }
                    }
                }
            }

            //總筆數
            int totalCount = old_bulletins.Count;

            //疑似髒資料 而未轉出的資料 筆數 
            int dirtyCount = 0;
            List<string> dirtyDataId = new List<string>();

            //轉出的公告筆數
            int insertCount = 0;

            //總附件數
            int attachCount = old_att.Count;

            //轉出附件數
            int attachInsertCount = 0;

            //丟失的附件資料
            int lostAttachCount = 0;
            List<string> lostAttachId = new List<string>();

            secretData secretData = new secretData();

            StringBuilder sqlScript = new StringBuilder();
            StringBuilder attachSqlScript = new StringBuilder();


            outputNote.Append("藝文空間、最新消息 類別的公告 不知道是幹嘛用的 原系統找不到對應位置");
            outputNote.Append("\r\n");

            foreach (var rowData in old_bulletins)
            {
                //全部不對外公開的篩掉 不加入新的資料庫 並記錄
                //[H_R_IN]
                //,[H_R_IN_GROUP]
                //,[H_R_IN_PERSONNEL]
                //,[H_R_EF]
                //,[H_R_EF_GROUP]
                //,[H_R_EF_PERSONNEL]
                //,[H_R_NTC]
                //皆為-1
                if (rowData.H_R_IN.Trim() == "-1" &&
                    rowData.H_R_IN_GROUP.Trim() == "-1" &&
                    rowData.H_R_IN_PERSONNEL.Trim() == "-1" &&
                    rowData.H_R_EF.Trim() == "-1" &&
                    //rowData.H_R_EF_GROUP.Trim() == "-1" &&
                    rowData.H_R_EF_PERSONNEL.Trim() == "-1" &&
                    rowData.H_R_NTC == "-1"
                    //最後一項 H_R_IN_PERSONNEL為非數字
                    )
                {
                    //疑似髒資料 額外紀錄
                    dirtyCount++;

                    outputNote.Append("疑似髒資料 未公開的公告 H_ID = ");
                    outputNote.Append(rowData.H_ID);
                    outputNote.Append("\r\n");

                    dirtyDataId.Add(rowData.H_ID);
                    continue;
                }

                int tempParse = 0;
                if (!int.TryParse(rowData.H_R_IN_PERSONNEL.Trim(), out tempParse))
                {
                    //疑似髒資料 額外紀錄
                    dirtyCount++;

                    outputNote.Append("疑似髒資料 H_R_IN_PERSONNEL包含非數字(個人公告?) H_ID = ");
                    outputNote.Append(rowData.H_ID);
                    outputNote.Append("\r\n");

                    dirtyDataId.Add(rowData.H_ID);
                    continue;
                }

                //TODO: STID 會被前面條件挑出來
                //if(rowData.H_ST_ID == "")


                //判斷所屬群組 Dmid
                string oldDmName = converArray(rowData.H_DEP_ID, secretData.orgdepartArray, 1);

                string newDmId = converArray(oldDmName, secretData.newDepartArray, 0);

                #region 可看的權限
                List<string> fmIds = new List<string>();

                //[H_R_IN] = 0 表示會內人員皆可看
                bool NTPVisable = rowData.H_R_IN == "0";

                if (NTPVisable)
                {
                    //會內網站 會內資訊
                    fmIds.Add("cb1b206b-dcb5-4d25-871b-d8a3fad98569");
                }

                //[H_R_EF] = 13 表示議員皆可看
                bool PVisable = rowData.H_R_EF == "13";

                if (PVisable)
                {
                    //議員 會內資訊
                    fmIds.Add("0da244b6-014d-407f-a876-0b7d9b8cf625");
                }

                //共享給某個組室
                if (rowData.H_R_IN_GROUP != "-1")
                {
                    string visbleDepartName = converArray(rowData.H_R_IN_GROUP, secretData.orgdepartArray, 1);

                    //第一審查會 、第一審查委員會 其轉換方法 
                    if (visbleDepartName.Length > 6)
                        visbleDepartName = visbleDepartName.Substring(0, 4);

                    //此為會內 
                    string snmId = converArray("會內網站", secretData.snmArray, 0);

                    //string fmId = converArray(visbleDepartName, secretData.funcArray, 0, snmId);

                    fmIds.Add(converArray(visbleDepartName, secretData.funcArray, 0, snmId));
                    //各組室訊息
                    fmIds.Add("4e44a3da-45b0-4a6b-8e8b-10646052e90b");
                }

                //市府人員 公告
                //H_R_NTC != -1
                if (rowData.H_R_NTC != "-1")
                {
                    //此為市府
                    //string snmId = converArray("市府專區", secretData.snmArray, 0);

                    //string fmId = "ed60223e-54cb-414b-8ed0-21ffbcce0313";
                    fmIds.Add("ed60223e-54cb-414b-8ed0-21ffbcce0313");
                }

                #region BM 生成 Sqlscript
                string umId = "";
                //UmId
                try
                {
                    umId = converArray(rowData.H_USER_ID, secretData.userArray, 0);
                }
                catch
                {//不存在的使用者
                    dirtyCount++;
                    outputNote.Append("疑似髒資料 不存在的使用者 H_ID = ");
                    outputNote.Append(rowData.H_ID);
                    outputNote.Append("\r\n");

                    dirtyDataId.Add(rowData.H_ID);

                    continue;
                }

                //BmId
                string bmId = Guid.NewGuid().ToString();

                sqlScript.Append("INSERT INTO [dbo].[BULLETIN_MAIN] ( [BM_ID], [DM_ID], [UM_ID], [PUBLISH_START_DTM], [PUBLISH_END_DTM], [BULLETIN_TITLE], [BULLETIN_CONTENT], [BULLETIN_TYPE], [IS_ENABLE], [CREATE_USER], [CREATE_DTM], [MODIFY_USER], [MODIFY_DTM], [BULLETIN_ORDER])");
                sqlScript.Append("VALUES ( '");
                sqlScript.Append(bmId);
                sqlScript.Append("', '");
                sqlScript.Append(newDmId);
                sqlScript.Append("', '");
                sqlScript.Append(umId);
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_START_DATE);
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_END_DATE);
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_TITLE);
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_CONTENT);
                sqlScript.Append("', 'BULLETIN");
                sqlScript.Append("', 'Y");
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_USER_NAME.Trim());
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_START_DATE);
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_USER_NAME.Trim());
                sqlScript.Append("', '");
                sqlScript.Append(rowData.H_ALTER_DATE);
                sqlScript.Append("',");
                if (!string.IsNullOrEmpty(rowData.H_SORT))
                {
                    sqlScript.Append("'");
                    sqlScript.Append(rowData.H_SORT);
                    sqlScript.Append("'");
                }
                else
                {
                    sqlScript.Append("NULL");
                    //sqlScript.Append("");
                }

                sqlScript.Append(")");
                sqlScript.Append("\r\n");

                insertCount++;

                #endregion

                #region BP 公告關聯(發佈到哪些科室) SqlScript

                foreach (var fmId in fmIds)
                {
                    string bpId = Guid.NewGuid().ToString();

                    sqlScript.Append("INSERT INTO [dbo].[BULLETIN_PUBLISH] ([BP_ID], [BM_ID], [FM_ID], [CREATE_USER], [CREATE_DTM], [MODIFY_USER],[MODIFY_DTM])");
                    sqlScript.Append("VALUES ( '");
                    sqlScript.Append(bpId);
                    sqlScript.Append("', '");
                    sqlScript.Append(bmId);
                    sqlScript.Append("', '");
                    sqlScript.Append(fmId);
                    sqlScript.Append("', '");
                    sqlScript.Append(rowData.H_USER_NAME.Trim());
                    sqlScript.Append("', '");
                    sqlScript.Append(rowData.H_START_DATE);
                    sqlScript.Append("', '");
                    sqlScript.Append(rowData.H_USER_NAME.Trim());
                    sqlScript.Append("', '");
                    sqlScript.Append(rowData.H_ALTER_DATE);
                    sqlScript.Append("'");


                    sqlScript.Append(")");
                    sqlScript.Append("\r\n");
                }

                #endregion
                #endregion

                string HyperLink = "";
                string HyperLinkTitle = "";
                //HyperLink處理
                if (!string.IsNullOrEmpty(rowData.H_LINK.Trim()))
                {
                    if (rowData.H_LINK.Contains("=h"))
                    {//有帶名
                        var strArray = rowData.H_LINK.Split("=", 2);
                        HyperLinkTitle = strArray[0];
                        HyperLink = strArray[1];
                    }
                    else
                    {
                        HyperLink = HyperLinkTitle = rowData.H_LINK;
                    }
                }

                #region 附件
                List<string> baIds = new List<string>();
                if (old_att.Any(a => a.HCF_H_ID == rowData.H_ID))
                {
                    List<Info_Attachment> listAtts = old_att.Where(a => a.HCF_H_ID == rowData.H_ID).ToList();
                    foreach (var item in listAtts)
                    {
                        float fileLength = 0;
                        string baId = Guid.NewGuid().ToString();
                        string filePath = "";
                        var attDate = DateTime.Parse(item.HCF_FILE_ALTER_DATE).ToString("yyyyMMdd");
                        try
                        {
                            using (HttpClient contentHttpClient = new HttpClient())
                            {
                                using (HttpResponseMessage response = await contentHttpClient.GetAsync(item.HCF_FILE_PATH))
                                {
                                    response.EnsureSuccessStatusCode();
                                    //圖片
                                    using (var inputStream = await response.Content.ReadAsStreamAsync())
                                    {
                                        if (!Directory.Exists("./AttFile/BULLETIN/" + attDate))
                                            Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate);
                                        if (!Directory.Exists("./AttFile/BULLETIN/" + attDate + "/" + bmId))
                                            Directory.CreateDirectory("./AttFile/BULLETIN/" + attDate + "/" + bmId);

                                        filePath = "BULLETIN/" + attDate + "/" + bmId + "/";

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
                        }
                        catch
                        {
                            //附件失效

                            outputNote.Append("公告附件 檔案抓取失敗 對應 HCF_ID = ");
                            outputNote.Append(item.HCF_ID);
                            outputNote.Append("\r\n");

                            lostAttachCount++;
                            lostAttachId.Add(item.HCF_ID);

                            continue;
                        }

                        #region Attach Sqlscript

                        var filenameIndex = item.HCF_NAME.LastIndexOf('.');

                        sqlScript.Append("INSERT INTO [dbo].[BULLETIN_ATTCH] ([BA_ID], [BM_ID], [FILEPATH], [ORIGIN_FILENAME], [FILENAME], [FILENAME_EXT], [CREATE_USER], [CREATE_DTM], [MODIFY_USER], [MODIFY_DTM], [FILE_SIZE])");
                        sqlScript.Append("VALUES ( '");
                        sqlScript.Append(baId);
                        sqlScript.Append("', '");
                        sqlScript.Append(bmId);
                        sqlScript.Append("', '");
                        sqlScript.Append(filePath);
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_NAME.Substring(0, filenameIndex));
                        sqlScript.Append("', '");
                        sqlScript.Append(baId);
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_NAME.Substring(filenameIndex + 1));
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_USER_NAME);
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_ALTER_DATE);
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_FILE_USER_NAME);
                        sqlScript.Append("', '");
                        sqlScript.Append(item.HCF_FILE_ALTER_DATE);
                        sqlScript.Append("', '");
                        sqlScript.Append(fileLength.ToString("f2"));
                        sqlScript.Append("'");

                        sqlScript.Append(")");
                        sqlScript.Append("\r\n");

                        attachInsertCount++;
                        #endregion
                    }
                }


                #endregion


                Console.WriteLine(rowData.H_ID);
            }

            //因髒資料而被排除的公告 的附件
            var lastLostAtt = old_att.Where(a => dirtyDataId.Any(b => b == a.HCF_H_ID)).ToList();

            lostAttachCount += lastLostAtt.Count;
            lostAttachId.AddRange(lastLostAtt.Select(a => a.HCF_H_ID));

            //輸出SqlScript
            File.WriteAllText("./sqlScript.txt", sqlScript.ToString());


            //輸出報告
            //總筆數
            outputNote.Append("公告總共筆數為 : ");
            outputNote.Append(totalCount);
            outputNote.Append("\r\n");
            //轉出的公告筆數
            outputNote.Append("公告輸出筆數為 : ");
            outputNote.Append(insertCount);
            outputNote.Append("\r\n");

            //疑似髒資料 而未轉出的資料 筆數
            outputNote.Append("公告疑似髒資料 而未轉出的資料 筆數 : ");
            outputNote.Append(dirtyCount);
            outputNote.Append("\r\n");

            //附件數
            outputNote.Append("附件 總筆數 : ");
            outputNote.Append(attachCount);
            outputNote.Append("\r\n");

            //轉出附件數
            outputNote.Append("轉出附件 筆數 : ");
            outputNote.Append(attachInsertCount);
            outputNote.Append("\r\n");

            //丟失的附件資料 + 因髒資料而被排除的公告 的附件
            outputNote.Append("丟失的附件資料 + 因髒資料而被排除的公告 的附件 ");
            outputNote.Append(lostAttachCount);
            outputNote.Append("\r\n");

            File.WriteAllText("./LogReport.txt", outputNote.ToString());
        }

        private static void MapInfoMainData(SqlDataReader reader)
        {
            INFO_MAIN iNFO_MAIN = new INFO_MAIN();
            Type infoMainType = typeof(INFO_MAIN);

            foreach (var property in infoMainType.GetProperties())
            {
                string pName = property.Name;
                if (reader[pName] is DateTime)
                {
                    infoMainType.GetProperty(pName).SetValue(iNFO_MAIN, ((DateTime)reader[pName]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
                else
                {
                    infoMainType.GetProperty(pName).SetValue(iNFO_MAIN, reader[pName].ToString());
                }
            }

            old_bulletins.Add(iNFO_MAIN);
        }

        private static void MapInfoAttachmentData(SqlDataReader reader)
        {
            Info_Attachment info_Attachment = new Info_Attachment();
            Type infoAttachmentType = typeof(Info_Attachment);

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

            old_att.Add(info_Attachment);
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
