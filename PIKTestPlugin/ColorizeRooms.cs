namespace RoomColoringPlugin
{
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    /// <summary>
    /// Класс, реализующий команду внешнего приложения для Revit, предназначенную для окрашивания комнат.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ColorizeRooms : IExternalCommand
    {
        /// <summary>
        /// Метод, выполняющий команду окрашивания комнат.
        /// </summary>
        /// <param name="commandData">Данные команды, предоставленные Revit.</param>
        /// <param name="message">Сообщение об ошибке, если выполнение команды не удалось.</param>
        /// <param name="elements">Набор элементов, участвующих в команде.</param>
        /// <returns>Результат выполнения команды.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Получаем доступ к активному документу Revit.
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                // Создаем и отображаем главное окно плагина.
                MainWindow form = new MainWindow(uiApp, uiDoc, doc);

                form.Show();

                // Возвращаем успешный результат выполнения команды.
                return Result.Succeeded;
            }
            catch
            {
                // В случае ошибки возвращаем результат выполнения с ошибкой.
                return Result.Failed;
            }
        }
    }
}
