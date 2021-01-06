using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESJZoneReptile
{
    public static class FileUtil
    {
        public static void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public static string ReadTextFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
