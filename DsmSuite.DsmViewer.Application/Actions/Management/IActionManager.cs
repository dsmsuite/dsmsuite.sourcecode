using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Management
{
    public interface IActionManager
    {
        void Add(IAction action);
        IEnumerable<IAction> GetActionsInChronologicalOrder();
    }
}
