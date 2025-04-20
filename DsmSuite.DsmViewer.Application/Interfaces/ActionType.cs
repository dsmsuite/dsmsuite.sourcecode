namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public enum ActionType
    {
        ElementChangeName,
        ElementChangeParent,
        ElementChangeType,
        ElementCreate,
        ElementDelete,
        ElementMoveUp,
        ElementMoveDown,
        ElementSort,
        ElementCopy,
        ElementCut,
        ElementPaste,

        RelationChangeType,
        RelationChangeWeight,
        RelationCreate,
        RelationDelete,

        Snapshot
    }
}
