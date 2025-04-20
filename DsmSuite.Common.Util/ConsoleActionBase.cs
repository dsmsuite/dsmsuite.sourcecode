using System.Diagnostics;
using System.Text;

namespace DsmSuite.Common.Util
{
    public abstract class ConsoleActionBase : IProgress<ProgressInfo>
    {
        private const string InputParameters = "Input parameters";
        private const string Processing = "Processing";
        private const string OutputParameters = "Ouput parameters";
        private const string Performance = "Performance";

        private readonly string _title;
        private readonly Stopwatch _stopWatch;
        private string _currentText = string.Empty;

        protected ConsoleActionBase(string title)
        {
            _title = title;
            _stopWatch = new Stopwatch();
        }

        public void Execute()
        {
            LogTitle();
            EmptyLine();

            Logger.LogAssemblyInfo();
            EmptyLine();

            if (CheckPrecondition())
            {
                StartTimer();

                LogSubTitle(InputParameters);
                LogInputParameters();
                EmptyLine();

                LogSubTitle(Processing);
                Action();
                EmptyLine();

                LogSubTitle(OutputParameters);
                LogOutputParameters();
                EmptyLine();

                LogSubTitle(Performance);
                StopTimer();
                Logger.LogUserMessage($"Total elapsed time={_stopWatch.Elapsed}");
                Logger.LogResourceUsage();
                EmptyLine();
            }
        }

        public void Report(ProgressInfo progress)
        {
            WriteToConsole(
                progress.Percentage.HasValue
                    ? $"{progress.ActionText} {progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType} {progress.Percentage.Value}%"
                    : $"{progress.ActionText} {progress.CurrentItemCount} {progress.ItemType}", true, progress.Done);
        }

        protected abstract bool CheckPrecondition();
        protected abstract void LogInputParameters();
        protected abstract void Action();
        protected abstract void LogOutputParameters();

        private void StartTimer()
        {
            _stopWatch.Start();
        }

        private void StopTimer()
        {
            _stopWatch.Stop();
        }

        private void LogTitle()
        {
            Logger.LogUserMessage(_title);
            Underline('=', _title.Length);
        }

        private void LogSubTitle(string text)
        {
            Logger.LogUserMessage(text);
            Underline('-', text.Length);
        }

        private void Underline(char character, int length)
        {
            Logger.LogUserMessage(new String(character, length));
        }

        private void EmptyLine()
        {
            Logger.LogUserMessage("");
        }

        private void WriteToConsole(string text, bool overwrite, bool endOfLine)
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
                Console.Write(outputBuilder);

                if (endOfLine)
                {
                    outputBuilder.Append("\n");
                    Logger.LogUserMessage(outputBuilder.ToString());
                }

                _currentText = outputBuilder.ToString();
            }
        }
    }
}
