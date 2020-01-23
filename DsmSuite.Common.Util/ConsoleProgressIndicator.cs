using System;
using System.Diagnostics;

namespace DsmSuite.Common.Util
{
    public class ConsoleProgressIndicator
    {
        private const string Spacer = "                             ";

        private int _progress;
        private Stopwatch _stopWatch;

        public ConsoleProgressIndicator()
        {
            _progress = 0;
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public void UpdateProgress(ProgressInfo progress)
        {
            if (progress.Percentage.HasValue)
            {
                UpdateProgressWithPercentage(progress);
            }
            else
            {
                UpdateProgressWithoutPercentage(progress);
            }
        }

        public void Done()
        {
            _stopWatch.Stop();
            Logger.LogUserMessage($" Total elapsed time={_stopWatch.Elapsed}");

            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" Peak physical memory usage {peakWorkingSetMb:0.000} MB");
            Logger.LogUserMessage($" Peak paged memory usage    {peakPagedMemMb:0.000} MB");
            Logger.LogUserMessage($" Peak virtual memory usage  {peakVirtualMemMb:0.000} MB");
        }

        private void UpdateProgressWithPercentage(ProgressInfo progress)
        {
            Debug.Assert(progress.Percentage.HasValue);

            if (_progress != progress.Percentage)
            {
                _progress = progress.Percentage.Value;
                Console.Write($"\r {progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} progress={progress.Percentage:000}% {Spacer}");
            }

            if (progress.Done)
            {
                Console.WriteLine($"\r {progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} progress={progress.Percentage:000}% {Spacer}");
            }
        }

        private void UpdateProgressWithoutPercentage(ProgressInfo progress)
        {
            Console.Write($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType} {Spacer}");

            if (progress.Done)
            {
                Console.WriteLine($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType} {Spacer}");
            }
        }
    }
}
