using System;
using System.IO;
using System.Reflection;

namespace DsmSuite.DsmViewer.Util
{
    public class SystemInfo
    {
        public static string GetExecutableInfo(Assembly assembly)
        {
            string name = assembly.GetName().Name;
            string version = assembly.GetName().Version.ToString();
            DateTime buildDate = new FileInfo(assembly.Location).LastWriteTime;
            return $"{name} version={version} build={buildDate}";
        }
    }
}
