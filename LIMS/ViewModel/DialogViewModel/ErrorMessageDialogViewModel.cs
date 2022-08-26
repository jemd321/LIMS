using System;

namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// ViewModel for the ErrorMessageDialog control.
    /// </summary>
    public class ErrorMessageDialogViewModel : ViewModelBase, IStringIODialogViewModel
    {
        /// <summary>
        /// Fired when the user accepts the dialog. Not required here as this dialog has no yes/no.
        /// </summary>
        public event EventHandler DialogAccepted;

        /// <summary>
        /// Gets or sets the string input passed to the dialog, in this case the error message to display.
        /// </summary>
        public string DialogInput { get; set; }

        /// <summary>
        /// Gets or sets the result returned from the dialog - not required for this error dialog.
        /// </summary>
        public string DialogOutput { get; set; }
    }
}
