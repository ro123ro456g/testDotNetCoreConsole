using System;
using System.Net.Http;

namespace CoreTestConsoleApp
{
    class Program
    {
        static string pd { get; set; }
        static void Main(string[] args)
        {


            Console.WriteLine("Hello World!");
            HttpClient httpClient = new HttpClient();
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
    }
}
