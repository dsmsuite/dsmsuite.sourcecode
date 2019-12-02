using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmResolvedRelation
    {
        IDsmElement Consumer { get; }
        IDsmElement Provider { get; }
        string Type { get; }
        int Weight { get; }
    }
}
