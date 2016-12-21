using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace app.Utils
{
    public sealed class ActionCommand : ICommand
    {
        private Action _action;
        private Func<bool> _isExecutable;

        public Func<bool> IsExecutable
        {
            get { return _isExecutable; }
            set
            {
                _isExecutable = value;
                if (CanExecuteChanged == null)
                    return;
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public ActionCommand(Action action) : this(action, () => true) { }

        public ActionCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _isExecutable = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return IsExecutable();
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }
}
