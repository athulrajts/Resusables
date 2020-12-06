using System;
using System.IO;

namespace KEI.Infrastructure.Helpers
{
    public class FileHelper
    {
        public static bool Copy(string source, string destination, bool overwrite = true)
        {

            if (!File.Exists(source))
            {
                return false;
            }

            if (string.IsNullOrEmpty(destination))
            {
                return false;
            }

            CreateDirectoryIfNotExist(destination);

            try
            {
                File.Copy(source, destination, overwrite);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool CreateDirectoryIfNotExist(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                try
                {
                    Directory.CreateDirectory(dir);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return false;
        }
    }
}
