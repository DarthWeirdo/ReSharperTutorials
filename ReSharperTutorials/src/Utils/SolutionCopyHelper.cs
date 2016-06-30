using System;
using System.IO;
using System.Linq;

namespace ReSharperTutorials.Utils
{
    public static class SolutionCopyHelper
    {
        public const string PluginName = "JetBrains.ReSharperTutorials";


        public static void CopySolution(string sourceFolder, string targetFolder)
        {
            //            var dir = new DirectoryInfo(sourceFolder);
            //
            //            if (!dir.Exists)
            //            {
            //                throw new DirectoryNotFoundException(
            //                    "Unable to find sample solution. Please reinstall the plugin"
            //                    + sourceFolder);
            //            }
            //
            //            DirectoryInfo[] dirs = dir.GetDirectories();
            //            
            //            if (!Directory.Exists(targetFolder))            
            //                Directory.CreateDirectory(targetFolder);
            //                        
            //            FileInfo[] files = dir.GetFiles();
            //            foreach (FileInfo file in files)
            //            {
            //                var temppath = Path.Combine(targetFolder, file.Name);
            //                file.CopyTo(temppath, false);
            //            }
            //
            //            foreach (DirectoryInfo subdir in dirs)
            //            {
            //                var temppath = Path.Combine(targetFolder, subdir.Name);
            //                CopySolution(subdir.FullName, temppath);
            //            }

            foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceFolder, targetFolder));
            
            foreach (string newPath in Directory.GetFiles(sourceFolder, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourceFolder, targetFolder), true);

        }

        public static string GetTutorialsPath()
        {
            var pluginsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\JetBrains\\plugins";
            var dirs = Directory.GetDirectories(pluginsPath);
            string result = null;

            foreach (var dir in dirs.Where(dir => dir.Contains(PluginName)))
            {
                result = dir + "\\Tutorials";
            }

            return result;
        }


        public static string GetWorkingCopyPath()
        {
            var pluginsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\JetBrains\\plugins";
            var dirs = Directory.GetDirectories(pluginsPath);
            string result = null;

            foreach (var dir in dirs.Where(dir => dir.Contains(PluginName)))
            {
                result = dir + "\\WorkingCopy";
            }

            return result;
        }

    }
}
