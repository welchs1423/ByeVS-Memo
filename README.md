# ByeVS-Memo

> A multi-functional text editor for developers and planners — built entirely with VS Code and the .NET SDK, no Visual Studio required.

**ByeVS-Memo** started as a toy project to explore C# WPF fundamentals, and grew into a feature-rich editor through pair programming with an AI agent (Claude Code). 16 core modules were built and shipped in a short period, covering everything from Markdown live preview to easter egg window shake effects.

---

## Features

### Professional Editing

| Feature | Shortcut | Description |
|---|---|---|
| **Markdown Live Preview** | `F8` | Splits the editor left/right. The right pane renders live HTML via the [Markdig](https://github.com/xoofx/markdig) library (tables, task lists, footnotes, etc.) with a 500 ms debounce. Theme-aware CSS adapts to dark/light mode. |
| **Document Outline** | Toolbar button | A collapsible 200 px side panel lists all Markdown headings (`#`, `##`, `###`) extracted from the current text. Each entry is indented proportionally to its level. Clicking any entry scrolls the editor to that line. |
| **Smart Text Snippets** | Space after shorthand | Type a shorthand then press Space to auto-expand: `/date` → current datetime, `/todo` → `[ ] `, `/sign` → `- Author: [username] -`. |
| **Find & Replace** | `Ctrl+F` / `Ctrl+H` | Collapsible panel with Find Next (case-insensitive, wraps around), Replace, and Replace All. `Enter` triggers the action; `Esc` closes the panel. |
| **Text Formatting** | Right-click | Context menu on the editor: convert selection to UPPERCASE / lowercase, or remove all empty lines. Operates on selected text only, or the full document if nothing is selected. |
| **Timestamp Insert** | `F5` | Inserts `[yyyy-MM-dd HH:mm]` at the caret position without overwriting any existing content. |
| **Drag & Drop** | Drop onto editor | Drop any file onto the editor area to open it. The path is added to the recent files list automatically. |
| **Print** | `Ctrl+P` | Renders the current text into a `FlowDocument` fitted to the printer's printable area (50 px padding). |
| **Document Statistics** | Stats button | Displays a popup with word count, UTF-8 byte size, and line count. |
| **History Backup** | On every save | A timestamped copy (e.g. `filename_20260411_193000.txt`) is automatically written to a `Backups/` folder next to the executable. |
| **Auto-Save** | Every 1 minute | Silently saves the current text to `autosave_temp.txt` in the application folder via `async Task.Run` to avoid UI freezing. |
| **Line Numbers** | Always on | A 45 px gutter to the left of the editor, synchronized pixel-for-pixel with the editor's scroll position. Colors adapt to dark/light theme. |

---

### UI / UX

| Feature | Shortcut | Description |
|---|---|---|
| **Sticky Note Mode** | `F12` | Removes the window frame, hides the toolbar and status bar, and resizes the window to 300×300. The note floats anywhere on screen and can be dragged by clicking anywhere on the text area. |
| **Focus Mode** | `F11` | Borderless fullscreen — hides the toolbar and status bar, leaving only the text area. Press `F11` or `Esc` to exit. |
| **Dark / Light Theme** | Theme button | Full dark mode with persisted preference (`theme_setting.txt`). All panels — toolbar, status bar, gutter, search panel, outline panel, and Markdown preview CSS — adapt in sync. |
| **Always on Top** | 📌 button | Pins the window above all other windows until toggled off. |
| **Window Opacity** | Slider | Adjusts window transparency from 20% to 100%, preventing the window from becoming fully invisible. |
| **Word Wrap** | Checkbox | Toggles `TextWrapping` on the main text area. |
| **Font Size** | `Ctrl+Wheel` | Scroll with Ctrl held to increase or decrease font size (clamped to 8–72 px). |
| **Status Bar** | Always on | Displays cursor position (line/column), total character count, and a live HH:mm:ss clock. |
| **System Tray** | Minimize | Minimizing hides the window from the taskbar and places it in the system tray. Double-click or right-click the tray icon to restore or exit. |
| **Recent Files** | File menu | Up to 10 recently opened or saved paths are stored in `recent_files.json`. Selecting a deleted entry shows a warning and removes it from the list. |

---

### Easter Eggs

> Three hidden modes that make the editor a little more fun.

- **Window Shake Mode** (`⌨️` button) — When rapid typing is detected, the window shakes left and right with a short animation, simulating the feel of a mechanical keyboard impact.
- **Hacker Mode** (`🟩` button) — Applies a Matrix-style theme: black background, neon-green monospace font. Toggle off to restore the previous theme.
- **Secret Snippets** — Two personal shorthand expansions are hidden inside the snippet engine. Type them and press Space to find out what they do.

---

## Keyboard Shortcuts

| Key | Action |
|---|---|
| `Ctrl+N` | New document |
| `Ctrl+O` | Open file |
| `Ctrl+S` | Save file |
| `Ctrl+P` | Print |
| `Ctrl+F` | Find |
| `Ctrl+H` | Find & Replace |
| `Ctrl+Wheel` | Adjust font size |
| `F5` | Insert timestamp |
| `F8` | Toggle Markdown preview |
| `F11` | Toggle Focus Mode |
| `F12` | Toggle Sticky Note Mode |
| `Esc` | Exit Focus Mode / Close search panel |

---

## Tech Stack

| Category | Details |
|---|---|
| **Language** | C# |
| **Framework** | .NET 8.0, WPF (Windows Presentation Foundation) |
| **Library** | [Markdig](https://github.com/xoofx/markdig) 0.38.0 — Markdown parsing |
| **Interop** | `System.Windows.Forms.NotifyIcon` via `UseWindowsForms` (for system tray) |
| **IDE** | Visual Studio Code (C# Dev Kit), Cursor |
| **AI Tools** | Claude Code (Agent), GitHub Copilot |
| **Build Tools** | .NET CLI, Git, GitHub |

No Visual Studio. Built entirely with `dotnet new wpf`, `dotnet run`, and a terminal.

---

## Build & Run

```bash
# Run in development
dotnet run

# Build only
dotnet build

# Publish self-contained executable for Windows x64
dotnet publish -c Release -r win-x64 --self-contained
```

---

## Runtime Files

These files are created next to the executable at runtime and are excluded from the repository:

| File / Folder | Purpose |
|---|---|
| `theme_setting.txt` | Persisted dark/light preference |
| `recent_files.json` | Recent file list (up to 10 paths) |
| `autosave_temp.txt` | Auto-save output (written every 1 minute) |
| `Backups/` | Timestamped backup copies created on every save |

---

## Project Background

This project was built to understand the fundamentals of C# WPF — event handling, layout systems, commands, timers, and interop — without a heavy IDE. Every feature was designed, coded, and iterated through a real-time conversation with Claude Code, making it an experiment in AI-assisted solo development as much as a useful desktop tool.
