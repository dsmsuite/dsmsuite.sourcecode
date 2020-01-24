using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.Common.Util
{
    public class ConsoleActionExecutor
    {
        public void Execute(Action action, string description)
        {
            action();
        }
    }
}
