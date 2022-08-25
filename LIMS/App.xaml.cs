using LIMS.Data;
using LIMS.Dialog;
using LIMS.View.Dialog;
using LIMS.Factory;
using LIMS.Model;
using LIMS.ViewModel;
using LIMS.ViewModel.DialogViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.Windows;

namespace LIMS
{
    public partial class App : Application
    {
        public readonly ServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ConfigureDialogService();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MainWindow>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<IRegressionViewModel, RegressionViewModel>();

            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<IDataImporter, AnalystDataImporter>();
            services.AddTransient<IRegressionFactory, RegressionFactory>();
            services.AddTransient<IDataProvider, FileDataProvider>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IFileSystem, FileSystem>();

            services.AddTransient<ProjectCreationDialogViewModel>();
            services.AddTransient<ProjectCreationDialog>();
            services.AddTransient<OpenAnalyticalRunDialogViewModel>();
            services.AddTransient<OpenAnalyticalRunDialog>();
            services.AddTransient<SaveAnalyticalRunDialogViewModel>();
            services.AddTransient<SaveAnalyticalRunDialog>();
        }

        private void ConfigureDialogService()
        {
            DialogService.ServiceProvider = _serviceProvider;
            DialogService.RegisterDialog<ProjectCreationDialog, ProjectCreationDialogViewModel>();
            DialogService.RegisterDialog<OpenAnalyticalRunDialog, OpenAnalyticalRunDialogViewModel>();
            DialogService.RegisterDialog<SaveAnalyticalRunDialog, SaveAnalyticalRunDialogViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
