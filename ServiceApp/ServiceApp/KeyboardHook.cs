using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace ServiceApp
{
    public static class KeyboardHook
    {
        // Constants for Windows Hook API
        private const int WH_KEYBOARD_LL = 13; // Low-level keyboard hook ID
        private const int WM_KEYDOWN = 0x0100; // Event ID for a key being pressed

        // buffer stores keystrokes in memory before saving to disk to improve performance
        private static StringBuilder buffer = new StringBuilder();
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static System.Timers.Timer timer;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static void StartHook()
        {
            _hookId = SetHook(_proc);

            // Initialize a timer to flush the buffer to a file every 3 seconds
            // This prevents constant disk writing (which is slow and wears out SSDs)
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        public static void StopHook()
        {
            // IMPORTANT: Always unhook when closing to prevent system lag or instability
            UnhookWindowsHookEx(_hookId);

            timer?.Stop();
            timer?.Dispose();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            // Link the hook to the current process module
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }

        private static int ok = 0; // Counter to keep track of keys per line (formatting)

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // nCode >= 0 means the event is valid
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                // Marshal reads the Virtual Key Code from memory
                int vkCode = Marshal.ReadInt32(lParam);

                string hour = DateTime.Now.Hour.ToString();
                string minute = DateTime.Now.Minute.ToString();
                string second = DateTime.Now.Second.ToString();

                // Format: KeyCode:H:M:S (e.g., 32:14:05:22)
                buffer.Append(vkCode);
                buffer.Append($":{hour}:{minute}:{second}");

                // Add a space between entries, but wrap to a new line every 10 keys
                if (ok != 9)
                    buffer.Append(" ");

                ok++;
                if (ok == 10)
                {
                    buffer.AppendLine();
                    ok = 0;
                }
            }
            // Pass the event to the next application in the hook chain (CRITICAL)
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If nothing was typed in the last 3 seconds, do nothing
            if (buffer.Length == 0)
                return;

            // Copy buffer content and clear it for the next round
            string data = buffer.ToString();
            buffer.Clear();

            // Ensure the directory exists in AppData
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyrLogs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Write to the daily log file (e.g., 2026-02-14.txt)
            string todayFile = Path.Combine(folder, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            File.AppendAllText(todayFile, data);

            // Also append to a master "total.txt" file for All Time statistics
            string totalFile = Path.Combine(folder, "total.txt");
            File.AppendAllText(totalFile, data);
        }

        // P/Invoke: Imports from Windows User32 and Kernel32 libraries
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}