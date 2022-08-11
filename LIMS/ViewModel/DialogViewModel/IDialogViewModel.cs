using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.ViewModel.DialogViewModel
{
    public interface IDialogViewModel
    {
        void Load();
        event EventHandler DialogAccepted;
    }
}
