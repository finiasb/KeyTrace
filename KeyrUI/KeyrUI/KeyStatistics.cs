using System;

namespace WpfApp1
{
    // A Data Transfer Object (DTO) that stores the processed keyboard metrics.
    // It maintains the raw counts and calculates the relative frequency of each key.
    public class KeyStatistics
    {
        public int TotalKeys { get; set; } // Global count across all processed logs
        public int TodayKeys { get; set; } // Count for the current day

        /// <summary>
        /// An array where the index represents the Virtual Key Code (0-255).
        /// Example: Counts[65] stores how many times 'A' was pressed.
        /// </summary>
        public int[] Counts { get; } = new int[256]; 
        public double[] Percentages { get; } = new double[256]; // Stores the distribution percentage for each key(0.0 to 100.0).
        public int MinutesCount { get; set; }
        public double KPM { get; set; } // Keys Per Minute
        public int WPM { get; set; }    // Words Per Minute

        // Resets all statistics to zero.
        public void Clear()
        {
            TotalKeys = 0;
            MinutesCount = 0;
            TodayKeys = 0;
            Array.Clear(Counts, 0, Counts.Length);
            Array.Clear(Percentages, 0, Percentages.Length);
        }

        // Manually increments the count for a specific key.
        public void AddKey(int key)
        {
            if (key < 0 || key > 255)
                return;
            TotalKeys++;
            Counts[key]++;
        }

        // Converts raw counts into percentages relative to TotalKeys.
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