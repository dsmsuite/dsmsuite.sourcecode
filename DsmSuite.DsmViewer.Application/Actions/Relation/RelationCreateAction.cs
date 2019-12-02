using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Actions.Relation
{
    public class RelationCreateAction : ActionBase, IAction
    {
        private int _consumerId;
        private int _providerId;
        private string _type;
        private int _weight;

        public RelationCreateAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            _consumerId = consumerId;
            _providerId = providerId;
            _type = type;
            _weight = weight;
        }

        public void Do()
        {
            Model.AddRelation(_consumerId, _providerId, _type, _weight);
        }

        public void Undo()
        {
            Model.RemoveRelation(_consumerId, _providerId, _type, _weight);
        }

        public string Description => "Create relation";
    }
}
