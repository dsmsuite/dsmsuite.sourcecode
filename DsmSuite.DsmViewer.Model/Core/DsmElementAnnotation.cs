using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmElementAnnotation : IDsmElementAnnotation
    {
        public DsmElementAnnotation(int elementId, string text)
        {
            ElementId = elementId;
            Text = text;
        }

        public int ElementId { get; }

        public string Text { get; set; }
    }
}
