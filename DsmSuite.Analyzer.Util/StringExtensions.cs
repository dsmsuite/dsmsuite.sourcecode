using System.Text.RegularExpressions;

namespace DsmSuite.Analyzer.Util
{
    public static class StringExtensions
    {
        public static string ReplaceIgnoreCase(this string input, string oldValue, string newValue)
        {
            return Regex.Replace(input,Regex.Escape(oldValue),newValue.Replace("$", "$$"),RegexOptions.IgnoreCase);
        }
    }
}
