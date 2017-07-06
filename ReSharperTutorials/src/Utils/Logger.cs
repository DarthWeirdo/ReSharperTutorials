using System.Diagnostics;

namespace ReSharperTutorials.Utils
{
    public static class Logger
    {
        public static void Log(string line)
        {
           // return;

            System.IO.Directory.CreateDirectory(@"C:\rstutorialslog\");

            using (var file = new System.IO.StreamWriter(@"C:\rstutorialslog\rstutlog.txt", true))
            {
                file.WriteLine(Stopwatch.GetTimestamp() + ": " + line);
            }
        }        
    }
}