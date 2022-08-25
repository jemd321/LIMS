using System;

namespace LIMS.Dialog
{
    public interface IDialogService
    {
        string ShowOpenFileDialog();

        void ShowActionDialog<TViewModel>(Action<bool> dialogResultCallback);

        void ShowStringIODialog<TViewModel>(Action<bool> dialogResultCallback, string dialogInput, Action<string> dialogOutputCallback);
    }
}
