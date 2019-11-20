
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Server;

namespace DsmSuite.Transformer.Transformation
{
    public abstract class Action
    {
        private string _name;
        private bool _enabled;

        public Action(string name, bool enabled)
        {
            _name = name;
            _enabled = enabled;
        }

        public bool IsEnabled
        {
            get { return _enabled; }
        }

        public void Execute()
        {
            if (IsEnabled)
            {
                ExecuteImpl();
            }
        }

        protected abstract void ExecuteImpl();

        public string Name
        {
            get { return _name; }
        }
    }
}
