using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BHWPF.UI.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action commandTask;

 
        public RelayCommand(Action t_workToDo)
        {
            commandTask = t_workToDo;

        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove { CommandManager.RequerySuggested -= value; }
        }


        protected void OnCanExecuteChanged(Object sender, EventArgs e)
        {
            // this.CanExecuteChanged?.Invoke(this, new EventArgs() );
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(Object parameter)
        {
            commandTask();
        }
    }

}