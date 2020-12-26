using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmRelationAnnotation : IDsmRelationAnnotation
    {
        public DsmRelationAnnotation(int consumerId, int providerId, string text)
        {
            ConsumerId = consumerId;
            ProviderId = providerId;
            Text = text;
        }

        public int ConsumerId { get; }
        public int ProviderId { get; }
        public string Text { get; set; }
    }
}
