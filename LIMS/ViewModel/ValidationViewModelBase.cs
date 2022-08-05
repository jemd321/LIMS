using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LIMS.ViewModel
{
    public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new();

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return propertyName is not null && _errorsByPropertyName.ContainsKey(propertyName)
            ? _errorsByPropertyName[propertyName]
            : Enumerable.Empty<string>();
        }

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

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        protected void AddError(string error, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                return;
            }
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
                RaisePropertyChanged(nameof(HasErrors));
            }
        }

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
