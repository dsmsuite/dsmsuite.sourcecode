using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public abstract class ActionBase
    {
        protected ActionBase(IDsmModel model)
        {
            Model = model;
        }

        protected IDsmModel Model { get; }
    }
}
