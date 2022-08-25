namespace LIMS.ViewModel.DialogViewModel
{
    public interface IStringIODialogViewModel : IDialogViewModel
    {
        string DialogInput { get; set; }

        string DialogOutput { get; set; }
    }
}
