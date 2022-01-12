using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class ElementSearchViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _searchPathElement;
        private IDsmElement _selectedElement;
        private readonly bool _markMatchingElements;

        private string _searchText;
        private bool _caseSensitiveSearch;
        private string _selectedElementType;
        private ObservableCollection<string> _searchMatches;
        private SearchState _searchState;
        private string _searchResult;

        public event EventHandler SearchUpdated;

        public ElementSearchViewModel(IDsmApplication application, IDsmElement searchPathElement, IDsmElement selectedElement, string preSelectedElementType, bool markMatchingElements)
        {
            _application = application;
            _searchPathElement = (searchPathElement != null) ? searchPathElement : _application.RootElement;
            _selectedElement = selectedElement;
            _markMatchingElements = markMatchingElements;

            if ((searchPathElement != null) && (!searchPathElement.HasChildren))
            {
                _selectedElement = searchPathElement;
            }
            if (_selectedElement != null)
            {
                SearchText = _selectedElement.Fullname;
                SearchState = SearchState.Off;
            }
            else
            {
                SearchText = "";
                SearchState = SearchState.NoInput;
            }

            SearchPath = (searchPathElement != null) ? searchPathElement.Fullname : "";

            ElementTypes = new List<string>(application.GetElementTypes());
            SelectedElementType = preSelectedElementType;

            ClearSearchCommand = new RelayCommand<object>(ClearSearchExecute, ClearSearchCanExecute);
        }

        public List<string> ElementTypes { get; }

        public ICommand ClearSearchCommand { get; }

        public string SearchPath
        {
            get;
        }

        public IDsmElement SelectedElement
        {
            get { return _selectedElement; }
            set { _selectedElement = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    OnSearchTextUpdated();
                }
            }
        }

        public bool CaseSensitiveSearch
        {
            get { return _caseSensitiveSearch; }
            set 
            {
                if (_caseSensitiveSearch != value)
                {
                    _caseSensitiveSearch = value;
                    OnPropertyChanged();
                    OnSearchTextUpdated();
                }
            }
        }

        public string SelectedElementType
        {
            get { return _selectedElementType; }
            set 
            {
                if (_selectedElementType != value)
                {
                    _selectedElementType = value;
                    OnPropertyChanged();
                    OnSearchTextUpdated();
                }
            }
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
            if (SearchState != SearchState.Off)
            {
                IList<IDsmElement> matchingElements = _application.SearchElements(SearchText, _searchPathElement, CaseSensitiveSearch, SelectedElementType, _markMatchingElements);
                if (SearchText != null)
                {
                    List<string> matchingElementNames = new List<string>();
                    foreach (IDsmElement matchingElement in matchingElements)
                    {
                        matchingElementNames.Add(matchingElement.Fullname);
                    }
                    SearchMatches = new ObservableCollection<string>(matchingElementNames);

                    if (SearchText.Length == 0)
                    {
                        SearchState = SearchState.NoInput;
                        SearchResult = "";
                        SelectedElement = null;
                    }
                    else if (SearchMatches.Count == 0)
                    {
                        SearchState = SearchState.NoMatch;
                        SearchResult = SearchText.Length > 0 ? "None found" : "";
                        SelectedElement = null;
                    }
                    else
                    {
                        SearchState = SearchState.Match;
                        SearchResult = $"{SearchMatches.Count} found";

                        if (SearchMatches.Count == 1)
                        {
                            SearchText = matchingElements[0].Fullname;
                        }
                    }

                    foreach (IDsmElement matchingElement in matchingElements)
                    {
                        if (SearchText == matchingElement.Fullname)
                        {
                            SelectedElement = matchingElement;
                        }
                    }

                    SearchUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ClearSearchExecute(object parameter)
        {
            SearchText = "";
        }

        private bool ClearSearchCanExecute(object parameter)
        {
            return SearchState != SearchState.Off;
        }
    }
}
