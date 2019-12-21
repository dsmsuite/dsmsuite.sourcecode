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

        public string GetString(string key)
        {
            return _data[key];
        }

        public int GetInt(string key)
        {
            return int.Parse(_data[key]);
        }

        public int? GetNullableInt(string key)
        {
            int? value = null;

            int number;
            if (_data.ContainsKey(key) && int.TryParse(_data[key], out number))
            {
                value = number;
            }

            return value;
        }
    }
}
