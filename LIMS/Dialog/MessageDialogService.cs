using LIMS.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace LIMS.Dialog
{
    public class MessageDialogService : IMessageDialogService
    {
        public Project ShowProjectCreationDialog()
        {
            return new Project("Temp");
            //var projectDialog = new ProjectCreationDialog();
            //bool projectCreated = projectDialog.ShowDialog().GetValueOrDefault();
            //if (projectCreated)
            //{
            //    return new Project(projectDialog.SelectedProjectID);
            //}
            //else return null;
        }
    }
}
