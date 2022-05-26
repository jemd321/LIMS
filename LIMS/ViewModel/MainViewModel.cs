using LIMS.Command;
using Microsoft.Win32;
using System;

namespace LIMS.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(RegressionViewModel regressionViewModel)
        {
            RegressionViewModel = regressionViewModel;
            OpenFileCommand = new DelegateCommand(OpenFile);
        }

        private void OpenFile(object parameter)
        {
            var filedialog = new OpenFileDialog();
            filedialog.ShowDialog();
        }

        public RegressionViewModel RegressionViewModel { get; }
        public DelegateCommand OpenFileCommand { get; }


    }
}
