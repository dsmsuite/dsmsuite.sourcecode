using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView
    {
        public ReportView()
        {
            InitializeComponent();
            DataContextChanged += OnReportViewDataContextChanged;
        }

        private void OnReportViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReportViewModel viewModel = DataContext as ReportViewModel;
            if (viewModel != null)
            {
                Title = viewModel.Title;
                Browser.NavigateToString(viewModel.Content);
            }
        }
    }
}
