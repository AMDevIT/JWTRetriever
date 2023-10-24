using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AMDevIT.Security.JWTRetriever.Common
{
    public class DelegateCommand
      : ICommand
    {
        #region Events

        public event EventHandler? CanExecuteChanged;

        #endregion

        #region Fields

        private Func<object?, bool>? canExecuteFunction;
        private Action<object?> executeAction;
        private bool canExecutePreviousValue;

        #endregion

        #region .ctor

        public DelegateCommand(Action<object?> executeAction)
            : this(executeAction, null)
        {
        }

        public DelegateCommand(Action<object?> executeAction, Func<object?, bool>? canExecute)
        {
            this.executeAction = executeAction;
            this.canExecuteFunction = canExecute;
        }

        #endregion

        #region Methods

        public bool CanExecute(object? parameter = null)
        {
            try
            {
                bool result = true;

                if (this.canExecuteFunction != null)
                {
                    bool functionResult = canExecuteFunction(parameter);

                    if (this.canExecutePreviousValue != functionResult)
                    {
                        this.canExecutePreviousValue = functionResult;
                        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    }

                    result = this.canExecutePreviousValue;
                }
                return result;
            }
            catch (NullReferenceException)
            {
                return true;
            }
            catch (Exception exc)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine($"[{nameof(this.CanExecute)}] Error occured: " +
                                    $"{Environment.NewLine}{exc}");
                return true;
            }
        }

        public void Execute(object? parameter)
        {
            this.executeAction?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            try
            {
                this.CanExecuteChanged?.Invoke(this, e);
            }
            catch (Exception exc)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine($"[{nameof(this.OnCanExecuteChanged)}] Error occured: " +
                                    $"{Environment.NewLine}{exc}");
            }
        }

        #endregion
    }
}
