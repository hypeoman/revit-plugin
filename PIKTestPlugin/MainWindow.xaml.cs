namespace RoomColoringPlugin
{
    using System.Windows;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using PIKTestPlugin;

    public partial class MainWindow : Window
    {
        private ApplicationViewModel _viewModel;
        private ExternalAnalyzeHandler _externalEventHandler;
        private ExternalEvent _externalEvent;

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        /// <param name="uiApp">UIApplication получаемое от Revit</param>
        /// <param name="uiDoc">UIDocument получаемый от Revit</param>
        /// <param name="doc">Document получаемый от Revit</param>
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
