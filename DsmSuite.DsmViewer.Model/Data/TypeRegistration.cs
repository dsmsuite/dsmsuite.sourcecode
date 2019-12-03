using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Model.Data
{
    /// <summary>
    /// Collection of relation or element types. There can be max 256 different types.
    /// Introduced to reduce memory usage by avoiding storing type string multiple times in memory.
    /// </summary>
    public class TypeRegistration
    {
        private readonly Dictionary<char, string> _typeNames = new Dictionary<char, string>();
        private readonly Dictionary<string, char> _typeIds = new Dictionary<string, char>();

        public char AddTypeName(string typeName)
        {
            Debug.Assert(_typeNames.Count < 255);
            if (_typeIds.ContainsKey(typeName))
            {
                return _typeIds[typeName];
            }
            else
            {
                char id = (char)_typeNames.Count;
                _typeNames[id] = typeName;
                _typeIds[typeName] = id;
                return id;
            }
        }

        public string GetTypeName(char typeId)
        {
            if (_typeNames.ContainsKey(typeId))
            {
                return _typeNames[typeId];
            }
            else
            {
                return "unknown";
            }
        }
    }
}
