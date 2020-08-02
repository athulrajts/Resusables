using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KEI.Infrastructure.Utils
{
    public static class PathUtils
    {
        public static string CompanyName = "GES";
        public static string ProductName = "Application";

        public static string GetPath(string folderPath)
        {
            string path = string.Empty;
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            //Get path based on where we are
            if (Environment.CurrentDirectory.Contains(progFiles))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CompanyName, ProductName, folderPath);
            }
            else
            {
                path = folderPath;
            }

            return path;
        }
    }
}
