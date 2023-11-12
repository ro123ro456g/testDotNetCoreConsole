using CoreTestConsoleApp.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Management;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CoreTestConsoleApp
{
    class Program
    {
        static string pd { get; set; }
        static void Main(string[] args)
        {
            #region config
            //Load Config檔
            LoadConfig();
            #endregion

            #region 字串處理1
            //for (int i = 0; i < 38; i++)
            //{
            //    string url = $@"https://www.esjzone.cc/list-11/{i}.html,";

            //    Console.Write(url);
            //}
            #endregion

            #region 字串處理2
            var source = @"abyAccount	期貨帳號	TByte22	22
struTradeDate	交易日期	TPolaDate	4
byMarketNo	市場代碼	Byte	1
abyMarketName	市場名稱	TByte30	30
abyCommodityID1	商品名稱1	TByte7	7
intSettlementMonth1	商品月份1	Int	4
intStrikePrice1	履約價1	int	4
abyBuySellKind1	買賣別1	TByte	1
abyCommodityID2	商品名稱2	TByte7	7
intSettlementMonth2	商品月份2	Int	4
intStrikePrice2	履約價2	Int	4
abyBuySellKind2	買賣別2	TByte	1
abyOpenOffsetKind	新/平倉	TByte	1
abyOrderCondition	委託條件	TByte	1
abyOrderPrice	委託價	TByte10	10
intBeforeQty	前一次委託量	Int	4
intAferQty	目前委託量	Int	4
intOKQty	成交口數	Int	4
shtStatus	委託狀態	short	2
struAcceptDate	委託日期	TYuantaDate	4
struAcceptTime	委託時間	TYuantaTime	5
abyErrorNo	錯誤代碼	TByte10	10
abyErrorMessage	錯誤訊息	TByte120	120
abyOrderNO	委託單號	TByte5	5
abyProductType	商品種類	TByte	1
ushtSeller	營業員代碼	UShort	2
lngTotalMatFee	手續費總和	Long	8
lngTotalMatExchTax	交易稅總和	Long	8
lngTotalMatPremium	應收付	Long	8
abyDayTradeID	當沖註記	TByte	1
abyCancelFlag	可取消Flag	TByte	1
abyReduceFlag	可減量Flag	TByte	1
abyStkName1	商品名稱1	TByte30	30
abyStkName2	商品名稱2	TByte30	30
abyTraditionFlag	傳統單Flag	TByte	1
abyTRID	商品代碼	TByte20	20
abyCurrencyType	交易幣別	TByte3	3
abyCurrencyType2	交割幣別	TByte3	3
abyBasketNo	BasketNo	Tbyte10	10
byMarketNo1	市場代碼1	Byte	1
abyStkCode1	行情股票代碼1	TByte12	12
byMarketNo2	市場代碼2	Byte	1
abyStkCode2	行情股票代碼2	TByte12	12";
            var souceDataset = source.Split("\r\n");

            foreach (var x in souceDataset)
            {
                var propertyName = x.Split("\t")[0];
                var propertyCnName = x.Split("\t")[1];
                var propertyClass = x.Split("\t")?[2];
                var propertysize = x.Split("\t")?[3];

                // Model建立
                //Console.WriteLine("/// <summary>");
                //Console.WriteLine($"/// {propertyCnName}");
                //Console.WriteLine("/// </summary>");
                //Console.WriteLine($"[DisplayName(\"{propertyCnName}\")]");
                //Console.WriteLine($"public string {propertyName} {{ get; set; }}");
                //Console.WriteLine();

                // converter
                switch (propertyName.Substring(0, 3))
                {
                    case "aby":
                        if (propertysize == 1.ToString())
                            Console.WriteLine($"result.{propertyName} =  dataGetter.GetStr(Marshal.SizeOf(typeof(TByte)));");
                        else
                            Console.WriteLine($"result.{propertyName} =  dataGetter.GetStr(Marshal.SizeOf(typeof(TByte{propertysize})));");
                        break;
                    case "lng":
                        Console.WriteLine($"result.{propertyName} = dataGetter.GetLong().ToString();");
                        break;
                    case "uin":
                        Console.WriteLine($"result.{propertyName} = dataGetter.GetUInt().ToString();");
                        break;
                    case "int":
                        Console.WriteLine($"result.{propertyName} = dataGetter.GetInt().ToString();");
                        break;
                    case "sht":
                        Console.WriteLine($"result.{propertyName} = dataGetter.GetShort().ToString();");
                        break;
                    case "str":
                        //var time = dataGetter.GetTYuantaTime();
                        //var strtime = time.bytHour.ToString() + ":" + time.bytMin.ToString() + ":" + time.bytSec.ToString() + "." + time.ushtMSec;
                        //result.struTime = strtime;
                        //var date = dataGetter.GetTYuantaDate();
                        //var strdate = date.ushtYear.ToString() + "/" + date.bytMon.ToString() + "/" + date.bytDay.ToString();

                        if (propertyClass?.ToLower() == "tyuantadate")
                        {
                            Console.WriteLine($"var date{propertyName} = dataGetter.GetTYuantaDate();");
                            Console.WriteLine($"var strdate{propertyName} =  date{propertyName}.ushtYear.ToString() + \"/\" + date{propertyName}.bytMon.ToString() + \"/\" + date{propertyName}.bytDay.ToString();");
                            Console.WriteLine($"result.{propertyName} = strdate{propertyName};");
                        }
                        else if (propertyClass?.ToLower() == "tyuantatime")
                        {
                            Console.WriteLine($"var time{propertyName} = dataGetter.GetTYuantaTime();");
                            Console.WriteLine($"var strtime{propertyName} = time{propertyName}.bytHour.ToString() + \":\" + time{propertyName}.bytMin.ToString() + \":\" + time{propertyName}.bytSec.ToString() + \".\" + time{propertyName}.ushtMSec;");
                            Console.WriteLine($"result.{propertyName} = strtime{propertyName};");
                        }
                        else if (propertyClass?.ToLower() == "tyuantadatetime")
                        {
                            Console.WriteLine($"var datetime{propertyName} = dataGetter.GetTYunataDateTime();");
                            Console.WriteLine($"var strdatetime{propertyName} = datetime{propertyName}.struDate.ushtYear.ToString() + \"/\" + datetime{propertyName}.struDate.bytMon.ToString() + \"/\" + datetime{propertyName}.struDate.bytDay.ToString() + \" \" + datetime{propertyName}.struTime.bytHour.ToString() + \":\" + datetime{propertyName}.struTime.bytMin.ToString() + \":\" + datetime{propertyName}.struTime.bytSec.ToString() + \".\" + datetime{propertyName}.struTime.ushtMSec;");
                            Console.WriteLine($"result.{propertyName} = strdatetime{propertyName};");
                        }
                        else if (propertyClass?.ToLower().StartsWith("tbyte") ?? false)
                        {
                            Console.WriteLine($"result.{propertyName} = dataGetter.GetStr(Marshal.SizeOf(typeof(TByte{propertysize})));");
                        }
                        else
                        {
                            Console.WriteLine($"result.{propertyName} = dataGetter. //??? {propertyClass}");
                        }
                        break;
                    default:
                        if (propertyName.StartsWith("by"))
                        {
                            Console.WriteLine($"result.{propertyName} = dataGetter.GetByte().ToString();");
                        }
                        else
                        {
                            Console.WriteLine($"result.{propertyName} = dataGetter. //??? {propertyClass}");
                        }
                        break;
                }
            }

            #endregion

            #region 憑證
            //設定遇到憑證的反應
            //SetCertificateAction();
            #endregion

            #region httpclient 爬蟲
            //HttpClient httpClient = new HttpClient();

            //httpClient.BaseAddress = new Uri("https://localhost:44367/");

            //var responseMessage = httpClient.PostAsync("QuarkService/NewPatcherInfo", new StringContent("", Encoding.UTF8, "application/json"));
            ////Interlocked.Increment()

            //Task.WaitAll(responseMessage);

            //HttpResponseMessage httpResponseMessage = responseMessage.Result;

            //try
            //{
            //    var json = httpResponseMessage.Content.ReadAsStringAsync();

            //    Task.WaitAll(json);

            //    var jsonResult = json.Result;

            //    //TODO:版本判斷 未完成
            //    PackageConfigModel newPackage = JsonSerializer.Deserialize<PackageConfigModel>(jsonResult.ToString());

            //    Version newVer = new Version(newPackage.PackageVersion);



            //    Console.WriteLine(jsonResult.ToString());
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    //throw;
            //}
            #endregion

        }

        private static void LoadConfig()
        {
            IConfiguration config = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .Build();

            //config.GetSection("path").Bind(new object());
            //string a = config["SampleReportPath"];
        }

        static void LoadKey()
        {

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                //篩選鍵盤上的字(符號) 範圍外的是其他按鈕ex:F1、F2
                int keyValue = (int)key.Key;
                if (!((keyValue >= 48 && keyValue <= 90) || (keyValue >= 96 && keyValue <= 111) || (keyValue >= 186 && keyValue <= 226)))
                {
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("");
                    continue;
                }

                // Backspace Should Not Work

                if (key.Key != ConsoleKey.Backspace)
                {
                    pd += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (!string.IsNullOrEmpty(pd))
                    {
                        // remove one character from the list of password characters
                        pd = pd.Substring(0, pd.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

        }

        static string GetUUID()
        {
            string uuid = string.Empty;

            ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");

            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {

                uuid = mo.Properties["UUID"].Value.ToString();

                break;

            }

            Console.WriteLine(uuid);

            return uuid;
        }

        static void SetCertificateAction()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    if (policyErrors == SslPolicyErrors.None)
                    {
                        return true;
                    }

                    return false;
                };
        }
    }
}
