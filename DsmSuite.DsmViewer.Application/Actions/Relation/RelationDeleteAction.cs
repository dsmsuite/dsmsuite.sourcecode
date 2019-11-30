using DsmSuite.DsmViewer.Application.Actions.Base;

namespace DsmSuite.DsmViewer.Model.Actions.Relation
{
    public class RelationDeleteAction : ActionBase
    {
        private int _consumerId;
        private int _providerId;
        private string _type;
        private int _weight;

        public RelationDeleteAction(IDsmModel model, int consumerId, int providerId, string type, int weight) : base(model)
        {
            _consumerId = consumerId;
            _providerId = providerId;
            _type = type;
            _weight = weight;
        }

        public void Do()
        {
            Model.RemoveRelation(_consumerId, _providerId, _type, _weight);
        }

        public void Undo()
        {
            Model.UnremoveRelation(_consumerId, _providerId, _type, _weight);
        }

        public string Description => "Delete relation";
    }
}
