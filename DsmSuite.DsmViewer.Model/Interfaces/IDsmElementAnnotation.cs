using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmElementAnnotation
    {
        int ElementId { get; }
        string Text { get; set; }
    }
}
