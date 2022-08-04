using LIMS.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace LIMS.Dialog
{
    public class MessageDialogService : IMessageDialogService
    {
        public Project ShowProjectCreationDialog(ObservableCollection<Project> projects)
        {
            var projectDialog = new ProjectCreationDialog(projects)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            };
            bool projectCreated = projectDialog.ShowDialog().GetValueOrDefault();
            if (projectCreated)
            {
                return new Project(projectDialog.SelectedProjectID);
            }
            else return null;
        }
    }
}
