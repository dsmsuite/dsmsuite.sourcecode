using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementSortAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly string _algorithm;
        private string _order;

        public const string TypeName = "epartition";

        public ElementSortAction(object[] args)
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

            _algorithm = attributes.GetString(nameof(_algorithm));
            _order= attributes.GetString(nameof(_order));
        }

        public ElementSortAction(IDsmModel model, IDsmElement element, string algorithm)
        {
            _model = model;
            Debug.Assert(_model != null);

            _element = element;
            Debug.Assert(_element != null);

            _algorithm = algorithm;
            _order = "";
        }

        public string Type => TypeName;
        public string Title => "Partition element";
        public string Description => $"element={_element.Fullname} algorithm={_algorithm}";

        public void Do()
        {
            ISortAlgorithm sortAlgorithm = SortAlgorithmFactory.CreateAlgorithm(_model, _element, _algorithm);
            SortResult sortResult = sortAlgorithm.Sort();
            _model.ReorderChildren(_element, sortResult);
            _order = sortResult.Data;
        }

        public void Undo()
        {
            SortResult sortResult = new SortResult(_order);
            sortResult.InvertOrder();
            _model.ReorderChildren(_element, sortResult);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_algorithm), _algorithm);
                attributes.SetString(nameof(_order), _order);
                return attributes.Data;
            }
        }
    }
}
