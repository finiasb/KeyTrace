using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Keyr
{
    public class KeyCalculator
    {
        public double CalculateKPM(int totalKeys, int activeMinutes)
        {
            if (activeMinutes <= 0) return 0;
            return (double)totalKeys / activeMinutes;
        }
        public int CalculateWPM(int KPM)
        {
            return KPM / 5;
        }
    }
}
