using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LIMS.ViewModel
{
    /// <summary>
    /// Base Class for viewModels that need to implement validation logic for controls.
    /// </summary>
    public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new();

        /// <summary>
        /// Event that is fired when the collection of errors on the viewModel has been updated.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets a value indicating whether there are currently any errors in the viewModel.
        /// </summary>
        public bool HasErrors => _errorsByPropertyName.Any();

        /// <summary>
        /// Gets an enumeration of the errors for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that the errors should be listed for.</param>
        /// <returns>An enumeration of the errors for this property.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return propertyName is not null && _errorsByPropertyName.ContainsKey(propertyName)
            ? _errorsByPropertyName[propertyName]
            : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets all the errors for the supplied property name as a list of strings.
        /// </summary>
        /// <param name="propertyName">The name of the property that the errors should be listed for.</param>
        /// <returns>A list of the error strings for this property.</returns>
        public List<string> GetErrorsAsList(string propertyName)
        {
            var errors = GetErrors(propertyName);

            // Enumerate through GetErrors collection and cast to string
            var errorList = new List<string>();
            foreach (string error in errors)
            {
                errorList.Add(error);
            }

            return errorList;
        }

        /// <summary>
        /// Raises the errors changed event to signal that the list of errors has updated.
        /// </summary>
        /// <param name="e">The custom <see cref="DataErrorsChangedEventArgs"/> event args with the name of the property.</param>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Add a new error to the collection of errors for the supplied property name.
        /// </summary>
        /// <param name="error">The name of the validation error.</param>
        /// <param name="propertyName">The property name that has a validation error.</param>
        protected void AddError(string error, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                return;
            }

            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                // This property has not yet had any errors - set it up.
                _errorsByPropertyName[propertyName] = new List<string>();
            }

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                // Add the error to the collection and raise an event to signal the collection has changed.
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
                RaisePropertyChanged(nameof(HasErrors));
            }
        }

        /// <summary>
        /// Reset the errors for the chosen property.
        /// </summary>
        /// <param name="propertyName">The property to clear the error collection for.</param>
        protected void ClearErrors([CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                return;
            }

            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
                RaisePropertyChanged(nameof(HasErrors));
            }
        }
    }
}
