using System;
using System.Collections.Generic;
using System.Windows;
using LIMS.View.Dialog;
using LIMS.ViewModel.DialogViewModel;
using Microsoft.Win32;

namespace LIMS.Dialog
{
    /// <summary>
    /// Service that handles the creation and showing of dialogs. Win32 dialogs are wrapped, and custom dialogs can be requested
    /// by providing the type of the dialogViewModel to be shown.
    /// </summary>
    /// <remarks>All new custom dialogs need to be registered in the service collection, and in the ConfigureDialogService method of app.xaml.cs.</remarks>
    public class DialogService : IDialogService
    {
        private static readonly Dictionary<Type, Type> _mappings = new();

        /// <summary>
        /// Reference to the service provider, allowing this class to resolve dialog views and viewModels.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Adds a dialogView and dialogViewModel to a dictionary, to associate them.
        /// </summary>
        /// <typeparam name="TView">The type of dialogView to register</typeparam>
        /// <typeparam name="TViewModel">The type of dialogViewModel to associate with the dialogView</typeparam>
        public static void RegisterDialog<TView, TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        /// <summary>
        /// Displays the Win32 <see cref="OpenFileDialog"/> and returning the user selected file path.
        /// </summary>
        /// <returns>The user selected file path as a string.</returns>
        public string ShowOpenFileDialog()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Text documents (.txt)|*.txt",
            };

            bool fileSelected = fileDialog.ShowDialog().GetValueOrDefault();
            return fileSelected ? fileDialog.FileName : string.Empty;
        }

        /// <summary>
        /// Displays a custom dialog that requires no input or output.
        /// </summary>
        /// <typeparam name="TViewModel">The name of the dialogViewModel to be resolved,
        /// which must implement <see cref="IDialogViewModel"/>.</typeparam>
        /// <param name="dialogResultCallback">Optional callback to indicate whether the user accepted or cancelled the dialog if required.</param>
        public void ShowActionDialog<TViewModel>(Action<bool> dialogResultCallback)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowActionDialogInternal(viewType, typeof(TViewModel), dialogResultCallback);
        }

        /// <summary>
        /// Displays a custom dialog that accepts string input and can return a string.
        /// </summary>
        /// <typeparam name="TViewModel">The name of the dialogViewModel to be resolved,
        /// which must implement <see cref="IStringIODialogViewModel"/>.</typeparam>
        /// <param name="dialogResultCallback">Optional callback to indicate whether the user accepted or cancelled the dialog if required.</param>
        /// <param name="dialogInput">The string input passed to the viewModel.</param>
        /// <param name="dialogOutputCallback">The string output returned from the viewModel.</param>
        public void ShowStringIODialog<TViewModel>(Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback)
        {
            var viewType = _mappings[typeof(TViewModel)];
            ShowStringIODialogInternal(viewType, typeof(TViewModel), dialogResultCallback, dialogInput, dialogOutputCallback);
        }

        private static void ShowActionDialogInternal(Type viewType, Type viewModelType, Action<bool> dialogResultCallback)
        {
            // Container window for the dialog view
            var dialog = new DialogWindow();

            // Resolve view and viewModel. VM is cast to interface so load method can be called.
            var view = ServiceProvider.GetService(viewType);
            if (view is null || ServiceProvider.GetService(viewModelType) is not IDialogViewModel viewModel)
            {
                return;
            }

            viewModel.Load();

            // Assign viewModel to View
            var viewContent = view as FrameworkElement;
            viewContent.DataContext = viewModel;

            void DialogClosedEventHandler(object sender, EventArgs e)
            {
                // callback to get true or false result (accept or cancel) from dialog when closed
                dialogResultCallback(dialog.DialogResult.GetValueOrDefault());
                dialog.Closed -= DialogClosedEventHandler;
            }

            dialog.Closed += DialogClosedEventHandler;

            ShowDialog(dialog, view);
        }

        private static void ShowStringIODialogInternal(Type viewType, Type viewModelType, Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback)
        {
            // Container window for the dialog view
            var dialog = new DialogWindow();

            // Resolve view and viewModel. VM is cast to interface so load method and input/output string properties can be accessed here.
            var view = ServiceProvider.GetService(viewType);
            if (view is null || ServiceProvider.GetService(viewModelType) is not IStringIODialogViewModel viewModel)
            {
                return;
            }

            viewModel.DialogInput = dialogInput;
            viewModel.Load();

            var viewContent = view as FrameworkElement;
            viewContent.DataContext = viewModel;

            // Handle dialog accepted event, indicating an output string is available from the dialog.
            void DialogAcceptedEventHandler(object sender, EventArgs e)
            {
                dialog.DialogResult = true;
                dialog.Close();
                viewModel.DialogAccepted -= DialogAcceptedEventHandler;
            }

            viewModel.DialogAccepted += DialogAcceptedEventHandler;

            void DialogClosedEventHandler(object sender, EventArgs e)
            {
                // Callback to return true or false result (accept or cancel) from dialog when closed, then return string output.
                dialogResultCallback(dialog.DialogResult.GetValueOrDefault());
                dialogOutputCallback(viewModel.DialogOutput);
                dialog.Closed -= DialogClosedEventHandler;
            }

            dialog.Closed += DialogClosedEventHandler;

            ShowDialog(dialog, view);
        }

        private static void ShowDialog(DialogWindow dialog, object content)
        {
            dialog.Content = content;
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }
    }
}
