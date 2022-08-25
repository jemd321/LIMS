using System;
using System.Windows.Input;

namespace LIMS.Command
{
    /// <summary>
    /// Implementation of the WFP ICommand interface to provide delegate commands.
    /// </summary>
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
        /// Fired when the executable state of the command changes. Call RaiseCanExecuteChanged to update listeners.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise the CanExecuteChanged event, indicating that the executable state of the command has changed.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">A parameter to pass to the execute delegate.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Determines whether the command can execute.
        /// </summary>
        /// <param name="parameter">Optional parameter to pass to the canExecute delegate.</param>
        /// <returns><c>true</c> if the command can execute or a null delegate was passed in the constructor; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }
    }
}
