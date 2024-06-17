using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PIKTestPlugin;
using System.Linq.Expressions;
using System.Windows;

namespace RoomColoringPlugin
{
    public partial class MainWindow : Window
    {
        public ApplicationViewModel _viewModel;
        private ExternalAnalyzeHandler _externalEventHandler;
        private ExternalEvent _externalEvent;

        public MainWindow(UIApplication uiApp, UIDocument uiDoc, Document doc)
        {
            InitializeComponent();
            _viewModel = new ApplicationViewModel(uiApp, uiDoc, doc);
            DataContext = _viewModel;

            _externalEventHandler = new ExternalAnalyzeHandler(_viewModel);
            _externalEvent = ExternalEvent.Create(_externalEventHandler);
        }
    }
}
