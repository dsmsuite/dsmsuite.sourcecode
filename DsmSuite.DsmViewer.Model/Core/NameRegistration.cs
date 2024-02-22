using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Model.Core
{
    /// <summary>
    /// Collection of relation or element types. There can be max 256 different types.
    /// Introduced to reduce memory usage by avoiding storing type string multiple times in memory.
    /// </summary>
    public class NameRegistration
    {
        private readonly Dictionary<char, string> _registeredNames = new Dictionary<char, string>();
        private readonly Dictionary<string, char> _nameIds = new Dictionary<string, char>();

        public char RegisterName(string typeName)
        {
            Debug.Assert(_registeredNames.Count < 255);
            if (_nameIds.ContainsKey(typeName))
            {
                return _nameIds[typeName];
            }
            else
            {
                char id = (char)_registeredNames.Count;
                _registeredNames[id] = typeName;
                _nameIds[typeName] = id;
                return id;
            }
        }

        public string GetRegisteredName(char typeId)
        {
            if (_registeredNames.ContainsKey(typeId))
            {
                return _registeredNames[typeId];
            }
            else
            {
                return "unknown";
            }
        }

        public IEnumerable<string> GetRegisteredNames()
        {
            return _registeredNames.Values;
        }
    }
}
