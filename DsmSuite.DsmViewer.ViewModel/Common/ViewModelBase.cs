using DsmSuite.DsmViewer.ViewModel.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
