using System;
using System.Diagnostics;
using System.Text;

namespace DsmSuite.Common.Util
{
    public class ConsoleActionExecutor
    {
        private readonly string _description;
        private int _progress;
        private string _currentText;
        private readonly Stopwatch _stopWatch;
        
        public ConsoleActionExecutor(string description, string settingFilename)
        {
            _description = description;
            _progress = 0;
            _currentText = string.Empty;
            _stopWatch = new Stopwatch();
        }

        public void Execute(Action<IProgress<ProgressInfo>> action)
        {
            Logger.LogUserMessage(_description);
            Logger.LogUserMessage(new String('-', _description.Length));
            Progress<ProgressInfo> progress = new Progress<ProgressInfo>(UpdateProgress);

            StartTimer();
            action(progress);
            StopTimer();
            Logger.LogUserMessage($"Total elapsed time={ElapsedTime}");
            Logger.LogResourceUsage();
        }

        private void StartTimer()
        {
            _stopWatch.Start();
        }

        public void StopTimer()
        {
            _stopWatch.Stop();
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopWatch.Elapsed; }
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
                UpdateText($"{progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} progress={progress.Percentage}%");
            }

            if (progress.Done)
            {
                UpdateText("\n");
            }
        }

        private void UpdateProgressWithoutPercentage(ProgressInfo progress)
        {
            UpdateText($"{progress.ActionText} {progress.CurrentItemCount} {progress.ItemType}");

            if (progress.Done)
            {
                UpdateText("\n");
            }
        }

        private void UpdateText(string text)
        {
            // Find common part
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }
    }
}
