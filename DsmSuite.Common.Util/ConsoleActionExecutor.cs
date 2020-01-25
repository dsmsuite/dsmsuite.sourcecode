using System;
using System.Diagnostics;
using System.Threading;

namespace DsmSuite.Common.Util
{
    public class ConsoleActionExecutor
    {
        private const string Spacer = "                             ";

        private readonly string _description;
        private int _progress;
        private Stopwatch _stopWatch;

        public ConsoleActionExecutor(string description)
        {
            _description = description;
            _progress = 0;
            StartTimer();
        }

        public void Execute(Action<IProgress<ProgressInfo>> action)
        {
            Logger.LogUserMessage(_description);

            Progress<ProgressInfo> progress = new Progress<ProgressInfo>(UpdateProgress);

            StartTimer();
            action(progress);
            StopTimer();
            Logger.LogResourceUsage();
        }

        private void StartTimer()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public void StopTimer()
        {
            _stopWatch.Stop();
            Logger.LogUserMessage($" Total elapsed time={_stopWatch.Elapsed}");
        }

        private void UpdateProgress(ProgressInfo progress)
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
                Logger.LogUserMessage($"\r {progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} progress={progress.Percentage:000}% {Spacer}");
            }
        }

        private void UpdateProgressWithoutPercentage(ProgressInfo progress)
        {
            Console.Write($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType} {Spacer}");

            if (progress.Done)
            {
                Logger.LogUserMessage($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType} {Spacer}");
            }
        }
    }
}
