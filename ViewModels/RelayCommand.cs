using System;
using System.Windows.Input;

namespace LabyrinthGame
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
    }
   
}