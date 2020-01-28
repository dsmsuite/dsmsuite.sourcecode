using System.Collections.Generic;

namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public class BestMatchIncludeFileResolveStrategy : IncludeResolveStrategy
    {
        public BestMatchIncludeFileResolveStrategy(IList<string> includeDirectories) : base(includeDirectories)
        {
        }

        protected override void SelectCandidates(string sourceFilename, IList<IncludeCandidate> candidates)
        {
            IncludeCandidate bestCandidate = FindBestIncludeFilenameCandidates(sourceFilename, candidates);
            if (bestCandidate != null)
            {
                bestCandidate.Resolved = true;
            }
        }

        private static IncludeCandidate FindBestIncludeFilenameCandidates(string sourceFilename, IList<IncludeCandidate> candidates)
        {
            switch (candidates.Count)
            {
                case 0:
                    return null;
                case 1:
                    return candidates[0];
                default:
                    IncludeCandidate bestCandidate = candidates[0];
                    int bestMatchLength = 0;
                    foreach (IncludeCandidate candidate in candidates)
                    {
                        int matchLength = FindMatchingCharacters(sourceFilename, candidate.Filename);

                        if (matchLength > bestMatchLength)
                        {
                            bestMatchLength = matchLength;
                            bestCandidate = candidate;
                        }
                    }

                    return bestCandidate;
            }
        }

        private static int FindMatchingCharacters(string filename1, string filename2)
        {
            bool end = false;
            int index = 0;

            while (!end)
            {
                if (filename2.Length > index &&
                    filename1.Length > index &&
                    filename2[index] == filename1[index])
                {
                    index++;
                }
                else
                {
                    end = true;
                }
            }

            return index;
        }
    }
}
