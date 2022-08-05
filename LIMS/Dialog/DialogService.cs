using LIMS.View.Dialog;
using LIMS.ViewModel;
using LIMS.ViewModel.DialogViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LIMS.Dialog
{
    public interface IDialogService
    {
        void ShowDialog<TViewModel>(Action<string> callback, string optionalMessage);
    }
    public class DialogService : IDialogService
    {
        private static Dictionary<Type, Type> _mappings = new();

        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterDialog<TView, TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowDialog<TViewModel>(Action<string> callback, string optionalMessage = null)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowDialogInternal(viewType, callback, typeof(TViewModel), optionalMessage);
        }

        private void ShowDialogInternal(Type viewType, Action<string> callback, Type viewModelType, string optionalMessage = null)
        {
            var dialog = new DialogWindow();
            EventHandler closeEventHandler = null;
            closeEventHandler = (sender, e) =>
            {
                callback(dialog.DialogResult.ToString());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;

            var content = ServiceProvider.GetService(viewType);
            
            if (viewModelType != null)
            {
                var viewModel = ServiceProvider.GetService(viewModelType) as IDialogViewModel;
                viewModel.Load();
                if (optionalMessage is not null)
                {
                    viewModel.OptionalMessage = optionalMessage;
                }
                var viewContent = content as FrameworkElement;
                viewContent.DataContext = viewModel;

            }

            dialog.Content = content;
            dialog.Owner = Application.Current.MainWindow;

            dialog.ShowDialog();
        }
    }
}
