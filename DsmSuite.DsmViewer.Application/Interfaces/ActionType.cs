namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public enum ActionType
    {
        ElementChangeAnnotation,
        ElementChangeName,
        ElementChangeParent,
        ElementChangeType,
        ElementCreate,
        ElementDelete,
        ElementMoveUp,
        ElementMoveDown,
        ElementSort,
        
        RelationChangeType,
        RelationChangeWeight,
        RelationCreate,
        RelationDelete,

        Snapshot
    }
}
