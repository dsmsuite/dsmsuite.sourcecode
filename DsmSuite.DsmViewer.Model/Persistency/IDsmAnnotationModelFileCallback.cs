using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmAnnotationModelFileCallback
    {
        IEnumerable<IDsmElementAnnotation> GetElementAnnotations();
        IEnumerable<IDsmRelationAnnotation> GetRelationAnnotations();

        void ImportElementAnnotation(int elementId, string text);
        void ImportRelationAnnotation(int consumerId, int providerId, string text);
    }
}
