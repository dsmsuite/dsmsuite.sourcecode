using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ReadOnlyActionAttributes
    {
        private readonly IReadOnlyDictionary<string, string> _data;

        public ReadOnlyActionAttributes(IReadOnlyDictionary<string, string> data)
        {
            _data = data;
        }

        public string GetString(string memberName)
        {
            return _data[RemoveUnderscore(memberName)];
        }

        public int GetInt(string memberName)
        {
            return int.Parse(_data[RemoveUnderscore(memberName)]);
        }

        public int? GetNullableInt(string memberName)
        {
            int? value = null;

            int number;
            if (_data.ContainsKey(RemoveUnderscore(memberName)) && 
                int.TryParse(_data[RemoveUnderscore(memberName)], out number))
            {
                value = number;
            }

            return value;
        }

        private static string RemoveUnderscore(string memberName)
        {
            return memberName.Substring(1);
        }
    }
}
