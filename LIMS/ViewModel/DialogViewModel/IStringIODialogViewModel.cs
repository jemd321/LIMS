namespace LIMS.ViewModel.DialogViewModel
{
    /// <summary>
    /// An interface for a viewModel for a custom dialog that supports string input or output from the dialog.
    /// </summary>
    public interface IStringIODialogViewModel : IDialogViewModel
    {
        /// <summary>
        /// Gets or sets the input sent to the dialog open opening.
        /// </summary>
        string DialogInput { get; set; }

        /// <summary>
        /// Gets or sets the output of the dialog returned when the dialog has been accepted - usually returning a user selection.
        /// </summary>
        string DialogOutput { get; set; }
    }
}
