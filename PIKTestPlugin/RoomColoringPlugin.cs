using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;

namespace RoomColoringPlugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ColorizeRooms : IExternalCommand
    {
        // Функция для получения значения параметра элемнта по названию
        public string GetParamValueByName(string name, Element e)
        {
            var paramValue = string.Empty;

            foreach (Parameter parameter in e.Parameters)
            {
                if (parameter.Definition.Name == name)
                {
                    paramValue = parameter.AsString();
                }
            }
            return paramValue;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Получаем объект приложения и документа
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                RoomFilter filter = new RoomFilter();

                FilteredElementCollector collector = new FilteredElementCollector(doc);
                IList<Element> rooms = collector.WherePasses(filter).ToElements(); // получаем список со всеми помещениями

                // IList<Element> apartments = new List<Element>(); // список для помещений где ROM_Зона содержит Квартира

                var parameters = from Element in rooms group Element by Element.Parameters;



                // Если все хорошо
                return Result.Succeeded;
            }
            catch
            {
                // Если все плохо
                return Result.Failed;
            }
        }
    }
}