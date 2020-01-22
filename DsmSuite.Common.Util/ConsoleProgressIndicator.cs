using System;
using System.Diagnostics;

namespace DsmSuite.Common.Util
{
    public class ConsoleProgressIndicator
    {
        private int _progress;

        public ConsoleProgressIndicator()
        {
            _progress = 0;
        }

        public void Update(ProgressInfo progress)
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
                Console.Write($"\r {progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} progress={progress.Percentage:000}%");
            }

            if (progress.Done)
            {
                Console.WriteLine($"\r {progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType}  progress={progress.Percentage:000}%");
            }
        }

        private void UpdateProgressWithoutPercentage(ProgressInfo progress)
        {
            Console.Write($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType}");

            if (progress.Done)
            {
                Console.WriteLine($"\r {progress.ActionText} {progress.CurrentItemCount} {progress.ItemType}");
            }
        }
    }
}
