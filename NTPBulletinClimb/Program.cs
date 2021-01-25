using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace NTPBulletinClimb
{
    class Program
    {
        static List<INFO_MAIN> old_bulletins = new List<INFO_MAIN>();

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();

            string orgNTPDbConnectString = config.GetConnectionString("OriginDatabase");

            using (var conn = new SqlConnection(orgNTPDbConnectString))
            {
                conn.Open();

                using (var cmd = new SqlCommand("select*from [NTPM].[dbo].[INFO_MAIN]", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MapInfoMainData(reader);
                        }
                    }
                }
            }

            foreach (var a in old_bulletins)
            {


                Console.WriteLine(a.H_ID);
            }
            //Console.WriteLine(orgNTPDbConnectString);
        }

        private static void MapInfoMainData(SqlDataReader reader)
        {
            INFO_MAIN iNFO_MAIN = new INFO_MAIN();
            Type infoMainType = typeof(INFO_MAIN);

            foreach (var property in infoMainType.GetProperties())
            {
                string pName = property.Name;
                infoMainType.GetProperty(pName).SetValue(iNFO_MAIN, reader[pName].ToString());
            }

            old_bulletins.Add(iNFO_MAIN);
        }


    }
}
