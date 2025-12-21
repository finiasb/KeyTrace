using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private void ParseFile(string filePath, KeyStatistics stats)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                string[] parts = line.Split(' ');
                foreach (string s in parts)
                {
                    if (int.TryParse(s, out int charInInt))
                    {
                        stats.TotalKeys++;
                        if (charInInt >= 0 && charInInt < 256)
                            stats.Counts[charInInt]++;
                    }
                }
            }
        }

    }
}
