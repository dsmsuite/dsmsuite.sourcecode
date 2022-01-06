using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.Application.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListViewModel : ViewModelBase
    {
        private readonly ElementListViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;

        public ElementListViewModel(ElementListViewModelType viewModelType, IDsmApplication application, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _viewModelType = viewModelType;
            _application = application;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            Title = "Element List";

            IEnumerable<IDsmElement> elements;
            switch (viewModelType)
            {
                case ElementListViewModelType.RelationConsumers:
                    SubTitle = $"Consumers in relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    elements = _application.GetRelationConsumers(_selectedConsumer, _selectedProvider);
                    break;
                case ElementListViewModelType.RelationProviders:
                    SubTitle = $"Providers in relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    elements = _application.GetRelationProviders(_selectedConsumer, _selectedProvider);
                    break;
                case ElementListViewModelType.ElementConsumers:
                    SubTitle = $"Consumers of {_selectedProvider.Fullname}";
                    elements = _application.GetElementConsumers(_selectedProvider);
                    break;
                case ElementListViewModelType.ElementProvidedInterface:
                    SubTitle = $"Provided interface of {_selectedProvider.Fullname}";
                    elements = _application.GetElementProvidedElements(_selectedProvider);
                    break;
                case ElementListViewModelType.ElementRequiredInterface:
                    SubTitle = $"Required interface of {_selectedProvider.Fullname}";
                    elements = _application.GetElementProviders(_selectedProvider);
                    break;
                default:
                    SubTitle = "";
                    elements = new List<IDsmElement>();
                    break;
            }

            List<ElementListItemViewModel> elementViewModels = new List<ElementListItemViewModel>();

            foreach (IDsmElement element in elements)
            {
                elementViewModels.Add(new ElementListItemViewModel(element));
            }

            elementViewModels.Sort();

            int index = 1;
            foreach (ElementListItemViewModel viewModel in elementViewModels)
            {
                viewModel.Index = index;
                index++;
            }

            Elements = new ObservableCollection<ElementListItemViewModel>(elementViewModels);

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<ElementListItemViewModel> Elements { get;  }

        public ElementListItemViewModel SelectedElement { get; set; }

        public ICommand CopyToClipboardCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ElementListItemViewModel viewModel in Elements)
            {
                builder.AppendLine($"{viewModel.Index, -5}, {viewModel.ElementName, -100}, {viewModel.ElementType, -30}, {viewModel.Properties,-150}");
            }
            Clipboard.SetText(builder.ToString());
        }
    }
}
