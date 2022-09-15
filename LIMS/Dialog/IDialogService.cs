using System;

namespace LIMS.Dialog
{
    /// <summary>
    /// Describes a services that handles the creation and display of dialogs, allowing a supporting viewModel to be used if required.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Displays the Win32 <see cref="OpenFileDialog"/> and returning the user selected file path.
        /// </summary>
        /// <returns>The user selected file path as a string.</returns>
        string ShowOpenFileDialog();

        /// <summary>
        /// Displays a custom dialog that requires no input or output.
        /// </summary>
        /// <typeparam name="TViewModel">The name of the dialogViewModel to be resolved,
        /// which must implement <see cref="IDialogViewModel"/>.</typeparam>
        /// <param name="dialogResultCallback">Optional callback to indicate whether the user accepted or cancelled the dialog if required.</param>
        void ShowActionDialog<TViewModel>(Action<bool> dialogResultCallback);

        /// <summary>
        /// Displays a custom dialog that accepts string input and can return a string.
        /// </summary>
        /// <typeparam name="TViewModel">The name of the dialogViewModel to be resolved,
        /// which must implement <see cref="IStringIODialogViewModel"/>.</typeparam>
        /// <param name="dialogResultCallback">Optional callback to indicate whether the user accepted or cancelled the dialog if required.</param>
        /// <param name="dialogInput">The string input passed to the viewModel.</param>
        /// <param name="dialogOutputCallback">The string output returned from the viewModel.</param>
        void ShowStringIODialog<TViewModel>(Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback);
    }
}
