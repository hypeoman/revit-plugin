using System;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PIKTestPlugin;

/// <summary>
/// Представляет ViewModel для приложения Revit.
/// </summary>
public class ApplicationViewModel : ViewModelBase
{
    private UIApplication _uiApp;
    private UIDocument _uiDoc;
    private Document _doc;

    private string _allApartmentsText = "Количество квартир: -";
    private string _oneRoomApartmentsText = "Количество однокомнатных квартир: -";
    private string _twoRoomsApartmentsText = "Количество двухкомнатных квартир: -";
    private string _threeRoomsApartmentsText = "Количество трехкомнатных квартир: -";
    private string _fourRoomsApartmentsText = "Количество четырехкомнатных квартир: -";
    private string _studioApartmentText = "Количество квартир-студий: -";
    private string _allColoredApartmentsText = "Количество окрашенных квартир: -";
    private string _oneRoomColoredApartmentsText = "Количество окрашенных однокомнатных квартир: -";
    private string _twoRoomsColoredApartmentsText = "Количество окрашенных двухкомнатных квартир: -";
    private string _threeRoomsColoredApartmentsText = "Количество окрашенных трехкомнатных квартир: -";
    private string _fourRoomsColoredApartmentsText = "Количество окрашенных четырехкомнатных квартир: -";
    private string _studioColoredApartmentText = "Количество окрашенных квартир-студий: -";

    /// <summary>
    /// Конструктор для инициализации ViewModel.
    /// </summary>
    /// <param name="uiApp">Текущий экземпляр приложения Revit.</param>
    /// <param name="uiDoc">Текущий документ в Revit.</param>
    /// <param name="doc">Документ, связанный с текущим проектом.</param>
    public ApplicationViewModel(UIApplication uiApp, UIDocument uiDoc, Document doc)
    {
        _uiApp = uiApp;
        _uiDoc = uiDoc;
        _doc = doc;

        AnalyzeCommand = new RelayCommand(obj => AnalyzeApartments());
        ColorCommand = new RelayCommand(obj => ColorApartments());

        ExternalColorEvent = ExternalEvent.Create(new ExternalColorHandler());
        ExternalAnalyzeEvent = ExternalEvent.Create(new ExternalAnalyzeHandler(this));
    }

    /// <summary>
    /// Получает или задает текст, связанный со всеми квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string AllApartmentsText
    {
        get { return _allApartmentsText; }
        set
        {
            if (_allApartmentsText != value)
            {
                _allApartmentsText = value;
                OnPropertyChanged(nameof(AllApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с однокомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string OneRoomApartmentsText
    {
        get { return _oneRoomApartmentsText; }
        set
        {
            if (_oneRoomApartmentsText != value)
            {
                _oneRoomApartmentsText = value;
                OnPropertyChanged(nameof(OneRoomApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с двухкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string TwoRoomsApartmentsText
    {
        get { return _twoRoomsApartmentsText; }
        set
        {
            if (_twoRoomsApartmentsText != value)
            {
                _twoRoomsApartmentsText = value;
                OnPropertyChanged(nameof(TwoRoomsApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с трехкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string ThreeRoomsApartmentsText
    {
        get { return _threeRoomsApartmentsText; }
        set
        {
            if (_threeRoomsApartmentsText != value)
            {
                _threeRoomsApartmentsText = value;
                OnPropertyChanged(nameof(ThreeRoomsApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с четырехкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string FourRoomsApartmentsText
    {
        get { return _fourRoomsApartmentsText; }
        set
        {
            if (_fourRoomsApartmentsText != value)
            {
                _fourRoomsApartmentsText = value;
                OnPropertyChanged(nameof(FourRoomsApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с квартирами-студиями.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string StudioApartmentsText
    {
        get { return _studioApartmentText; }
        set
        {
            if (_studioApartmentText != value)
            {
                _studioApartmentText = value;
                OnPropertyChanged(nameof(StudioApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный со всеми окрашенными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string AllColoredApartmentsText
    {
        get { return _allColoredApartmentsText; }
        set
        {
            if (_allColoredApartmentsText != value)
            {
                _allColoredApartmentsText = value;
                OnPropertyChanged(nameof(AllColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с окрашенными однокомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string OneRoomColoredApartmentsText
    {
        get { return _oneRoomColoredApartmentsText; }
        set
        {
            if (_oneRoomColoredApartmentsText != value)
            {
                _oneRoomColoredApartmentsText = value;
                OnPropertyChanged(nameof(OneRoomColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с окрашенными двухкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string TwoRoomsColoredApartmentsText
    {
        get { return _twoRoomsColoredApartmentsText; }
        set
        {
            if (_twoRoomsColoredApartmentsText != value)
            {
                _twoRoomsColoredApartmentsText = value;
                OnPropertyChanged(nameof(TwoRoomsColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с окрашенными трехкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string ThreeRoomsColoredApartmentsText
    {
        get { return _threeRoomsColoredApartmentsText; }
        set
        {
            if (_threeRoomsColoredApartmentsText != value)
            {
                _threeRoomsColoredApartmentsText = value;
                OnPropertyChanged(nameof(ThreeRoomsColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с окрашенными четырехкомнатными квартирами.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string FourRoomsColoredApartmentsText
    {
        get { return _fourRoomsColoredApartmentsText; }
        set
        {
            if (_fourRoomsColoredApartmentsText != value)
            {
                _fourRoomsColoredApartmentsText = value;
                OnPropertyChanged(nameof(FourRoomsColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Получает или задает текст, связанный с окрашенными квартирами-студиями.
    /// При изменении значения вызывается событие <see cref="OnPropertyChanged"/>.
    /// </summary>
    public string StudioColoredApartmentsText
    {
        get { return _studioColoredApartmentText; }
        set
        {
            if (_studioColoredApartmentText != value)
            {
                _studioColoredApartmentText = value;
                OnPropertyChanged(nameof(StudioColoredApartmentsText));
            }
        }
    }

    /// <summary>
    /// Команда для анализа квартир.
    /// </summary>
    public ICommand AnalyzeCommand { get; }

    /// <summary>
    /// Команда для окраски квартир.
    /// </summary>
    public ICommand ColorCommand { get; }

    /// <summary>
    /// Внешнее событие для окраски квартир.
    /// </summary>
    public ExternalEvent ExternalColorEvent { get; private set; }

    /// <summary>
    /// Внешнее событие для анализа квартир.
    /// </summary>
    public ExternalEvent ExternalAnalyzeEvent { get; private set; }

    /// <summary>
    /// Метод для вызова события окраски квартир.
    /// </summary>
    public void ColorApartments()
    {
        try
        {
            ExternalColorEvent.Raise();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошло исключение: " + ex.Message);
        }
    }

    /// <summary>
    /// Метод для вызова события анализа квартир.
    /// </summary>
    private void AnalyzeApartments()
    {
        try
        {
            ExternalAnalyzeEvent.Raise();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошло исключение: " + ex.Message);
        }
    }
}
