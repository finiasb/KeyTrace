using System;

namespace WpfApp1
{
    public class KeyStatistics
    {
        public int TotalKeys { get; set; }
        public int TodayKeys { get; set; }
        public int[] Counts { get; } = new int[256];
        public double[] Percentages { get; } = new double[256];
        public int MinutesCount { get; set; }
        public double KPM { get; set; }
        public int WPM { get; set; }

        public void Clear()
        {
            TotalKeys = 0;
            MinutesCount = 0;
            TodayKeys = 0;
            Array.Clear(Counts, 0, Counts.Length);
            Array.Clear(Percentages, 0, Percentages.Length);
        }

        public void AddKey(int key)
        {
            if (key < 0 || key > 255)
                return;
            TotalKeys++;
            Counts[key]++;
        }

        public void CalculatePercentages()
        {
            for (int i = 0; i < 256; i++)
            {
                if (Counts[i] != 0)
                {
                    Percentages[i] = ((double)Counts[i] / TotalKeys) * 100;
                }
                else
                {
                    Percentages[i] = 0;
                }
            }
        }
    }
}