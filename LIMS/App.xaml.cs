using System.IO.Abstractions;
using System.Windows;
using LIMS.Data;
using LIMS.Dialog;
using LIMS.Factory;
using LIMS.Model;
using LIMS.View.Dialog;
using LIMS.ViewModel;
using LIMS.ViewModel.DialogViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS
{
    /// <summary>
    /// The base class for the application.
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ConfigureDialogService();
        }

        /// <summary>
        /// Raises with override the onstartup event of the appliaction, setting up the mainwindow.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MainWindow>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<IRegressionViewModel, RegressionViewModel>();
            services.AddTransient<IRegressionDataViewModel, RegressionDataViewModel>();
            services.AddTransient<IRegressionInformationViewModel, RegressionInformationViewModel>();
            services.AddTransient<IRegressionGraphViewModel, RegressionGraphViewModel>();

            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<IDataImporter, AnalystDataImporter>();
            services.AddTransient<IRegressionFactory, RegressionFactory>();
            services.AddTransient<IDataService, FileDataService>();
            services.AddTransient<IFileSystem, FileSystem>();

            services.AddTransient<ProjectEditDialogViewModel>();
            services.AddTransient<ProjectEditDialog>();
            services.AddTransient<OpenAnalyticalRunDialogViewModel>();
            services.AddTransient<OpenAnalyticalRunDialog>();
            services.AddTransient<SaveAnalyticalRunDialogViewModel>();
            services.AddTransient<SaveAnalyticalRunDialog>();
            services.AddTransient<ErrorMessageDialog>();
            services.AddTransient<ErrorMessageDialogViewModel>();
        }

        private void ConfigureDialogService()
        {
            DialogService.ServiceProvider = _serviceProvider;
            DialogService.RegisterDialog<ProjectEditDialog, ProjectEditDialogViewModel>();
            DialogService.RegisterDialog<OpenAnalyticalRunDialog, OpenAnalyticalRunDialogViewModel>();
            DialogService.RegisterDialog<SaveAnalyticalRunDialog, SaveAnalyticalRunDialogViewModel>();
            DialogService.RegisterDialog<ErrorMessageDialog, ErrorMessageDialogViewModel>();
        }
    }
}