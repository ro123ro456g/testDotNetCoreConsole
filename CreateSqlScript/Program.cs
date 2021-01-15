using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CreateSqlScript
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var userList = secretData.userList;
            var cumList = secretData.cumList;

            StringBuilder txt = new StringBuilder();
            int count = 0;
            foreach(var userModel in userList)
            {
                var userSplit = userModel.Split(",");
                var userId = userSplit[0];
                var userDep = userSplit[1];
                var userName = userSplit[2];

                if (userDep.Contains("秘書"))
                {
                    continue;
                }

                foreach (var cumModel in cumList)
                {
                    var cumModelSplit = cumModel.Split(",");
                    var cumName = cumModelSplit[1];

                    if (cumName.Contains("審查委員會") && userDep.Contains("審查委員會"))
                    {
                        var cumId = cumModelSplit[0];

                        StringBuilder stringBuilder = new StringBuilder();
                        count++;
                        stringBuilder.Append("--");
                        stringBuilder.Append(count);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(userName);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(userDep);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(cumName);
                        stringBuilder.Append("\r\n");

                        stringBuilder.Append("update [dbo].[USER_MAIN] set CUM_ID = '");
                        stringBuilder.Append(cumId);
                        stringBuilder.Append("' where UM_ID = '");
                        stringBuilder.Append(userId);
                        stringBuilder.Append("'");

                        stringBuilder.Append("\r\n");

                        Console.WriteLine(stringBuilder.ToString());

                        txt.Append(stringBuilder);
                    }

                    if (cumName.Contains(userDep))
                    {
                        var cumId = cumModelSplit[0];

                        //update [dbo].[USER_MAIN] set CUM_ID = '' where UM_ID = ''

                        StringBuilder stringBuilder = new StringBuilder();
                        count++;
                        stringBuilder.Append("--");
                        stringBuilder.Append(count);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(userName);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(userDep);
                        stringBuilder.Append(" ");
                        stringBuilder.Append(cumName);
                        stringBuilder.Append("\r\n");

                        stringBuilder.Append("update [dbo].[USER_MAIN] set CUM_ID = '");
                        stringBuilder.Append(cumId);
                        stringBuilder.Append("' where UM_ID = '");
                        stringBuilder.Append(userId);
                        stringBuilder.Append("'");

                        stringBuilder.Append("\r\n");

                        Console.WriteLine(stringBuilder.ToString());

                        txt.Append(stringBuilder);
                    }
                }
            }

            File.WriteAllText("./output.txt", txt.ToString());
        }
    }
}
