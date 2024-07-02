using System.ComponentModel;

/// <summary>
/// Базовый класс ViewModel, реализующий интерфейс INotifyPropertyChanged.
/// Этот класс используется для оповещения представления (View) о изменениях свойств модели (Model).
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
    {
    /// <summary>
    /// Событие, вызываемое при изменении значения свойства.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Метод, вызывающий событие PropertyChanged.
    /// </summary>
    /// <param name="propertyName">Имя измененного свойства.</param>
    protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
}
