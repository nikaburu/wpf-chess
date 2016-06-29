using System.Windows.Media;

namespace WpfChess.WpfPresentation.Helpers
{
    public static class GameSettingHelper
    {
        public static Color WhiteColor
        {
            get { return Colors.White; }
        }

        public static Color BlackColor
        {
            get { return Colors.Black; }
        }

        public static Color WhiteCellColor
        {
            get { return Color.FromRgb(0xFD, 0xD7, 0xA9); }
        }

        public static Color BlackCellColor
        {
            get { return Color.FromRgb(0xDC, 0x8B, 0x27); }
        }

        public static Color MarkedCellColor
        {
            get { return Colors.SkyBlue; }
        }

        public static Color SelectedCellColor
        {
            get { return Colors.RosyBrown; }
        }
    }
}
