using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionAttributes
    {
        readonly Dictionary<string, string> _data;

        public ActionAttributes()
        {
            _data = new Dictionary<string, string>();
        }

        public void SetString(string memberName, string memberValue)
        {
            _data[RemoveUnderscore(memberName)] = memberValue;
        }

        public void SetInt(string memberName, int memberValue)
        {
            _data[RemoveUnderscore(memberName)] = memberValue.ToString();
        }

        public void SetNullableInt(string memberName, int? memberValue)
        {
            if (memberValue.HasValue)
            {
                _data[RemoveUnderscore(memberName)] = memberValue.Value.ToString();
            }
        }

        public IReadOnlyDictionary<string, string> Data => _data;

        private static string RemoveUnderscore(string memberName)
        {
            return memberName.Substring(1); 
        }
    }
}
