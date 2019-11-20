using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files.Dsi;
using DsmSuite.DsmViewer.Util;

namespace DsmSuite.DsmViewer.Builder
{
    public class Builder : IDsiModelFileReaderCallback
    {
        private readonly BuilderSettings _builderSettings;
        private readonly IDsmModel _model;
        private readonly Dictionary<int, IElement> _elementsById = new Dictionary<int, IElement>();

        public Builder(IDsmModel model, BuilderSettings builderSettings)
        {
            _builderSettings = builderSettings;
            _model = model;
        }

        public void BuildModel()
        {
            Logger.LogUserMessage("Build model");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _model.Clear();

            DsiModelFileReader dsiModelFile = new DsiModelFileReader(_builderSettings.InputFilename, this);
            dsiModelFile.ReadFile(null);
            _model.AssignElementOrder();
            _model.SaveModel(_builderSettings.OutputFilename, _builderSettings.CompressOutputFile, null);

            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            Logger.LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            Logger.LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");

            stopWatch.Stop();
            Logger.LogUserMessage($" total elapsed time = {stopWatch.Elapsed}");
        }
        
        public void FoundMetaData(string group, string name, string value)
        {
            _model.AddMetaData(group, name, value);
        }

        public void FoundElement(int id, string fullname, string type)
        {
            IElement parent = null;
            HierarchicalName elementName = new HierarchicalName();
            foreach (string name in new HierarchicalName(fullname).Elements)
            {
                elementName.Add(name);

                bool isElementLeaf = (fullname == elementName.FullName);
                string elementType = isElementLeaf ? type : "";

                int? parentId = parent?.Id;
                IElement element = _model.CreateElement(name, elementType, parentId);
                parent = element;

                if (isElementLeaf)
                {
                    _elementsById[id] = element;
                }
            }
        }

        public void FoundRelation(int consumerId, int providerId, string type, int weight)
        {
            if (_elementsById.ContainsKey(consumerId) && _elementsById.ContainsKey(providerId))
            {
                IElement consumer = _elementsById[consumerId];
                IElement provider = _elementsById[providerId];
                _model.AddRelation(consumer.Id, provider.Id, type, weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={consumerId} provider={providerId}");
            }
        }
    }
}
