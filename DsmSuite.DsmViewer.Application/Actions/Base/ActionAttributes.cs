using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionAttributes
    {
        Dictionary<string, string> _data;

        public ActionAttributes()
        {
            _data = new Dictionary<string, string>();
        }

        public void SetString(string key, string value)
        {
            _data[key] = value;
        }

        public void SetInt(string key, int value)
        {
            _data[key] = value.ToString();
        }

        public void SetNullableInt(string key, int? value)
        {
            if (value.HasValue)
            {
                _data[key] = value.Value.ToString();
            }
        }

        public IReadOnlyDictionary<string, string> GetData()
        {
            return _data;
        }
    }
}
