using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Editing;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for ElementEditView.xaml
    /// </summary>
    public partial class ElementEditView : Window
    {
        public ElementEditView()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            ElementEditViewModel viewModel = DataContext as ElementEditViewModel;
            if ((viewModel != null)&& (viewModel.EditElementCommand != null))
            {
                viewModel.EditElementCommand.Execute(null);
            }
            DialogResult = true;
        }
    }
}
