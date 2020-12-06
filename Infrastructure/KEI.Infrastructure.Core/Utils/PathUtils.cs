using System;
using System.IO;

namespace KEI.Infrastructure.Utils
{
    public static class PathUtils
    {
        public static string CompanyName = "GES";
        public static string ProductName = "Application";

        public static string GetPath(string folderPath)
        {
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            //Get path based on where we are

            return Environment.CurrentDirectory.Contains(progFiles)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CompanyName, ProductName, folderPath)
                : folderPath;
        }
    }
}
