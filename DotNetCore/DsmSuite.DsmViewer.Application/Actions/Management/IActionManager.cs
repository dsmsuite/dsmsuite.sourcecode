using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Management
{
    public interface IActionManager
    {
        bool Validate();
        void Clear();
        void Add(IAction action);
        object Execute(IAction action);
        IEnumerable<IAction> GetActionsInChronologicalOrder();
    }
}
