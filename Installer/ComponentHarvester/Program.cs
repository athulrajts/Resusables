using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ComponentHarvester
{
    class Program
    {
        private static Random random = new Random();
        private static string harvestingDir;
        private static string outputDir;
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                return;
            }

            harvestingDir = args[0];
            outputDir = args[1];

            DeleteDir(outputDir);
            CopyDir(harvestingDir, outputDir);
            Copy(Path.Combine(harvestingDir, "Application.exe"), Path.Combine(outputDir, "Application.exe"));
            Copy(Path.Combine(harvestingDir, "ConfigEditor.exe"), Path.Combine(outputDir, "ConfigEditor.exe"));
            Copy(Path.Combine(harvestingDir, "ServiceEditor.exe"), Path.Combine(outputDir, "ServiceEditor.exe"));
            Copy(Path.Combine(harvestingDir, "LogViewer.exe"), Path.Combine(outputDir, "LogViewer.exe"));
            Copy(Path.Combine(harvestingDir, @"Configs\view.view"), Path.Combine(outputDir, @"Configs\view.view"));
            Copy(Path.Combine(harvestingDir, @"Configs\Services.cfg"), Path.Combine(outputDir, @"Configs\Services.cfg"));

            CleanDirectories(outputDir);
        }

        public static void CleanDirectories(string rootPath)
        {
            var dir = new DirectoryInfo(rootPath);

            foreach (var item in dir.GetDirectories())
            {
                var subdirs = item.EnumerateDirectories();
                if (subdirs.Count() > 0)
                {
                    CleanDirectories(item.FullName);
                }
                if(item.EnumerateFiles().Count() == 0)
                {
                    item.Delete();
                }
            }
        }

        public static void CopyDir(string SourcePath, string DestinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.EnumerateDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            var files = Directory.EnumerateFiles(SourcePath, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".runtimeconfig.json", StringComparison.OrdinalIgnoreCase));
            
            foreach (string newPath in files)
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public static void Copy(string source, string dest)
        {
            File.Copy(source, dest, true);
        }

        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path) == false)
                return;

            var dir = new DirectoryInfo(path);

            foreach (var subdir in dir.EnumerateDirectories())
            {
                DeleteDir(subdir.FullName);
            }
            foreach (var file in dir.EnumerateFiles())
            {
                file.Delete();
            }

            dir.Delete();
        }
    }
}
