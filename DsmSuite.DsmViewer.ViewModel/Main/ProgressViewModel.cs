using System;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class ProgressViewModel : ViewModelBase
    {
        public event EventHandler<bool> BusyChanged;

        private bool _busy;
        private string _action;
        private string _text;
        private int _progressValue;
        private string _progressText;

        public void Update(int elementCount,
                           int relationCount,
                           int progress)
        {
            ProgressValue = progress;
            ProgressText = $"{progress} %";
            Busy = (progress > 0) && (progress < 100);

            Text = $"{Action} {elementCount} elements and {relationCount} relations";
        }

        public string Action
        {
            get { return _action; }
            set { _action = value; OnPropertyChanged(); }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }

        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (_busy != value)
                {
                    _busy = value;
                    OnPropertyChanged();
                    BusyChanged?.Invoke(this, _busy);
                }
            }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged(); }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set { _progressText = value; OnPropertyChanged(); }
        }
    }
}
