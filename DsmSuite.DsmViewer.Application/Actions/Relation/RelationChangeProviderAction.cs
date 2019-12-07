using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Relation
{
    public class RelationChangeProviderAction : ActionBase
    {
        public RelationChangeProviderAction(IDsmModel model) : base(model)
        {
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Description => "Move relation";
    }
}
