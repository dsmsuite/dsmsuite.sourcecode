using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DsmSuite.DsmViewer.Application.Test.Performance
{
    /// <summary>
    /// Summary description for PerformanceTest
    /// </summary>
    [TestClass]
    public class PerformanceTest
    {

        [TestMethod]
        public void TestModelLoadPerformance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            
            long peakPagedMemMbBefore = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMbBefore = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMbBefore = currentProcess.PeakWorkingSet64 / million;
            Console.WriteLine($"Peak physical memory usage {peakWorkingSetMbBefore:0.000}MB");
            Console.WriteLine($"Peak paged memory usage    {peakPagedMemMbBefore:0.000}MB");
            Console.WriteLine($"Peak virtual memory usage  {peakVirtualMemMbBefore:0.000}MB");
            
            string inputFilename = "DsmSuite.DsmViewer.Application.PerformanceTest.Input.dsm";

            DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);

            Assert.AreEqual(1, model.GetElementCount());
            Assert.AreEqual(0, model.GetRelationCount());

            Stopwatch watch = new  Stopwatch();
            watch.Start();

            application.LoadModel(inputFilename, null);

            watch.Stop();

            long peakPagedMemMbAfter = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMbAfter = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMbAfter = currentProcess.PeakWorkingSet64 / million;
            Console.WriteLine($"Peak physical memory usage {peakWorkingSetMbAfter:0.000}MB");
            Console.WriteLine($"Peak paged memory usage    {peakPagedMemMbAfter:0.000}MB");
            Console.WriteLine($"Peak virtual memory usage  {peakVirtualMemMbAfter:0.000}MB");

            Console.WriteLine($"Load time                  {watch.ElapsedMilliseconds:0}ms");

            Assert.AreEqual(1440, model.GetElementCount());
            Assert.AreEqual(9769, model.GetRelationCount());
        }

        [TestMethod]
        public void TestModelSavePerformance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;

            long peakPagedMemMbBefore = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMbBefore = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMbBefore = currentProcess.PeakWorkingSet64 / million;
            Console.WriteLine($"Peak physical memory usage {peakWorkingSetMbBefore:0.000}MB");
            Console.WriteLine($"Peak paged memory usage    {peakPagedMemMbBefore:0.000}MB");
            Console.WriteLine($"Peak virtual memory usage  {peakVirtualMemMbBefore:0.000}MB");

            string inputFilename = "DsmSuite.DsmViewer.Application.PerformanceTest.Input.dsm";
            string outputFilename = "DsmSuite.DsmViewer.Application.PerformanceTest.Output.dsm";

            DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);

            Assert.AreEqual(1, model.GetElementCount());
            Assert.AreEqual(0, model.GetRelationCount());

            application.LoadModel(inputFilename, null);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            application.SaveModel(outputFilename, null);

            watch.Stop();

            long peakPagedMemMbAfter = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMbAfter = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMbAfter = currentProcess.PeakWorkingSet64 / million;
            Console.WriteLine($"Peak physical memory usage {peakWorkingSetMbAfter:0.000}MB");
            Console.WriteLine($"Peak paged memory usage    {peakPagedMemMbAfter:0.000}MB");
            Console.WriteLine($"Peak virtual memory usage  {peakVirtualMemMbAfter:0.000}MB");

            Console.WriteLine($"Save time                  {watch.ElapsedMilliseconds:0}ms");

            Assert.AreEqual(1440, model.GetElementCount());
            Assert.AreEqual(9769, model.GetRelationCount());
        }
    }
}
