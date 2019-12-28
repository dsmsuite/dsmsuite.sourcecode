using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;

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

        public static ISortAlgorithm CreateAlgorithm(IDsmModel model, IDsmElement element, string algorithName)
        {
            ISortAlgorithm algoritm = null;

            if (Algorithms.ContainsKey(algorithName))
            {
                Type type = Algorithms[algorithName];
                object[] args = { model , element };
                object argumentList = args;
                algoritm = Activator.CreateInstance(type, argumentList) as ISortAlgorithm;
            }
            return algoritm;
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
