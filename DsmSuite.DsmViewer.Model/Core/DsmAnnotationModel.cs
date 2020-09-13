using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmAnnotationModel : IDsmAnnotationModelFileCallback
    {
        Dictionary<int, DsmElementAnnotation> _elementAnnotations = new Dictionary<int, DsmElementAnnotation>();
        Dictionary<int, Dictionary<int, DsmRelationAnnotation>> _relationAnnotations = new Dictionary<int, Dictionary<int, DsmRelationAnnotation>>();

        public void AddElementAnnotation(IDsmElement element, string text)
        {
            _elementAnnotations[element.Id] = new DsmElementAnnotation(element.Id, text);
        }

        public void AddRelationAnnotation(IDsmElement consumer, IDsmElement provider, string text)
        {
            if (!_relationAnnotations.ContainsKey(consumer.Id))
            {
                _relationAnnotations[consumer.Id] = new Dictionary<int, DsmRelationAnnotation>();
            }
            _relationAnnotations[consumer.Id][provider.Id] = new DsmRelationAnnotation(consumer.Id, provider.Id, text);
        }

        public IEnumerable<IDsmElementAnnotation> GetElementAnnotations()
        {
            return _elementAnnotations.Values;
        }

        public IEnumerable<IDsmRelationAnnotation> GetRelationAnnotations()
        {
            List<DsmRelationAnnotation> annotations = new List<DsmRelationAnnotation>();
            foreach (Dictionary<int, DsmRelationAnnotation> annotationsForConsumer in _relationAnnotations.Values)
            {
                annotations.AddRange(annotationsForConsumer.Values);
            }
            return annotations;
        }

        public IDsmElementAnnotation FindElementAnnotation(IDsmElement element)
        {
            DsmElementAnnotation annotation;
            _elementAnnotations.TryGetValue(element.Id, out annotation);
            return annotation;
        }

        public IDsmRelationAnnotation FindRelationAnnotation(IDsmElement consumer, IDsmElement provider)
        {
            DsmRelationAnnotation annotation = null;
            Dictionary<int, DsmRelationAnnotation> annotationsForConsumer;
            if (_relationAnnotations.TryGetValue(consumer.Id, out annotationsForConsumer))
            {
                annotationsForConsumer.TryGetValue(provider.Id, out annotation);
            }
            return annotation;
        }
    }
}
