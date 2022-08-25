using System;

namespace LIMS.ViewModel.DialogViewModel
{
    public interface IDialogViewModel
    {
        void Load();

        event EventHandler DialogAccepted;
    }
}
