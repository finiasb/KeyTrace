using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1
{
    // Handles reading and parsing log files to generate statistics 
    // regarding keystrokes and typing activity.
    public class KeyLogReader
    {
        private readonly string _path; // The directory where log files are stored

        public KeyLogReader(string path)
        {
            _path = path;
        }

        // Reads logs for a specific range of days ending today.
        public KeyStatistics ReadDays(int days)
        {
            KeyStatistics stats = new KeyStatistics();
            stats.Clear();

            // If days is 1, it specifically looks at yesterday's file
            if (days == 1)
            {
                string filePath = Path.Combine(_path, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + ".txt");
                if (!File.Exists(filePath))
                    return stats;
                ParseFile(filePath, stats);
            }
            else
            {
                // Loop backwards from 'days' ago up to today (0)
                for (int i = days; i >= 0; i--)
                {
                    string filePath = Path.Combine(_path, DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd") + ".txt");
                    if (!File.Exists(filePath))
                        continue;
                    ParseFile(filePath, stats);
                }
            }
            return stats;
        }

        //Reads "total.txt" file containing lifetime stats.
        public KeyStatistics ReadAll()
        {
            var stats = new KeyStatistics();
            string totalPath = Path.Combine(_path, "total.txt");
            if (File.Exists(totalPath))
            {
                ParseFile(totalPath, stats);
            }
            return stats;
        }

        // Core logic: Processes a single text file line by line.
        // Expected format: "KeyID:HH:mm:ss" separated by spaces.
        public void ParseFile(string filePath, KeyStatistics stats)
        {
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadLines(filePath))
            {
                string[] parts = line.Split(' ');
                foreach (string s in parts)
                {
                    string[] splitedStringS = s.Split(':');
                    if (int.TryParse(splitedStringS[0], out int charInInt))
                    {
                        stats.TotalKeys++;
                        if (charInInt >= 0 && charInInt < 256)
                            stats.Counts[charInInt]++;
                    }
                }
            }
        }

        // Calculates Keys Per Minute (KPM) activity for the current day.
        public void KPMReader(KeyStatistics stats)
        {
            string filePath = Path.Combine(_path, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            if (!File.Exists(filePath))
            {
                stats.KPM = 0;
                stats.MinutesCount = 0;
                return;
            }

            int todayKeys = 0;
            // Using a HashSet to store unique "HH:mm" strings to find total active minutes
            HashSet<string> minuteActive = new HashSet<string>();

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] entries = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    string[] parts = entry.Split(':');
                    if (parts.Length >= 3)
                    {
                        todayKeys++;
                        // parts[1] is Hour, parts[2] is Minute. 
                        // Adding "14:05" to the HashSet ensures we count that minute only once.
                        string minuteKey = $"{parts[1]}:{parts[2]}";
                        minuteActive.Add(minuteKey);
                    }
                }
            }

            stats.TodayKeys = todayKeys;
            // Total minutes where at least one key was pressed.
            stats.MinutesCount = minuteActive.Count;
        }
    }
}