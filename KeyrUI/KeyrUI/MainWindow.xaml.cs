using KeyrUI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly string _logPath;
        private readonly KeyLogReader _reader;
        private readonly KeyCalculator _calculator;
        private KeyStatistics _currentStats;
        private ToolTip _hoverToolTip;

        public MainWindow()
        {
            InitializeComponent();

            _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyrLogs");
            _reader = new KeyLogReader(_logPath);
            _calculator = new KeyCalculator();

            _hoverToolTip = new ToolTip
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                Background = new SolidColorBrush(Color.FromRgb(0x1a, 0x1f, 0x29)),
                Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0x88)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0x88)),
                BorderThickness = new Thickness(1)
            };

            this.Loaded += MainWindow_Loaded;
        }
        private void KeyboardTab_Click(object sender, RoutedEventArgs e)
        {
            // Arată secțiunea Keyboard
            KeyboardSection.Visibility = Visibility.Visible;
            MouseSection.Visibility = Visibility.Collapsed;

            // Actualizează stilurile butoanelor
            KeyboardTabButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
            KeyboardTabButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff88"));

            MouseTabButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6b7280"));
            MouseTabButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a3342"));
        }

        private void MouseTab_Click(object sender, RoutedEventArgs e)
        {
            // Arată secțiunea Mouse
            KeyboardSection.Visibility = Visibility.Collapsed;
            MouseSection.Visibility = Visibility.Visible;

            // Actualizează stilurile butoanelor
            MouseTabButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
            MouseTabButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff88"));

            KeyboardTabButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6b7280"));
            KeyboardTabButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a3342"));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInitialData();
            SetupKeyboardHoverEvents();

            ResetTab(TabToday);
            ActivateTab(TabToday);

            UpdateKeyboardColors();
        }
        private void LoadInitialData()
        {
            var todayStats = _reader.ReadDays(0);
            _reader.KPMReader(todayStats);
            todayStats.KPM = _calculator.CalculateKPM(todayStats.TodayKeys, todayStats.MinutesCount);
            todayStats.WPM = _calculator.CalculateWPM((int)todayStats.KPM);
            TodayKeys.Text = todayStats.TotalKeys.ToString("N0");
            KPM.Text = ((int)todayStats.KPM).ToString();
            WPM.Text = $"{todayStats.WPM} wpm";

            var yesterdayStats = _reader.ReadDays(1);
            YesterdayKeys.Text = yesterdayStats.TotalKeys.ToString("N0");

            var allTimeStats = _reader.ReadAll();
            AllTimeKeys.Text = allTimeStats.TotalKeys.ToString("N0");

            _currentStats = todayStats;
            UpdateKeyboardColors();
        }

        private void SetupKeyboardHoverEvents()
        {
            var allBorders = FindVisualChildren<Border>(this).Where(b => b.Cursor == Cursors.Hand);

            foreach (var border in allBorders)
            {
                border.MouseEnter += Key_MouseEnter;
                border.MouseLeave += Key_MouseLeave;
            }
        }

        private void Key_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border == null) return;

            var textBlock = FindVisualChild<TextBlock>(border);
            if (textBlock == null) return;

            int keyCode = KeyCodeHelper.GetKeyCode(textBlock.Text);

            if (keyCode >= 0 && keyCode < 256 && _currentStats != null)
            {
                int count = _currentStats.Counts[keyCode];
                double percentage = _currentStats.Percentages[keyCode];

                _hoverToolTip.Content = $"Keystrokes: {count:N0}\nProcent: {percentage:F2}%";
                _hoverToolTip.IsOpen = true;
            }
        }

        private void Key_MouseLeave(object sender, MouseEventArgs e)
        {
            _hoverToolTip.IsOpen = false;
        }



       


        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null)
                return;

            ResetTab(TabToday);
            ResetTab(TabYesterday);
            ResetTab(TabWeek);
            ResetTab(TabMonth);
            ResetTab(TabYear);
            ResetTab(TabAll);

            ActivateTab(clickedButton);

            int days = int.Parse(clickedButton.Tag.ToString());

            if (days == -1)
            {
                _currentStats = _reader.ReadAll();
            }
            else
            {
                _currentStats = _reader.ReadDays(days);
            }

            UpdateKeyboardColors();
        }
        private void UpdateKeyboardColors()
        {
            if (_currentStats == null) return;

            _currentStats.CalculatePercentages();
            var keyBorders = FindVisualChildren<Border>(this).Where(b => b.Cursor == Cursors.Hand);

            var defaultBg = (SolidColorBrush)new BrushConverter().ConvertFrom("#12161d");

            foreach (var border in keyBorders)
            {
                var textBlock = FindVisualChild<TextBlock>(border);
                if (textBlock == null) continue;

                int keyCode = KeyCodeHelper.GetKeyCode(textBlock.Text);


                if (keyCode >= 0 && keyCode < 256)
                {
                    double percentage = _currentStats.Percentages[keyCode];

                    if (percentage <= 0)
                    {
                        border.Background = defaultBg;
                    }
                    else
                    {
                        Color color = ColorHelper.GetColorForPercentage(percentage);
                        border.Background = new SolidColorBrush(color);
                    }
                }
            }
        }
        private void ResetTab(Button tab)
        {
            tab.BorderBrush = new SolidColorBrush(Color.FromRgb(0x2a, 0x33, 0x42));
            tab.BorderThickness = new Thickness(0, 0, 0, 2);
            tab.Foreground = new SolidColorBrush(Color.FromRgb(0x6b, 0x72, 0x80));
        }

        private void ActivateTab(Button tab)
        {
            tab.BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0x88));
            tab.BorderThickness = new Thickness(0, 0, 0, 3);
            tab.Foreground = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T t)
                        yield return t;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}