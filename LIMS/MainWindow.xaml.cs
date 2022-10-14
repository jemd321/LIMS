using LIMS.ViewModel;

namespace LIMS
{
    /// <summary>
    /// The mainwindow containing the application.
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="mainViewModel">The top level viewModel for this application.</param>
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _viewModel = mainViewModel;
            DataContext = _viewModel;
            mainViewModel.Load();
        }
    }
}
