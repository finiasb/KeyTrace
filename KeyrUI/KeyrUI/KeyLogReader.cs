using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1
{
    public class KeyLogReader
    {
        private readonly string _path;

        public KeyLogReader(string path)
        {
            _path = path;
        }

        public KeyStatistics ReadDays(int days)
        {
            KeyStatistics stats = new KeyStatistics();
            stats.Clear();

            if (days == 1)
            {
                string filePath = Path.Combine(_path, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + ".txt");
                if (!File.Exists(filePath))
                    return stats;
                ParseFile(filePath, stats);
            }
            else
            {
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
                        string minuteKey = $"{parts[1]}:{parts[2]}";
                        minuteActive.Add(minuteKey);
                    }
                }
            }

            stats.TodayKeys = todayKeys;
            stats.MinutesCount = minuteActive.Count;
        }
    }
}