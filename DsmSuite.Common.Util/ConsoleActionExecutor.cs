using System;
using System.Diagnostics;
using System.Text;

namespace DsmSuite.Common.Util
{
    public class ConsoleActionExecutor<T>
    {
        private readonly string _description;
        private readonly T _settings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly Stopwatch _stopWatch;

        public delegate void ConsoleAction(T settings, IProgress<ProgressInfo> progres);

        public ConsoleActionExecutor(string description, T settings, IProgress<ProgressInfo> progres)
        {
            _description = description;
            _settings = settings;
            _progress = progres;
            _stopWatch = new Stopwatch();
        }

        public void Execute(ConsoleAction action)
        {
            Logger.LogUserMessage(_description);
            Logger.LogUserMessage(new String('-', _description.Length));

            StartTimer();
            action(_settings, _progress);
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
    }
}
