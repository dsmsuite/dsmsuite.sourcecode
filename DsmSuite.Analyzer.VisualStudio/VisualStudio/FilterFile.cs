using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class FilterFile
    {
        private readonly Dictionary<string, string> _filterPaths = new Dictionary<string, string>();

        public FilterFile(string filterFilePath)
        {
            if (File.Exists(filterFilePath)) // Filter file is optional. Only present when filters defined
            {
                try
                {
                    XmlDocument xmlDocument = LoadXmlDocument(filterFilePath);
                    XmlNodeList clCompileNodes = xmlDocument.SelectNodes("//vs:Project/vs:ItemGroup/vs:ClCompile", GetXmlNamespaceManager(xmlDocument));
                    ScanSourceFileXmlNodes(clCompileNodes);

                    XmlNodeList clIncludenodes = xmlDocument.SelectNodes("//vs:Project/vs:ItemGroup/vs:ClInclude", GetXmlNamespaceManager(xmlDocument));
                    ScanSourceFileXmlNodes(clIncludenodes);

                    XmlNodeList midlnodes = xmlDocument.SelectNodes("//vs:Project/vs:ItemGroup/vs:Midl", GetXmlNamespaceManager(xmlDocument));
                    ScanSourceFileXmlNodes(midlnodes);
                }
                catch(Exception e)
                {
                    Logger.LogException($"Load filter failed file={filterFilePath}", e);
                }
            }
        }

        public string GetSourceFileProjectFolder(string relativeSourceFileName)
        {
            string sourceFileProjectFolder = "";
            if (_filterPaths.ContainsKey(relativeSourceFileName))
            {
                sourceFileProjectFolder = _filterPaths[relativeSourceFileName];
            }
            return sourceFileProjectFolder;
        }

        private void ScanSourceFileXmlNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Name == "Include")
                        {
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (childNode.Name == "Filter")
                                {
                                    _filterPaths[attribute.Value] = childNode.InnerText;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private XmlDocument LoadXmlDocument(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc;
        }

        private XmlNamespaceManager GetXmlNamespaceManager(XmlDocument doc)
        {
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
            xmlNamespaceManager.AddNamespace("vs", "http://schemas.microsoft.com/developer/msbuild/2003");
            return xmlNamespaceManager;
        }
    }
}
