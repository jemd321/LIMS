using LIMS.View.Dialog;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LIMS.Dialog
{
    public interface IDialogService
    {
        void ShowDialog<TViewModel>(Action<string> callback);
    }
    public class DialogService : IDialogService
    {
        static Dictionary<Type, Type> _mappings = new();

        public static void RegisterDialog<TView, TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowDialog<TViewModel>(Action<string> callback)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowDialogInternal(viewType, callback, typeof(TViewModel));
        }

        private static void ShowDialogInternal(Type viewType, Action<string> callback, Type viewModelType)
        {
            var dialog = new DialogWindow();
            EventHandler closeEventHandler = null;
            closeEventHandler = (sender, e) =>
            {
                callback(dialog.DialogResult.ToString());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;

            var content = Activator.CreateInstance(viewType);
            

            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType);
                (content as FrameworkElement).DataContext = viewModel;
            }

            dialog.Content = content;

            dialog.ShowDialog();
        }
    }
}
