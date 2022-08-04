using LIMS.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LIMS.Dialog
{
    /// <summary>
    /// Interaction logic for ProjectCreationDialog.xaml
    /// </summary>
    public partial class ProjectCreationDialog : UserControl
    {
        public ProjectCreationDialog()
        {
            InitializeComponent();
        }

        public string SelectedProjectID { get; set; }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            SelectedProjectID = ProjectID.Text;
            //DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = false;
        }
    }
}
