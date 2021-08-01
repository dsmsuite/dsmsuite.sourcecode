namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public class IncludeCandidate
    {
        public IncludeCandidate(string filename)
        {
            Filename = filename;
        }
        public string Filename { get; }
        public bool Resolved { set; get; }
    }
}
