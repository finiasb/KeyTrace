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
    public class KeyLogReader
    {
        string _path;
        public KeyLogReader(string path) 
        {
            _path = path;
        }

        public KeyStatistics ReadDays(int days)
        {
            KeyStatistics stats = new KeyStatistics();
            stats.Clear();
            if(days == 1)
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
            ParseFile(Path.Combine(_path, "total.txt"), stats);
            return stats;
        }

        public void ParseFile(string filePath, KeyStatistics stats)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                string[] parts = line.Split(' ');
                foreach (string s in parts)
                {
                    string[] SplitedStringS = s.Split(':');
                    if (int.TryParse(SplitedStringS[0], out int charInInt))
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
            int TodayKeys = 0;
            HashSet<string> minuteActive = new HashSet<string>();
            foreach (var linie in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(linie))
                    continue;

                string[] intrari = linie.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string intrare in intrari)
                {
                    string[] parti = intrare.Split(':');

                    if (parti.Length >= 3)
                    {
                        TodayKeys++;
                        string cheieMinut = $"{parti[1]}:{parti[2]}";
                        minuteActive.Add(cheieMinut);
                    }
                }
            }
            stats.TodayKeys = TodayKeys;
            stats.MinutesCount = minuteActive.Count;
        }
    }
}
