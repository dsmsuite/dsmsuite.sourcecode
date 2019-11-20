using System.Windows;
using System.Windows.Controls;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        private MainViewModel _mainViewModel;

        public InfoView()
        {
            InitializeComponent();
        }

        private void InfoViewOnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel = this.DataContext as MainViewModel;
        }
    }
}
