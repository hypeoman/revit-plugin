using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKTestPlugin
{
    public class ExternalAnalyzeHandler : IExternalEventHandler
    {
        private ApplicationViewModel _viewModel;

        public ExternalAnalyzeHandler(ApplicationViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Execute(UIApplication uiApp)
        {

            Document doc = uiApp.ActiveUIDocument.Document;

            var allApartments = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Rooms).OfClass(typeof(SpatialElement)).OfType<Room>()
                    .Where(e => e.LookupParameter("ROM_Зона")?
                    .AsString().Contains("Квартира") == true).GroupBy(room => new // Все квартиры (сгруппированные комнаты)
                    {
                        Zone = room.LookupParameter("ROM_Зона").AsString(),
                        Level = room.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId(),
                        Block = room.LookupParameter("BS_Блок").AsString(),
                        Subzone = room.LookupParameter("ROM_Подзона").AsString()
                    }).OrderBy(group => group.Key.Zone)
                    .ToList();

            _viewModel.AllApartmentsText = $"Общее количество квартир: {allApartments.Count().ToString()}";

            var oneRoomApartments = allApartments.Where(group => group.Key.Subzone == "Однокомнатная квартира");

            _viewModel.OneRoomApartmentsText = $"Количество однокомнатных квартир: {oneRoomApartments.Count().ToString()}";

            var twoRoomsApartments = allApartments.Where(group => group.Key.Subzone == "Двухкомнатная квартира");
            
            _viewModel.TwoRoomsApartmentsText = $"Количество двухкомнатных квартир: {twoRoomsApartments.Count().ToString()}";

            var threeRoomsApartments = allApartments.Where(group => group.Key.Subzone == "Трехкомнатная квартира");

            _viewModel.ThreeRoomsApartmentsText = $"Количество трехкомнатных квартир: {threeRoomsApartments.Count().ToString()}";

            var fourRoomsApartments = allApartments.Where(group => group.Key.Subzone == "Четырехкомнатная квартира");

            _viewModel.FourRoomsApartmentsText = $"Количество четырехкомнатных квартир: {fourRoomsApartments.Count().ToString()}";

            var studioApartments = allApartments.Where(group => group.Key.Subzone == "Квартира студия");

            _viewModel.StudioApartmentsText = $"Количество квартир-студий: {studioApartments.Count().ToString()}";

            var allColoredApartments = allApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.AllColoredApartmentsText = $"Общее количество окрашенных квартир: {allColoredApartments.Count().ToString()}";

            var oneRoomColoredApartments = oneRoomApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.OneRoomColoredApartmentsText = $"Количество окрашенных однокомнатных квартир: {oneRoomColoredApartments.Count().ToString()}";

            var twoRoomsColoredApartments = twoRoomsApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.TwoRoomsColoredApartmentsText = $"Количество окрашенных двухкомнатных квартир: {twoRoomsColoredApartments.Count().ToString()}";

            var threeRoomsColoredApartments = threeRoomsApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.ThreeRoomsColoredApartmentsText = $"Количество окрашенных трехкомнатных квартир:  {threeRoomsColoredApartments.Count().ToString()}";

            var fourRoomsColoredApartments = fourRoomsApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.FourRoomsColoredApartmentsText = $"Количество окрашенных четырехкомнатных квартир:  {fourRoomsColoredApartments.Count().ToString()}";

            var studioColoredApartments = studioApartments.Where(group => group.Any(room =>
                    !string.IsNullOrEmpty(room.LookupParameter("ROM_Подзона_Index")?.AsString())))
                    .ToList();

            _viewModel.StudioColoredApartmentsText = $"Количество окрашенных квартир-студий:  {studioColoredApartments.Count().ToString()}";
        }

        public string GetName()
        {
            return "ExternalColorHandler";
        }
    }

}
