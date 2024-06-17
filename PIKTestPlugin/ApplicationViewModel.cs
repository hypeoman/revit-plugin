using Autodesk.Revit.UI;
using PIKTestPlugin;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Windows;
using System.Windows.Interop;

public class ApplicationViewModel : ViewModelBase
{
    private string _allApartmentsText = "Количество квартир: -";
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

    private string _oneRoomApartmentsText = "Количество однокомнатных квартир: -";
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

    private string _twoRoomsApartmentsText = "Количество двухкомнатных квартир: -";
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

    private string _threeRoomsApartmentsText = "Количество трехкомнатных квартир: -";
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

    private string _fourRoomsApartmentsText = "Количество четырехкомнатных квартир: -";
    public string FourRoomsApartmentsText
    {
        get { return _fourRoomsApartmentsText;  }
        set
        {
            if (_fourRoomsApartmentsText != value)
            {
                _fourRoomsApartmentsText = value;
                OnPropertyChanged(nameof(FourRoomsApartmentsText));
            }
        }
    }

    private string _studioApartmentText = "Количество квартир-студий: -";
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

    private string _allColoredApartmentsText = "Количество окрашенных квартир: -";
    public string AllColoredApartmentsText
    {
        get { return _allColoredApartmentsText; }
        set
        {
            if ( _allColoredApartmentsText != value)
            {
                _allColoredApartmentsText = value;
                OnPropertyChanged(nameof(AllColoredApartmentsText));
            }
        }
    }

    private string _oneRoomColoredApartmentsText = "Количество окрашенных однокомнатных квартир: -";
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

    private string _twoRoomsColoredApartmentsText = "Количество окрашенных двухкомнатных квартир: -";
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

    private string _threeRoomsColoredApartmentsText = "Количество окрашенных трехкомнатных квартир: -";
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

    private string _fourRoomsColoredApartmentsText = "Количество окрашенных четырехкомнатных квартир: -";
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

    private string _studioColoredApartmentText = "Количество окрашенных квартир-студий: -";
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


    private UIApplication _uiApp;
    private UIDocument _uiDoc;
    private Document _doc;

    public ICommand AnalyzeCommand { get; }
    public ICommand ColorCommand { get; }

    public ExternalEvent ExternalColorEvent { get; private set; }
    public ExternalEvent ExternalAnalyzeEvent { get; private set; }


    private ApplicationViewModel() { }

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

    
}