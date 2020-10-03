using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListViewModel : ViewModelBase
    {
        public ElementListViewModel(string title, IEnumerable<IDsmElement> elements)
        {
            Title = title;

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

            Elements = elementViewModels;

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
        }

        public string Title { get; }

        public List<ElementListItemViewModel> Elements { get;  }

        public ICommand CopyToClipboardCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ElementListItemViewModel viewModel in Elements)
            {
                builder.AppendLine($"{viewModel.Index, -5} {viewModel.ElementName, -100} {viewModel.ElementType, -30}");
            }
            Clipboard.SetText(builder.ToString());
        }
    }
}
