using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyr
{
    public static class ColorHelper
    {
        public static Color GetColorForPercentage(double pct)
        {
            if (pct < 0.01) return Color.Gray;
            if (pct < 0.1) return Color.FromArgb(219, 238, 255);
            if (pct < 0.3) return Color.FromArgb(179, 229, 255);
            if (pct < 0.5) return Color.FromArgb(144, 238, 255);
            if (pct < 0.8) return Color.FromArgb(122, 217, 217);
            if (pct < 1) return Color.FromArgb(144, 238, 144);
            if (pct < 1.5) return Color.FromArgb(122, 217, 119);
            if (pct < 2) return Color.FromArgb(99, 196, 95);
            if (pct < 2.7) return Color.FromArgb(77, 175, 70);
            if (pct < 3.5) return Color.FromArgb(52, 155, 45);
            if (pct < 4.2) return Color.FromArgb(127, 155, 45);
            if (pct < 5) return Color.FromArgb(181, 176, 0);
            if (pct < 6) return Color.FromArgb(127, 120, 0);
            if (pct < 7) return Color.FromArgb(168, 108, 0);
            if (pct < 10) return Color.FromArgb(212, 83, 0);
            if (pct < 20) return Color.FromArgb(255, 0, 0);
            return Color.FromArgb(139, 0, 0);
        }
    }

}
