# ⌨️ KeyTrace: Keyboard & Mouse Analytics

<p align="center">
  <img src="Screenshots/preview.png" alt="KeyTrace Dashboard" width="600">
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

## 🖱️ Mouse Tracking (Current Status)

The **Mouse Tracking Engine** is fully operational and currently logs:
* Left-Click, Right-Click, and Middle-Click counts.
* Precise timestamps for every click.
* Data is stored in `%AppData%/KeyrLogs/Mouse/`.

> **Note:** The Mouse Statistics UI is currently under development. A dedicated dashboard for mouse heatmaps and click-frequency analytics is **coming soon**.

---

## 🚀 Setup: Running the Logger in Background

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

## 📊 Heatmap Color Guide

The dashboard uses a neon-inspired gradient to represent key usage intensity:

| Percentage | Color | Usage Level |
| :--- | :--- | :--- |
| **0%** | 🔘 Deep Gray | Unused |
| **< 1%** | 🔵 Blue/Purple | Occasional |
| **1% - 5%** | 🟢 Teal | Moderate |
| **> 5%** | 🔋 Neon Green | Frequent |

---

## 📁 Data Storage
Your data is kept private and stored locally on your machine:
`%AppData%\Roaming\KeyrLogs`

* **Daily Logs:** `yyyy-MM-dd.txt`
* **Mouse Logs:** `/Mouse/yyyy-MM-dd_mouse.txt`
* **Lifetime Stats:** `total.txt`

---

## 🛠️ Technical Specifications
* **Language:** C# (.NET)
* **Framework:** WPF (Windows Presentation Foundation)
* **APIs:** Win32 API (`User32.dll`, `Kernel32.dll`)
* **Persistence:** Local File System I/O with `StringBuilder` buffering.

---

## ⚠️ Security Notice
This software is intended for **personal productivity analysis only**. Because this tool records keystrokes, it can capture sensitive information. Use this responsibly and strictly for personal data analysis.

---
<p align="center">
  © 2026 KeyTrace Pro. All rights reserved.
</p>
