# ByeVS-Memo

A lightweight Windows desktop toy project built with only VS Code and the .NET SDK — no heavy Visual Studio required. Created to understand the basic structure of C# WPF and event handling flow.

## 🛠️ Tech Stack & Tools

- **Language**: C#
- **Framework**: .NET 8.0, WPF (Windows Presentation Foundation)
- **IDE / Editor**: Visual Studio Code (C# Dev Kit)
- **Tools**: .NET CLI, Git, GitHub

## 📝 Changelog

### [2026-04-10]

- **Timestamp Insert (F5)**: Pressing `F5` inserts the current date and time as ` [yyyy-MM-dd HH:mm] ` at the caret position in the editor. Implemented via a `RoutedCommand` (`InsertTimestampCommand`) bound to `F5` through `Window.InputBindings` and `Window.CommandBindings`. Uses `String.Insert` to splice the timestamp into the existing text without overwriting any content; the caret is advanced to the character immediately after the inserted string.

- **Print (Ctrl+P)**: Added print support via `ApplicationCommands.Print` binding. Invokes `PrintDialog` and renders the current text into a `FlowDocument` fitted to the printer's printable area (50 px padding). Wrapped in `try-catch` to gracefully handle printer errors without crashing the app.
- **Document Statistics**: Added a `통계` button to the top panel. Clicking it displays a `MessageBox` with three metrics: total word count (split on whitespace/tab/newline), UTF-8 byte size (1 byte per ASCII char, 3 bytes per Korean character), and logical line count.
- **Drag & Drop File Open**: Set `AllowDrop="True"` on `MainTextBox`. Dropping any file onto the editor area reads its contents into the text box and adds the path to the recent files list (`recent_files.json`). Only the first file is opened when multiple files are dropped simultaneously; non-file data and missing paths are handled gracefully without crashing.
- **Line Numbers**: Added a line-number gutter to the left of the main text area. A narrow `Border` + `ScrollViewer` panel (45 px wide) sits in `Column 0` of a new two-column `Grid`; the `MainTextBox` occupies `Column 1`. Line numbers are redrawn on every `TextChanged` event by counting `\n` characters (logical lines, O(n) scan via `StringBuilder`). Vertical scroll is synchronized pixel-for-pixel by forwarding `ScrollViewer.ScrollChanged` offsets from the TextBox to the line-number `ScrollViewer`. The gutter background and foreground update automatically when switching between dark mode (`#252526` / `#858585`) and light mode (`#E8E8E8` / `#777777`) via a new `ApplyLineNumberTheme(bool dark)` helper.

- **Status Bar**: Added a bottom status bar (Row 2 of `MainGrid`) that displays three live-updated items on the right side: current cursor position (줄/칸), total character count, and a real-time clock (HH:mm:ss). Updated via `SelectionChanged` and `TextChanged` events; the clock uses a 1-second `DispatcherTimer`.
- **Always-on-Top Toggle**: Added a 📌 `ToggleButton` to the top panel. Clicking it sets `Window.Topmost` to `true`/`false`, keeping the window pinned above all others while active.
- **Auto-Save**: A 1-minute `DispatcherTimer` silently saves the current text to `autosave_temp.txt` in the application folder using `await Task.Run(...)` to avoid UI freezing. Both timers are stopped on window close.
- **Theme Sync**: Status bar background and text colors now update alongside the rest of the UI when switching between dark and light mode.
- **Window Opacity Slider**: Added a slider to the top panel to adjust window transparency from 20% to 100%, preventing the window from becoming fully invisible and unclickable.
- **System Tray Minimize**: Minimizing the window now hides it from the taskbar and places it in the system tray (notification area). Double-clicking the tray icon restores the window. Right-clicking shows a context menu with **Restore** and **Exit** options. Uses `System.Windows.Forms.NotifyIcon` via `UseWindowsForms` in the project file.
- **Keyboard Shortcuts**: Added `Ctrl+N` (new document), `Ctrl+O` (open file), and `Ctrl+S` (save file) via WPF `CommandBindings`.
- **Word Wrap Toggle**: Added a word wrap checkbox to the top panel that toggles `TextWrapping` on the main text area.
- **Font Size via Ctrl+Wheel**: Holding `Ctrl` and scrolling the mouse wheel over the text area increases or decreases the font size (clamped between 8 and 72).
- **Find & Replace Panel**: Added a collapsible search bar above the text area. `Ctrl+F` opens find-only mode; `Ctrl+H` opens find-and-replace mode; pressing the same shortcut again or clicking **닫기** collapses the panel. Supports **다음 찾기** (case-insensitive, wraps around), **바꾸기** (replaces current match then advances), and **모두 바꾸기** (replaces all occurrences with a count notification). `Enter` in either input field triggers the action; `Esc` closes the panel. Panel colors adapt to dark/light theme via `ApplySearchPanelTheme()`.
- **Focus Mode (F11)**: Pressing `F11` collapses the top toolbar and status bar, and switches the window to borderless full-screen (`WindowStyle=None`, `WindowState=Maximized`), leaving only the text area visible. Pressing `F11` or `Esc` restores the previous window state and UI panels.

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
