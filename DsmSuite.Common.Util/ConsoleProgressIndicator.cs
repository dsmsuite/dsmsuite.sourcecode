using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (_progress != progress.Progress)
            {
                _progress = progress.Progress;
                Console.Write($"\r {progress.ActionText} progress={progress.Progress:000}%");
            }

            if (_progress == 100)
            {
                Console.Write($"{progress.ActionText} progress={progress.Progress:000}%");
                Console.Write("Done");
            }
        }
    }
}
