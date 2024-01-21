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
                    string key = GetParamValueByName("Уровень", apartment) + "|" +
                                 GetParamValueByName("BS_Блок", apartment) + "|" +
                                 GetParamValueByName("ROM_Подзона", apartment) + "|" +
                                 GetParamValueByName("ROM_Зона", apartment);

                    if (!groupedApartments.ContainsKey(key))
                    {
                        groupedApartments[key] = new List<Element>();
                    }

                    groupedApartments[key].Add(apartment);
                }

                // Перебираем сгруппированные квартиры
                foreach (var apartmentGroup in groupedApartments.Values)
                {
                    // Проверяем, есть ли соседние номера квартир
                    if (CheckAdjacentApartmentNumbers(apartmentGroup))
                    {
                        // Получаем значение из ROM_Расчетная_подзона_ID
                        string calculatedZoneId = GetParamValueByName("ROM_Расчетная_подзона_ID", apartmentGroup[0]);

                        // Обновляем ROM_Подзона_Index для всех квартир в группе
                        foreach (Element apartment in apartmentGroup)
                        {
                            string newParamValue = calculatedZoneId + ".Полутон";
                            SetParamValueByName("ROM_Подзона_Index", apartment, newParamValue);
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

        // Функция для проверки смежности номеров квартир
        private bool CheckAdjacentApartmentNumbers(List<Element> apartmentGroup)
        {
            List<int> numericValues = new List<int>();

            for (int i = 0; i < apartmentGroup.Count - 1; i++)
            {
                string zoneValue1 = GetParamValueByName("ROM_Зона", apartmentGroup[i]);
                string zoneValue2 = GetParamValueByName("ROM_Зона", apartmentGroup[i + 1]);

                int numericPart1, numericPart2;

                // Извлечение числовой части, предполагая, что она всегда в конце строки
                if (int.TryParse(zoneValue1.Split(' ').Last(), out numericPart1) &&
                    int.TryParse(zoneValue2.Split(' ').Last(), out numericPart2))
                {
                    // Проверка, что значения для двух квартир смежны
                    if (numericPart2 != numericPart1 + 1)
                    {
                        return false;
                    }
                }
            }

            return true;
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