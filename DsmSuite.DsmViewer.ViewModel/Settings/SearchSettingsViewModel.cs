using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    public class SearchSettingsViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private bool _caseSensitiveSearch;
        private SearchMode _selectedSearchMode;

        public SearchSettingsViewModel(IDsmApplication application)
        {
            _application = application;

            CaseSensitiveSearch = _application.CaseSensitiveSearch;
            SupportedSearchModes = new ObservableCollection<SearchMode>() { SearchMode.All, SearchMode.Bookmarked, SearchMode.Annotated };
            SelectedSearchMode = _application.SelectedSearchMode;

            _application.CaseSensitiveSearch = CaseSensitiveSearch;

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public ICommand AcceptChangeCommand { get; }

        public bool CaseSensitiveSearch
        {
            get { return _caseSensitiveSearch; }
            set { _caseSensitiveSearch = value; OnPropertyChanged(); }
        }

        public ObservableCollection<SearchMode> SupportedSearchModes { get; set; }
        public SearchMode SelectedSearchMode
        {
            get { return _selectedSearchMode; }
            set { _selectedSearchMode = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.CaseSensitiveSearch = CaseSensitiveSearch;
            _application.SelectedSearchMode = SelectedSearchMode;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}
