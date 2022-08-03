using LIMS.Data;
using LIMS.Factory;
using LIMS.Model;
using LIMS.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using System.Windows;

namespace LIMS
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MainWindow>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<RegressionViewModel>();

            services.AddTransient<IRegressionDataProvider, AnalystDataProvider>();
            services.AddTransient<IRegressionFactory, RegressionFactory>();
            services.AddTransient<IFileDataService, FileDataService>();
            services.AddTransient<IFileSystem, FileSystem>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
