using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ActionListViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private IEnumerable<ActionListItemViewModel> _actions;

        public ActionListViewModel(IDsmApplication application)
        {
            Title = "Edit history";
            SubTitle = "Modifications on model";

            _application = application;
            _application.ActionPerformed += OnActionPerformed;

            UpdateActionList();
            
            CopyToClipboardCommand =  new RelayCommand<object>(CopyToClipboardExecute);
            ClearCommand = new RelayCommand<object>(ClearExecute);
        }

        private void OnActionPerformed(object sender, System.EventArgs e)
        {
            UpdateActionList();
        }

        public string Title { get; }
        public string SubTitle { get; }

        public IEnumerable<ActionListItemViewModel> Actions
        {
            get { return _actions; }
            set { _actions = value; OnPropertyChanged(); }
        }

        public ICommand CopyToClipboardCommand { get; }
        public ICommand ClearCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach(ActionListItemViewModel viewModel in Actions)
            {
                builder.AppendLine($"{viewModel.Index, -5}, {viewModel.Action, -30}, {viewModel.Details}");
            }
            Clipboard.SetText(builder.ToString());
        }

        private void ClearExecute(object parameter)
        {
            _application.ClearActions();
            UpdateActionList();
        }

        private void UpdateActionList()
        {
            var actionViewModels = new List<ActionListItemViewModel>();
            int index = 1;
            foreach (IAction action in _application.GetActions())
            {
                actionViewModels.Add(new ActionListItemViewModel(index, action));
                index++;
            }

            Actions = actionViewModels;
        }
    }
}
