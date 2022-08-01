using LIMS.ViewModel;

namespace LIMS
{
    public partial class MainWindow
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _viewModel = mainViewModel;
            DataContext = _viewModel;
        }
    }
}
