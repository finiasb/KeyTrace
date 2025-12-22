using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Keyr
{
    public partial class Form1 : Form
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyrLogs");
        private ToolTip toolTip = new ToolTip();
        KeyStatistics stats;
        KeyLogReader reader;
        public Form1()
        {
            InitializeComponent();
            reader = new KeyLogReader(path);
            stats = reader.ReadDays(0);

            label1.Text = "Today:\n  " + stats.TotalKeys;
            UpdateButtonColors();   
            stats = reader.ReadDays(1);
            label2.Text = "Yesterday:\n     " + stats.TotalKeys;
            stats = reader.ReadAll();
            label3.Text = "All Time: \n   " + stats.TotalKeys;

            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 300;
            toolTip.ReshowDelay = 200;
            toolTip.ShowAlways = true;
        }
        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.Location = new Point(btn.Location.X + 1, btn.Location.Y + 1);
            }
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.Location = new Point(btn.Location.X - 1, btn.Location.Y - 1);
            }
        }

        private void UpdateButtonColors()
        {
            stats.CalculatePercentages();
            for (int i = 1; i < 256; i++)
            {
                string name = $"button{i}";
                Button button = this.Controls.Find(name, true).FirstOrDefault() as Button;
                if (button == null)
                    continue;

                if (stats.Counts[i] == 0)
                {
                    button.BackColor = Color.Gray;
                    toolTip.SetToolTip(button, $"Apăsări: 0\nProcent: 0.00%");
                    continue;
                }

                button.BackColor = ColorHelper.GetColorForPercentage(stats.Percentages[i]);
                toolTip.SetToolTip(button, $"Apăsări: {stats.Counts[i]}\nProcent: {stats.Percentages[i]:0.00}%");
            }
        }

        private void buttonToday_Click(object sender, EventArgs e)
        {
            stats = reader.ReadDays(0);
            UpdateButtonColors();   
        }

        private void buttonYesterday_Click(object sender, EventArgs e)
        {
            stats = reader.ReadDays(1);
            UpdateButtonColors();
        }

        private void buttonWeek_Click(object sender, EventArgs e)
        {
            stats = reader.ReadDays(7);
            UpdateButtonColors();
        }

        private void buttonMonth_Click(object sender, EventArgs e)
        {
            stats = reader.ReadDays(30);
            UpdateButtonColors();
        }

        private void buttonYear_Click(object sender, EventArgs e)
        {
            stats = reader.ReadDays(365);
            UpdateButtonColors();
        }

        private void buttonAllTime_Click(object sender, EventArgs e)
        {
            stats = reader.ReadAll();
            UpdateButtonColors();
        }
    }
}
