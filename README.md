# KeyTrace: Keyboard & Mouse Analytics

<p align="center">
  <img src="KeyrUI/KeyrUI/Screenshots/preview.png" alt="KeyTrace Dashboard" width="600">
</p>

**KeyTrace** is a comprehensive solution for monitoring and visualizing your peripheral activity. It transforms raw input data into an intuitive, color-coded **Heatmap**, allowing you to analyze your typing habits and mouse usage over different periods.

---

## 🏗️ System Architecture

The project is split into two specialized components:

### 1. ServiceApp (The Logger)
* **Background Operation:** Runs silently in the background with minimal CPU footprint.
* **Low-Level Hooks:** Utilizes `WH_KEYBOARD_LL` and `WH_MOUSE_LL` via the Win32 API to capture system-wide inputs.
* **Smart Buffering:** To protect your SSD/HDD, data is buffered in memory and flushed to disk every 3-5 seconds.
* **Storage:** Data is organized by date in `%AppData%/KeyrLogs`.

### 2. KeyTrace Visualizer (The Dashboard)
* **WPF Interface:** A modern, high-performance UI built with XAML.
* **Heatmap Logic:** Dynamically calculates frequency percentages and updates the keyboard layout from **Cold (Gray/Blue)** to **Hot (Neon Green)**.
* **Time Filtering:** Analyze data for Today, Yesterday, Week, Month, Year, or All Time.

---

## Setup: Running the Logger in Background

To ensure accurate statistics, `ServiceApp.exe` should run continuously.

### Option 1: The Startup Folder (Quickest)
1. Press `Win + R`, type `shell:startup`, and hit **Enter**.
2. Create a shortcut to `ServiceApp.exe` in this folder.
3. The logger will now start automatically every time you log into Windows.

### Option 2: Task Scheduler (Recommended)
1. Search for **Task Scheduler** in the Start menu.
2. Create a Basic Task named `KeyTraceLogger`.
3. Set the trigger to **"When I log on"**.
4. Set the action to **"Start a program"** and select `ServiceApp.exe`.

---

## Data Storage
Your data is kept private and stored locally on your machine:
`%AppData%\Roaming\KeyrLogs`

---

## ⚠️ Security Notice
This software is intended for **personal productivity analysis only**. Because this tool records keystrokes, it can capture sensitive information. Use this responsibly and strictly for personal data analysis.
