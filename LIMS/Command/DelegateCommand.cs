using System;
using System.Windows.Input;

namespace LIMS.Command
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">Action representing the command to be executed.</param>
        /// <param name="canExecute">Function determining if the command is able to execute.</param>
        /// <exception cref="ArgumentNullException">Throws if command provided is null.</exception>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event for when the ability of the command to execute has been changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise CanExecuteChanged event when the ability of the command to be executed may change.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
