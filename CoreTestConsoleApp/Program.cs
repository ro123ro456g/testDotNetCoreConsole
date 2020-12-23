using CoreTestConsoleApp.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Management;
using System.Net.Http;
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
            //設定遇到憑證的反應
            SetCertificateAction();

            //Load Config檔
            LoadConfig();

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:44367/");

            var responseMessage = httpClient.PostAsync("QuarkService/NewPatcherInfo", new StringContent("", Encoding.UTF8, "application/json"));
            //Interlocked.Increment()

            Task.WaitAll(responseMessage);

            HttpResponseMessage httpResponseMessage = responseMessage.Result;

            try
            {
                var json = httpResponseMessage.Content.ReadAsStringAsync();

                Task.WaitAll(json);

                var jsonResult = json.Result;

                JsonSerializer.Deserialize<PackageConfigModel>(jsonResult.ToString());

                Console.WriteLine(jsonResult.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }

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
                    return true;
                };
        }
    }
}
