using System.ComponentModel;
using System.Runtime.CompilerServices;
using LIMS.Model;

namespace LIMS.ViewModel
{
    /// <summary>
    /// Base class for all viewModels that need to implement property changed notifications.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event for when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Load the viewModel, performing initialisation if required for this viewModel.
        /// </summary>
        public virtual void Load()
        {
        }

        /// <summary>
        /// Load the viewModel, performing initialisation if required for this viewModel, supplying an analytical run to be loaded.
        /// </summary>
        /// <param name="analyticalRun">The analytical run that will be used by this viewModel when loading.</param>
        public virtual void Load(AnalyticalRun analyticalRun)
        {
        }

        /// <summary>
        /// Raises the property changed event, signalling that the property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
