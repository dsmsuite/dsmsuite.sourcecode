using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ActionListViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private IEnumerable<ActionListItemViewModel> _actions;

        public ActionListViewModel(IDsmApplication application)
        {
            Title = "Actions";

            _application = application;
            _application.ActionPerformed += OnActionPerformed;

            UpdateActionList();
        }

        private void OnActionPerformed(object sender, System.EventArgs e)
        {
            UpdateActionList();
        }

        public string Title { get; }

        public IEnumerable<ActionListItemViewModel> Actions
        {
            get { return _actions; }
            set { _actions = value; OnPropertyChanged(); }
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
