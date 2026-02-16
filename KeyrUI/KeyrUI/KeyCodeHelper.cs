using System;
using System.Collections.Generic;

namespace KeyrUI
{
    public static class KeyCodeHelper
    {
        private static readonly Dictionary<string, int> KeyTextMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "ESC", 27 },
            { "F1", 112 },
            { "F2", 113 },
            { "F3", 114 },
            { "F4", 115 },
            { "F5", 116 },
            { "F6", 117 },
            { "F7", 118 },
            { "F8", 119 },
            { "F9", 120 },
            { "F10", 121 },
            { "F11", 122 },
            { "F12", 123 },
            { "`", 192 },
            { "~", 192 },
            { "1", 49 },
            { "2", 50 },
            { "3", 51 },
            { "4", 52 },
            { "5", 53 },
            { "6", 54 },
            { "7", 55 },
            { "8", 56 },
            { "9", 57 },
            { "0", 48 },
            { "-", 189 },
            { "=", 187 },
            { "BACK", 8 },
            { "TAB", 9 },
            { "Q", 81 },
            { "W", 87 },
            { "E", 69 },
            { "R", 82 },
            { "T", 84 },
            { "Y", 89 },
            { "U", 85 },
            { "I", 73 },
            { "O", 79 },
            { "P", 80 },
            { "[", 219 },
            { "]", 221 },
            { "\\", 220 },
            { "CAPS", 20 },
            { "A", 65 },
            { "S", 83 },
            { "D", 68 },
            { "F", 70 },
            { "G", 71 },
            { "H", 72 },
            { "J", 74 },
            { "K", 75 },
            { "L", 76 },
            { ";", 186 },
            { "'", 222 },
            { "ENTER", 13 },
            { "SHIFT", 160 },
            { "Z", 90 },
            { "X", 88 },
            { "C", 67 },
            { "V", 86 },
            { "B", 66 },
            { "N", 78 },
            { "M", 77 },
            { ",", 188 },
            { ".", 190 },
            { "/", 191 },
            { "CTRL", 162 },
            { "WIN", 91 },
            { "ALT", 164 },
            { "SPACE", 32 }
        };

        public static int GetKeyCode(string text)
        {
            return KeyTextMap.TryGetValue(text, out int keyCode) ? keyCode : -1;
        }
    }
}