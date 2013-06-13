using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools {
    public class RelayCommand<T> : DelegateCommand<T>, ICommand {
        public RelayCommand(Action<T> executeMethod)
            : base(executeMethod) {
        }
        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod) {
            CommandManager.RequerySuggested += CommandManagerRequerySuggested;
        }
        void CommandManagerRequerySuggested(object sender, EventArgs e) {
            RaiseCanExecuteChanged();
        }
    }
    public class RelayCommand : DelegateCommand {
        public RelayCommand(Action executeMethod)
            : base(executeMethod) {
            CommandManager.RequerySuggested += CommandManagerRequerySuggested;
        }
        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod) {
            CommandManager.RequerySuggested += CommandManagerRequerySuggested;
        }
        void CommandManagerRequerySuggested(object sender, EventArgs e) {
            RaiseCanExecuteChanged();
        }
    }
}
