using MB.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MB.Logging
{
    public static class InstallLog
    {
        static string GetPath()
        {
            string assemblyFile = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            return System.IO.Path.GetDirectoryName(assemblyFile) + "\\Sources.txt";
        }

        public static List<string> GetLogs()
        {
            string path = GetPath();
            List<string> output = new List<string>();

            if (System.IO.File.Exists(path))
            {
                string logString = System.IO.File.ReadAllText(path);

                if (logString != null)
                {
                    var stringArr = logString.Split(',');
                    foreach (string l in stringArr)
                    {
                        output.Add(l.Trim());
                    }
                }
            }

            return output;
        }

        public static void InstallLogs(List<string> Log)
        {
            foreach (string l in Log)
            {
                if (!EventLog.SourceExists(l))
                    EventLog.CreateEventSource(l, "Metabro");
            }
        }
    }
}
