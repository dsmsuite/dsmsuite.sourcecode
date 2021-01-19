using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.CompareLib;

namespace DsmSuite.Analyzer.Compare
{
    public class ConsoleAction : ConsoleActionBase
    {
        private readonly FileInfo _inputFile1;
        private readonly FileInfo _inputFile2;

        public ConsoleAction(FileInfo inputFile1, FileInfo inputFile2) : base("Comparing dependency files")
        {
            _inputFile1 = inputFile1;
            _inputFile2 = inputFile2;
        }

        protected override bool CheckPrecondition()
        {
            return true;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage("Input files:");
            Logger.LogUserMessage($" {_inputFile1}");
            Logger.LogUserMessage($" {_inputFile2}");
        }

        protected override void Action()
        {
            bool secondFileIsOlder = _inputFile1.CreationTime < _inputFile2.CreationTime;
            FileInfo oldModelFile = secondFileIsOlder ? _inputFile1 : _inputFile2;
            FileInfo newModelFile = secondFileIsOlder ? _inputFile2 : _inputFile1;

            DsiModel oldModel = new DsiModel("Diff", new List<string>(), Assembly.GetExecutingAssembly());
            DsiModel newModel = new DsiModel("Diff", new List<string>(), Assembly.GetExecutingAssembly());
            oldModel.Load(oldModelFile.FullName, this);
            newModel.Load(newModelFile.FullName, this);

            DsiModelCompare comparer = new DsiModelCompare(oldModel, newModel, this);
            comparer.Compare();

            if (comparer.AreIdentical)
            {
                Logger.LogUserMessage("Models are identical");
            }
            else
            {
                Logger.LogUserMessage("Models are different");
                ReportDeltas(comparer);
            }
        }

        private void ReportDeltas(DsiModelCompare comparer)
        {
            Logger.LogUserMessage("Added Elements");
            Logger.LogUserMessage("==============");
            foreach (string elementName in comparer.AddedElements)
            {
                Logger.LogUserMessage($" {elementName}");
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Removed Elements");
            Logger.LogUserMessage("===============");
            foreach (string elementName in comparer.RemovedElements)
            {
                Logger.LogUserMessage($" {elementName}");
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Added Relations");
            Logger.LogUserMessage("==============");
            foreach (KeyValuePair<string, HashSet<string>> relations in comparer.AddedRelations)
            {
                string consumer = relations.Key;
                foreach (string provider in relations.Value)
                {
                    Logger.LogUserMessage($" {consumer} -> {provider}");
                }
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Removed Relations");
            Logger.LogUserMessage("=================");
            foreach (KeyValuePair<string, HashSet<string>> relations in comparer.RemovedRelations)
            {
                string consumer = relations.Key;
                foreach (string provider in relations.Value)
                {
                    Logger.LogUserMessage($" {consumer} -> {provider}");
                }
            }

            Console.WriteLine("");
        }

        protected override void LogOutputParameters()
        {
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);

            if (args.Length < 2)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.Compare <file1> <file2>");
            }
            else
            {
                FileInfo inputFile1 = new FileInfo(args[0]);
                FileInfo inputFile2 = new FileInfo(args[1]);
                if (!inputFile1.Exists)
                {
                    Logger.LogUserMessage($"{inputFile1.FullName} does not exist.");
                }
                else if (!inputFile2.Exists)
                {
                    Logger.LogUserMessage($"{inputFile2.FullName} does not exist.");
                }
                else
                {
                    ConsoleAction action = new ConsoleAction(inputFile1, inputFile2);
                    action.Execute();
                }
            }
        }
    }
}
