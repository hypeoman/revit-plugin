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
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;

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
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                var allRooms = new FilteredElementCollector(doc) // Все квартиры
                    .OfCategory(BuiltInCategory.OST_Rooms).OfClass(typeof(SpatialElement)).OfType<Room>()
                    .Where(e => e.LookupParameter("ROM_Зона")?
                    .AsString().Contains("Квартира") == true);

                var groupedElements = allRooms // Квартиры, сгруппированные по этажу, секции и количеству комнат
                    .GroupBy(e => new
                    {
                        Level = e.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId(),
                        Block = e.LookupParameter("BS_Блок")?.AsString() ?? string.Empty,
                        Zone = e.LookupParameter("ROM_Подзона")?.AsString() ?? string.Empty
                    });

                using (Transaction trans = new Transaction(doc, "Окраска квартир"))
                {
                    trans.Start();
                    foreach (var group in groupedElements)
                    {
                        List<Room> sortedElementsInGroup = group
                            .OrderBy(e => GetNumericPartFromParameter(e.LookupParameter("ROM_Зона")?.AsString()))
                            .ToList();

                        GetParametersValue(sortedElementsInGroup);

                        List<Room> forColoring = new List<Room>();

                        int lastNumber = GetNumericPartFromParameter(sortedElementsInGroup[0].LookupParameter("ROM_Зона").AsString());
                        forColoring.Add(sortedElementsInGroup[0]);

                        bool lastApartmentIsColored = false;

                        int i = 1;

                        while (i < sortedElementsInGroup.Count) 
                        { 
                            if (lastNumber == GetNumericPartFromParameter(sortedElementsInGroup[i].LookupParameter("ROM_Зона").AsString()))
                            {
                                forColoring.Add(sortedElementsInGroup[i]);
                            }
                            else
                            {

                                if (Math.Abs(lastNumber - GetNumericPartFromParameter(sortedElementsInGroup[i].LookupParameter("ROM_Зона").AsString())) == 1)
                                {
                                    if (lastApartmentIsColored == false)
                                    {
                                        ColorApartments(forColoring, trans, doc);
                                        lastApartmentIsColored = true;
                                    }
                                }
                                else
                                {
                                    lastApartmentIsColored = false;
                                }
                                lastNumber = GetNumericPartFromParameter(sortedElementsInGroup[i].LookupParameter("ROM_Зона").AsString());
                                forColoring = new List<Room>();
                            }

                            i++;
                        }


                    }
                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public void GetParametersValue(List<Room> elements)
        {
            var filePath = "D:\\projects\\c#\\revit\\test-plugin\\Rooms.txt";

            StreamWriter sw = new StreamWriter(filePath, true);

            sw.WriteLine("1 ГРУППА");

            foreach (var element in elements)
            {
                sw.WriteLine($"ROM_Зона : {element.LookupParameter("ROM_Зона").AsString()}; ROM_Подзона : {element.LookupParameter("ROM_Подзона").AsString()}; BS_Блок : {element.LookupParameter("BS_Блок").AsString()}; Уровень : {element.LookupParameter("Уровень").AsString()} ");
            }

            sw.Close();
        }

        private int GetNumericPartFromParameter(string paramValue)
        {
            string[] parts = paramValue.Split(' ');
            if (parts.Length > 0 && int.TryParse(parts[parts.Length - 1], out int numericPart))
            {
                return numericPart;
            }
            return 0;
        }

        private void ColorApartments(List<Room> rooms, Transaction trans, Document doc)
        {
            foreach (var room in rooms)
            {
                ColorApartment(room, trans, doc);
            }
        }

        private void ColorApartment(Room room, Transaction trans, Document doc)
        {
            string zoneId = room.LookupParameter("ROM_Расчетная_подзона_ID")?.AsString();
            Parameter param = room.LookupParameter("ROM_Подзона_Index");
            if (!string.IsNullOrEmpty(param?.AsString())) return; // Пропускаем уже окрашенные комнаты

            using (SubTransaction subTrans = new SubTransaction(doc))
            {
                subTrans.Start();
                param.Set($"{zoneId}.Полутон");
                subTrans.Commit();
            }
        }
    }
}
