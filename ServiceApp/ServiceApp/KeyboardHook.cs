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
        // Windows Hook ID for Low-Level Keyboard events
        private const int WH_KEYBOARD_LL = 13;

        // Event ID for a key being pressed
        private const int WM_KEYDOWN = 0x0100; 

        private static StringBuilder buffer = new StringBuilder();
        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback; // Kept in a variable to prevent Garbage Collection
        private static System.Timers.Timer timer;

        // Delegate defining the signature for the hook callback function
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static int counter = 0;

        public static void StartHook()
        {
            _hookId = SetHook(_proc);
            
            // Setup timer to save logs to disk every 3 seconds
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        public static void StopHook()
        {
            // Release the Windows hook
            UnhookWindowsHookEx(_hookId);
            timer?.Stop();
            timer?.Dispose();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            // Link the hook to the current process module
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            // Registers the hook with the OS
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }


        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
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

                if (counter != 9)
                    buffer.Append(" ");

                counter++;
                if (counter == 10)
                {
                    buffer.AppendLine();
                    counter = 0;
                }
            }
            // Pass the event to the next application in the hook chain
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (buffer.Length == 0) return;

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

        // --- Native Windows API Imports ---

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