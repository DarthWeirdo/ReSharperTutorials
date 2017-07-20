using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ReSharperTutorials.Runner;

namespace ReSharperTutorials.Utils
{
    public static class SolutionCopyHelper
    {
        public static void CleanUpDirectory(string folder)
        {
            var directory = new DirectoryInfo(folder);
            foreach (var file in directory.GetFiles()) file.Delete();
            foreach (var subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        public static void CopySolution(string sourceFolder, string targetFolder)
        {
            foreach (var dirPath in Directory.GetDirectories(sourceFolder, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceFolder, targetFolder));

            foreach (var newPath in Directory.GetFiles(sourceFolder, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourceFolder, targetFolder), true);
        }

        public static string GetTutorialsPath()
        {
            var pluginsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                              "\\JetBrains\\plugins";
            var dirs = Directory.GetDirectories(pluginsPath);
            string result = null;

            foreach (var dir in dirs.Where(dir => dir.Contains(GlobalSettings.PluginName)))
            {
                result = dir + "\\Tutorials";
            }

            return result;
        }


        public static string GetWorkingCopyPath()
        {
            var pluginsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                              "\\JetBrains\\plugins";            

            var dirs = Directory.GetDirectories(pluginsPath);
            string result = null;

            foreach (var dir in dirs.Where(dir => dir.Contains(GlobalSettings.PluginName)))
            {
                result = dir + "\\WorkingCopy";
            }
                        
            return result;
        }
    }
}