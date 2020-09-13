using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmAnnotationModelFileCallback
    {
        IEnumerable<IDsmElementAnnotation> GetElementAnnotations();
        IEnumerable<IDsmRelationAnnotation> GetRelationAnnotations();
    }
}
