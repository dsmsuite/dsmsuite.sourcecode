using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class ElementSearchViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private string _searchText;
        private bool _caseSensitiveSearch;
        private string _elementTypeFilter;
        private ObservableCollection<string> _searchMatches;
        private SearchState _searchState;
        private string _searchResult;

        public event EventHandler SearchUpdated;

        public ElementSearchViewModel(IDsmApplication application)
        {
            _application = application;

            ClearSearchCommand = new RelayCommand<object>(ClearSearchExecute);
            FoundElement = null;
            ElementTypeFilter = null;
        }

        public ICommand ClearSearchCommand { get; }
        public IDsmElement FoundElement { get; private set; }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); OnSearchTextUpdated(); }
        }

        public bool CaseSensitiveSearch
        {
            get { return _caseSensitiveSearch; }
            set { _caseSensitiveSearch = value; OnPropertyChanged(); OnSearchTextUpdated(); }
        }

        public string ElementTypeFilter
        {
            get { return _elementTypeFilter; }
            private set { _elementTypeFilter = value; OnPropertyChanged(); OnSearchTextUpdated(); }
        }

        public ObservableCollection<string> SearchMatches
        {
            get { return _searchMatches; }
            private set { _searchMatches = value; OnPropertyChanged(); }
        }

        public SearchState SearchState
        {
            get { return _searchState; }
            set { _searchState = value; OnPropertyChanged(); }
        }

        public string SearchResult
        {
            get { return _searchResult; }
            set { _searchResult = value; OnPropertyChanged(); }
        }

        private void OnSearchTextUpdated()
        {
            IList<IDsmElement> matchingElements = _application.SearchElements(SearchText, CaseSensitiveSearch, _elementTypeFilter);
            if (SearchText != null)
            {
                List<string> matchingElementNames = new List<string>();
                foreach (IDsmElement matchingElement in matchingElements)
                {
                    matchingElementNames.Add(matchingElement.Fullname);
                }
                SearchMatches = new ObservableCollection<string>(matchingElementNames);

                FoundElement = null;
                if (SearchText.Length == 0)
                {
                    SearchState = SearchState.NoInput;
                    SearchResult = "";
                }
                if (SearchMatches.Count == 0)
                {
                    SearchState = SearchState.NoMatch;
                    SearchResult = SearchText.Length > 0 ? "None found" : "";
                }
                else if (SearchMatches.Count == 1)
                {
                    SearchState = SearchState.SingleMatch;
                    SearchResult = "1 found";
                    FoundElement = matchingElements[0];
                }
                else
                {
                    SearchState = SearchState.MultipleMatches;
                    SearchResult = $"{SearchMatches.Count} found";
                }

                SearchUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ClearSearchExecute(object parameter)
        {
            SearchText = "";
        }
    }
}
