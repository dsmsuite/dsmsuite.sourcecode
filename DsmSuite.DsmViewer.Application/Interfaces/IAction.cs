﻿using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IAction
    {
        ActionType Type { get; }
        string Title { get; }
        string Description { get; }

        object Do();
        void Undo();

        IReadOnlyDictionary<string, string> Data { get; }
    }
}
