using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.Analyzer.DotNet.Lib
{
    public class AssemblyTypeRelation
    {
        public AssemblyTypeRelation(string consumerName, string providerName, string type)
        {
            ConsumerName = consumerName;
            ProviderName = providerName;
            Type = type;
        }

        public string ConsumerName { get; }
        public string ProviderName { get; }
        public string Type { get; }
    }
}
