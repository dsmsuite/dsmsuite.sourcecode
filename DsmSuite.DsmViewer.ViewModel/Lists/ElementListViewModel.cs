using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using System.Collections.ObjectModel;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListViewModel : ViewModelBase
    {
        public ElementListViewModel(string subtitle, IEnumerable<IDsmElement> elements)
        {
            Title = "Element List";
            SubTitle = subtitle;

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
