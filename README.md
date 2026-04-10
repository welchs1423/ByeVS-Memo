# ByeVS-Memo

A lightweight Windows desktop toy project built with only VS Code and the .NET SDK — no heavy Visual Studio required. Created to understand the basic structure of C# WPF and event handling flow.

## 🛠️ Tech Stack & Tools

- **Language**: C#
- **Framework**: .NET 8.0, WPF (Windows Presentation Foundation)
- **IDE / Editor**: Visual Studio Code (C# Dev Kit)
- **Tools**: .NET CLI, Git, GitHub

## 📝 Changelog

### [2026-03-22]

- **Recent Files Menu**: File paths used via `Open` / `Save` are stored in `recent_files.json` (up to 10 entries) and can be reopened by filename from the **Recent Files** menu. Full path is shown in the tooltip. Selecting a deleted file shows a warning and removes it from the list. Menu colors adapt to the current dark/light theme.

### [2026-03-07]

- **Persistent Theme Setting**: Added file I/O logic to save the user's last selected theme (dark/light) to `theme_setting.txt`, so it is restored on next launch.

### [2026-03-01]

- **Dark Mode Support**: Added UI theme toggle (dark/light mode)
- **WPF Layout (XAML)**: Designed an intuitive UI using `Grid` and `StackPanel` to separate the top button area (Open/Save) from the main text input area (`TextBox`).
- **File I/O Integration**: Implemented C# event handler logic using `OpenFileDialog` and `SaveFileDialog` so users can open `.txt` files from their computer and save written content to a text file.
- **Lightweight Build Setup**: Configured the environment to create a project with `dotnet new wpf` and run it instantly with `dotnet run`, using only a terminal and VS Code — no heavy IDE needed.
- **Git Configuration**: Applied a `.NET`-specific `.gitignore` to exclude temporary build artifacts such as `bin/` and `obj/`, keeping the repository clean with only source code pushed to GitHub.
