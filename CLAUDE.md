# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Run in development
dotnet run

# Build only
dotnet build

# Build release
dotnet build -c Release

# Publish self-contained executable
dotnet publish -c Release -r win-x64 --self-contained
```

There are no automated tests in this project.

## Architecture

This is a single-window WPF desktop application targeting .NET 8.0. The entire UI and logic live in two files:

- [MainWindow.xaml](MainWindow.xaml) — XAML layout with a 3-row `MainGrid`: `TopPanelContainer` (Row 0), editor area (Row 1), `MainStatusBar` (Row 2).
- [MainWindow.xaml.cs](MainWindow.xaml.cs) — All event handlers, commands, and UI state logic as a single `partial class MainWindow`.
- [RecentFilesStore.cs](RecentFilesStore.cs) — Static helper that reads/writes `recent_files.json` (up to 10 paths) next to the executable.
- [App.xaml.cs](App.xaml.cs) — Minimal `App` entry point; no custom startup logic.

### Key design points

**Theme system**: `isDarkMode` bool drives all color changes. `ThemeButton_Click` sets colors imperatively (no XAML resources/styles), then calls `ApplySearchPanelTheme()` and `ApplyLineNumberTheme()`. The current theme is persisted to `theme_setting.txt` next to the executable.

**Window modes**: Two independent boolean flags track mutually-exclusive display modes:
- `_isFocusMode` (F11) — borderless fullscreen, hides `TopPanelContainer` and `MainStatusBar`
- `_isStickyNoteMode` (F12) — frameless 300×300 sticky note, hides top panel and status bar, enables drag-to-move via `PreviewMouseLeftButtonDown/Move/Up` on `MainTextBox`

**Timers**: Two `DispatcherTimer` instances are created in the constructor (not shown in the partial — they are initialized via `InitializeComponent` flow or field initializers). Both are stopped in `OnClosed`. The clock timer fires every second; the auto-save timer fires every minute and writes to `autosave_temp.txt`.

**WinForms interop**: `System.Windows.Forms.NotifyIcon` is used for system tray support (`UseWindowsForms` is enabled in the csproj). The `System.Windows.Forms` global using is explicitly removed to avoid `Application`/`MessageBox` namespace conflicts — always qualify WinForms types fully (e.g., `System.Windows.Forms.NotifyIcon`).

**Commands**: Two custom `RoutedCommand` statics on `MainWindow` (`InsertTimestampCommand` for F5, `ToggleStickyNoteModeCommand` for F12) are declared and bound in XAML `CommandBindings`.

**Runtime files** (written next to the executable, not in the repo):
- `theme_setting.txt` — persisted dark/light preference
- `recent_files.json` — recent file list (managed by `RecentFilesStore`)
- `autosave_temp.txt` — auto-save output
- `Backups/` — timestamped backup copies created on every save
