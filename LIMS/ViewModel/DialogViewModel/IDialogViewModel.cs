using System;

namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// Interface for a viewModel for a basic custom dialog with no input or output required.
    /// </summary>
    public interface IDialogViewModel
    {
        /// <summary>
        /// Event handler for the dialog accepted event, indicating that the user has accepted the dialog.
        /// </summary>
        event EventHandler DialogAccepted;

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        void Load();
    }
}
