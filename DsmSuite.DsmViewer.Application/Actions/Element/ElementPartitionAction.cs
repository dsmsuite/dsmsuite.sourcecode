using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly string _algorithm;
        private ISortResult _reorderSequence;

        public const string TypeName = "epartition";

        public ElementPartitionAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = _model.GetElementById(id);
            Debug.Assert(_element != null);

            _algorithm = attributes.GetString(nameof(Algorithm));
        }

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm)
        {
            _model = model;
            Debug.Assert(_model != null);

            _element = element;
            Debug.Assert(_element != null);

            _algorithm = algorithm;
        }

        public string Type => TypeName;
        public string Title => "Partition element";
        public string Description => $"element={_element.Fullname} algorithm={_algorithm}";

        public void Do()
        {
            ISortAlgorithm sortAlgorithm = SortAlgorithmFactory.CreateAlgorithm(_model, _element, _algorithm);
            _reorderSequence = sortAlgorithm.Sort();
            _model.ReorderChildren(_element, _reorderSequence);
        }

        public void Undo()
        {
            SortResult inverseVector = new SortResult(_reorderSequence.GetNumberOfElements());
            for (int i = 0; i < _reorderSequence.GetNumberOfElements(); i++)
            {
                inverseVector.SetIndex(_reorderSequence.GetIndex(i), i);
            }
            _model.ReorderChildren(_element, inverseVector);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_algorithm), _algorithm);
                return attributes.Data;
            }
        }
    }
}
