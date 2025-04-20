using System.Reflection;

namespace DsmSuite.Common.Util
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
