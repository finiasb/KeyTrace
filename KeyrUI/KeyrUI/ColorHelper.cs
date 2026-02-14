using System.Windows.Media;

namespace WpfApp1
{
    public static class ColorHelper
    {
        public static Color GetColorForPercentage(double pct)
        {
            if (pct <= 0) return Color.FromRgb(0x1a, 0x1f, 0x29);
            if (pct < 0.5) return Color.FromRgb(0x3b, 0x47, 0x5c);
            if (pct < 1.5) return Color.FromRgb(0x4a, 0x9e, 0xff);
            if (pct < 2.5) return Color.FromRgb(0x00, 0xd4, 0xaa);
            if (pct < 4.0) return Color.FromRgb(0xff, 0xd9, 0x3d);
            if (pct < 6.0) return Color.FromRgb(0xff, 0x8c, 0x32);
            return Color.FromRgb(0xff, 0x4d, 0x6d);
        }
    }
}