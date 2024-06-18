namespace PIKTestPlugin
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Класс для создания команд, которые могут быть привязаны к элементам управления в WPF.
    /// Реализует интерфейс ICommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // Поля
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        // Конструкторы

        /// <summary>
        /// Конструктор класса RelayCommand.
        /// </summary>
        /// <param name="execute">Действие, выполняемое командой.</param>
        /// <param name="canExecute">Функция, определяющая, может ли команда выполняться.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        // События

        /// <summary>
        /// Событие, вызываемое при изменении состояния выполнения команды.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Методы

        /// <summary>
        /// Определяет, может ли команда выполняться.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns>true, если команда может выполняться; в противном случае — false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
