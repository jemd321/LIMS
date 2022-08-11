namespace LIMS.ViewModel.DialogViewModel
{
    public interface IStringIODialogViewModel
    {
        void Load();
        string DialogInput { get; set; }
        string DialogOutput { get; set; }
    }
}
