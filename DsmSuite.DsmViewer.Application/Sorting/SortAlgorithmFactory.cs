using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Sorting
{
    public class SortAlgorithmFactory
    {
        private static readonly Dictionary<string, Type> Algorithms;

        static SortAlgorithmFactory()
        {
            Algorithms = new Dictionary<string, Type>();
            RegisterAlgorithmTypes();
        }

        public static void RegisterAlgorithm(string name, Type algorithm)
        {
            Algorithms[name] = algorithm;
        }

        public static ISortAlgorithm CreateAlgorithm(IDsmModel model, IDsmElement element, string algorithmName)
        {
            ISortAlgorithm algorithm = null;

            if (Algorithms.ContainsKey(algorithmName))
            {
                Type type = Algorithms[algorithmName];
                object[] args = { model, element };
                object argumentList = args;
                algorithm = Activator.CreateInstance(type, argumentList) as ISortAlgorithm;
            }
            return algorithm;
        }

        public static IEnumerable<string> GetSupportedAlgorithms()
        {
            return Algorithms.Keys;
        }

        private static void RegisterAlgorithmTypes()
        {
            RegisterAlgorithm(PartitionSortAlgorithm.AlgorithmName, typeof(PartitionSortAlgorithm));
            RegisterAlgorithm(AlphabeticalSortAlgorithm.AlgorithmName, typeof(AlphabeticalSortAlgorithm));
        }
    }
}
