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
        void ShowActionDialog<TViewModel>(Action<bool> dialogResultCallback);
        void ShowStringIODialog<TViewModel>(Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback);
    }
    public class DialogService : IDialogService
    {
        private static Dictionary<Type, Type> _mappings = new();

        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterDialog<TView, TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowActionDialog<TViewModel>(Action<bool> dialogResultCallback)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowActionDialogInternal(viewType, typeof(TViewModel), dialogResultCallback);
        }

        public void ShowStringIODialog<TViewModel>(Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowStringIODialogInternal(viewType, typeof(TViewModel), dialogResultCallback, dialogInput, dialogOutputCallback);
        }

        private void ShowActionDialogInternal(Type viewType, Type viewModelType, Action<bool> dialogResultCallback)
        {
            var dialog = new DialogWindow();

            var content = ServiceProvider.GetService(viewType);
            if (viewModelType != null)
            {
                // cast to ViewModelBase so we can call the load method to setup our VM before passing to the view.
                var viewModel = ServiceProvider.GetService(viewModelType) as ViewModelBase;
                viewModel.Load();
                var viewContent = content as FrameworkElement;
                viewContent.DataContext = viewModel;
            }
            EventHandler closeEventHandler = null;
            closeEventHandler = (sender, e) =>
            {
                dialogResultCallback(dialog.DialogResult.GetValueOrDefault());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;
            ShowDialog(dialog, content);
        }

        private void ShowStringIODialogInternal(Type viewType, Type viewModelType, Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback)
        {
            var dialog = new DialogWindow();

            var content = ServiceProvider.GetService(viewType);
            // Requires cast to interface defining input and output string properties on the viewmodel that we can access here
            var viewModel = ServiceProvider.GetService(viewModelType) as IStringIODialogViewModel;
            viewModel.DialogInput = dialogInput;
            viewModel.Load();

            var viewContent = content as FrameworkElement;
            viewContent.DataContext = viewModel;

            EventHandler closeEventHandler = null;
            closeEventHandler = (sender, e) =>
            {
                dialogResultCallback(dialog.DialogResult.GetValueOrDefault());
                dialogOutputCallback(viewModel.DialogOutput);
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;
            ShowDialog(dialog, content);
        }

        private void ShowDialog(DialogWindow dialog, object content)
        {
            dialog.Content = content;
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }

        private void SetupClosedEvent(Action<string> callback, DialogWindow dialog)
        {

        }
    }
}
