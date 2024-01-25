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

                IList<Element> apartments = new List<Element>(); // список для помещений где ROM_Зона содержит Квартира

                foreach (Element element in rooms)
                {
                    if (GetParamValueByName("ROM_Зона", element).Contains("Квартира"))
                    {
                        apartments.Add(element);
                    }
                }

                // Словарь для хранения квартир, сгруппированных по ключу (комбинация Level, BS_Блок, ROM_Подзона и ROM_Зона)
                Dictionary<string, List<Element>> groupedApartments = new Dictionary<string, List<Element>>();

                foreach (Element apartment in apartments)
                {
                    string key = GetParamValueByName("Level", apartment) + "|" +
                                 GetParamValueByName("BS_Блок", apartment) + "|" +
                                 GetParamValueByName("ROM_Подзона", apartment) + "|" +
                                 GetParamValueByName("ROM_Зона", apartment);

                    if (!groupedApartments.ContainsKey(key))
                    {
                        groupedApartments[key] = new List<Element>();
                    }

                    groupedApartments[key].Add(apartment);
                }


                foreach (var keyValuePair in groupedApartments)
                {
                    List<Element> apartmentList = keyValuePair.Value;

                    // Перебираем пары элементов
                    for (int i = 0; i < apartmentList.Count - 1; i++)
                    {
                        for (int j = i + 1; j < apartmentList.Count; j++)
                        {
                            Element apartment1 = apartmentList[i];
                            Element apartment2 = apartmentList[j];

                            // Получаем значения параметров
                            string romZone1 = GetParamValueByName("ROM_Зона", apartment1);
                            string romZone2 = GetParamValueByName("ROM_Зона", apartment2);

                            int numericPart1;
                            int numericPart2;

                            int.TryParse(romZone1.Split(' ').Last(), out numericPart1);
                            int.TryParse(romZone2.Split(' ').Last(), out numericPart2);

                            string romPodzonaIndex1 = GetParamValueByName("ROM_Подзона_Index", apartment1);
                            string romPodzonaIndex2 = GetParamValueByName("ROM_Подзона_Index", apartment2);

                            // Проверяем условия
                            if (Math.Abs(numericPart1 - numericPart2) == 1 &&
                                string.IsNullOrEmpty(romPodzonaIndex1) && string.IsNullOrEmpty(romPodzonaIndex2))
                            {
                                SetParamValueByName("ROM_Подзона_Index", apartment1, GetParamValueByName("ROM_Расчетная_подзона_ID", apartment1)+".Полутон");
                            }
                        }
                    }
                }



                // Если все хорошо
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // Обработка исключений
                message = ex.Message;
                return Result.Failed;
            }
        }

        // Функция для установки значения параметра по имени
        private void SetParamValueByName(string paramName, Element element, string paramValue)
        {
            Parameter parameter = element.LookupParameter(paramName);
            if (parameter != null && !parameter.IsReadOnly)
            {
                using (Transaction transaction = new Transaction(element.Document, "Set Parameter Value"))
                {
                    transaction.Start();
                    parameter.Set(paramValue);
                    transaction.Commit();
                }
            }
        }
    }
}