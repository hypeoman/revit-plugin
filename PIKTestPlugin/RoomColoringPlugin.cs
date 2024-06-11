using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;

namespace RoomColoringPlugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ColorizeRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                IEnumerable<Room> allRooms = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Rooms).OfClass(typeof(SpatialElement)).OfType<Room>()
                    .Where(e => e.LookupParameter("ROM_Зона")?
                    .AsString().Contains("Квартира") == true);

                var allApartments = allRooms.GroupBy(room => new // Все квартиры (сгруппированные комнаты)
                {
                    Zone = room.LookupParameter("ROM_Зона").AsString(),
                    Level = room.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId(),
                    Block = room.LookupParameter("BS_Блок").AsString(),
                    Subzone = room.LookupParameter("ROM_Подзона").AsString()
                }).OrderBy(group => group.Key.Zone) // Сортировка по номеру квартиры
                .ToList();

                var groupedApartments = allApartments.GroupBy(apartments => new // Группы квартир, по Уровню, Секции и Подзоне
                {
                    Level = apartments.First().get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId(),
                    Block = apartments.First().LookupParameter("BS_Блок").AsString(),
                    Subzone = apartments.First().LookupParameter("ROM_Подзона").AsString()
                }); 

                using (Transaction trans = new Transaction(doc, "Окраска квартир"))
                {
                    trans.Start();

                    foreach (var group in groupedApartments)
                    {
                        bool lastApartmentsIsColored = false; 

                        var groupList = group.ToList();

                        int lastNumber = GetNumericPartFromParameter(groupList[0].First().LookupParameter("ROM_Зона").AsString());

                        int i = 1;

                        while (i < group.Count())
                        {
                            // Проверяем, что квартиры смежные
                            if (Math.Abs(lastNumber - GetNumericPartFromParameter(groupList[i].First().LookupParameter("ROM_Зона").AsString())) == 1)
                            {
                                if (!lastApartmentsIsColored)
                                {
                                    lastApartmentsIsColored = true;
                                    ColorApartment(groupList[i], trans, doc);
                                }
                                else
                                {
                                    lastApartmentsIsColored = false;
                                }
                            }
                            else
                            {
                                lastApartmentsIsColored = false;
                            }
                            lastNumber = GetNumericPartFromParameter(groupList[i].First().LookupParameter("ROM_Зона").AsString());
                            i++;
                        }
;
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

        private int GetNumericPartFromParameter(string paramValue)
        {
            string[] parts = paramValue.Split(' ');
            if (parts.Length > 0 && int.TryParse(parts[parts.Length - 1], out int numericPart))
            {
                return numericPart;
            }
            return 0;
        }

        private void ColorApartment(IGrouping<object, Room> apartment, Transaction trans, Document doc)
        {
            foreach (var room in apartment)
            {
                ColorRoom(room, trans, doc);
            }
        }

        private void ColorRoom(Room room, Transaction trans, Document doc)
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
