using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Actions.Base;

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
