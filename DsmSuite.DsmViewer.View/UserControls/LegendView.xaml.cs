using System.Windows;
using System.Windows.Controls;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for LegendView.xaml
    /// </summary>
    public partial class LegendView : UserControl
    {
        private MainViewModel _mainViewModel;

        public LegendView()
        {
            InitializeComponent();
        }

        private void LegendView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel = DataContext as MainViewModel;
        }
    }
}
