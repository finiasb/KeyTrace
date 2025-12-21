# ⌨️ Keyr: Keyboard Heatmap & Analytics

**Keyr** is a comprehensive solution for monitoring and visualizing your keyboard activity. It transforms raw keystroke data into an intuitive, color-coded **Heatmap**, allowing you to analyze your typing habits over different periods.

---

## 🏗️ System Architecture

The project is split into two specialized components:

1.  **ServiceApp (The Logger)**
    * Runs silently in the background.
    * Utilizes a `Low-Level Keyboard Hook` to capture system-wide keystrokes.
    * Buffers data and saves it every 3 seconds to text files in `%AppData%/KeyrLogs`.

2.  **Keyr Visualizer (The Dashboard)**
    * A WinForms graphical interface.
    * Parses logs and calculates frequency percentages for 256 key codes.
    * **Heatmap Logic:** Dynamically updates button colors from **Gray** (unused) to **Red** (most used).

---

## 🚀 Setup: Running the Logger in Background

To ensure accurate statistics, the `ServiceApp.exe` should run continuously. Follow these steps to set it up as a background process:

### Option 1: The Startup Folder (Quickest)
1. Press `Win + R`, type `shell:startup`, and hit **Enter**.
2. Right-click and select **New > Shortcut**.
3. Browse to your `ServiceApp.exe` location and select it.
4. The logger will now start automatically every time you log into Windows.

### Option 2: Task Scheduler (Recommended for Silent Running)
1. Search for **Task Scheduler** in the Start menu.
2. Click **Create Basic Task** and name it `KeyrLogger`.
3. Set the **Trigger** to "When I log on".
4. Set the **Action** to "Start a program" and select your `ServiceApp.exe`.
5. Once finished, open the task **Properties** and check **"Run with highest privileges"** to ensure it captures keys in all applications.

---

## 📊 Heatmap Color Guide

The dashboard uses a color gradient to represent key usage intensity:

| Percentage | Color | Usage Level |
| :--- | :--- | :--- |
| **0%** | 🔘 Gray | Unused |
| **< 1%** | 🔵 Light Blue | Occasional |
| **1% - 5%** | 🟢 Green | Moderate |
| **5% - 10%** | 🟠 Orange | Frequent |
| **> 10%** | 🔴 Dark Red | Highest Usage |

---

## 📁 Data Storage
Your data is kept private and stored locally on your machine at:
`%AppData%\Roaming\KeyrLogs`

The files are named by date (e.g., `2025-12-22.txt`) for history tracking, plus a `total.txt` for lifetime statistics.

---

## 🛠️ Technical Specifications
* **Language:** C# (.NET)
* **APIs:** Win32 API (`User32.dll`, `Kernel32.dll`)
* **Hook Method:** `WH_KEYBOARD_LL` (Global Keyboard Hook)
* **Persistence:** Local File System I/O

---

## ⚠️ Security Notice
This software is intended for **personal productivity analysis only**. A keylogger records everything typed, which may include sensitive information. Use this responsibly and do not install it on devices you do not own.