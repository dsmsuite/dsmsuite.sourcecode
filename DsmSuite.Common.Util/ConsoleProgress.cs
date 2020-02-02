using System;
using System.Text;

namespace DsmSuite.Common.Util
{
    public class ConsoleProgress : IProgress<ProgressInfo>
    {
        private string _currentText = string.Empty;

        public void Report(ProgressInfo progress)
        {
            if (progress.Percentage.HasValue)
            {
                WriteToConsole($"{progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} {progress.Percentage.Value}%", true, progress.Done);
            }
            else
            {
                WriteToConsole($"{progress.ActionText} {progress.CurrentItemCount} {progress.ItemType}", true, progress.Done);
            }
        }

        public  void WriteToConsole(string text, bool overwrite, bool endOfLine)
        {
            if (!Console.IsOutputRedirected)
            {
                StringBuilder outputBuilder = new StringBuilder();

                if (overwrite)
                {
                    outputBuilder.Append("\r");
                    outputBuilder.Append(text);
                    int overlapCount = _currentText.Length - text.Length;
                    if (overlapCount > 0)
                    {
                        outputBuilder.Append(' ', overlapCount);
                        outputBuilder.Append('\b', overlapCount);
                    }

                    Console.Write(outputBuilder);
                }

                outputBuilder.Clear();

                if (overwrite)
                {
                    outputBuilder.Append("\r");
                }
                outputBuilder.Append(text);
                if (endOfLine)
                {
                    outputBuilder.Append("\n");
                }
                Console.Write(outputBuilder);

                _currentText = outputBuilder.ToString();
            }
        }
    }
}
