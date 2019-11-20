namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class GeneratedFileRelation
    {
        public GeneratedFileRelation(SourceFile consumer, SourceFile provider)
        {
            Consumer = consumer;
            Provider = provider;
        }

        public SourceFile Consumer { get; private set; }
        public SourceFile Provider { get; private set; }
    }
}
