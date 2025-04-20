

using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private readonly List<RelayCommand<object>> _commands = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        protected ICommand RegisterCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            var command = new RelayCommand<object>(execute, canExecute);
            _commands.Add(command);
            return command;
        }

        protected ICommand RegisterCommand(Action<object?> execute)
        {
            var command = new RelayCommand<object>(execute);
            _commands.Add(command);
            return command;
        }

        protected void NotifyCommandsCanExecuteChanged()
        {
            foreach (var command in _commands)
            {
                command.NotifyCanExecuteChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
