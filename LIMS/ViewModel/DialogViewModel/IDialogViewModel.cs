namespace LIMS.ViewModel.DialogViewModel
{
    public interface IDialogViewModel
    {
        void Load();
        string OptionalMessage { get; set; }
    }
}
