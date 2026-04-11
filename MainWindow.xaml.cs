using System.Globalization;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Markdig;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace ByeVS_Memo
{
    public partial class MainWindow : Window
    {
        // ── 테마/상태 관련 필드 ───────────────────────────────────────
        private bool isDarkMode = false;
        private readonly DispatcherTimer _clockTimer;
        private readonly DispatcherTimer _autoSaveTimer;
        public static readonly RoutedCommand InsertTimestampCommand = new RoutedCommand();
        private bool _isFocusMode = false;
        private WindowState _prevWindowState = WindowState.Normal;
        private WindowStyle _prevWindowStyle = WindowStyle.SingleBorderWindow;
        private System.Windows.Forms.NotifyIcon _notifyIcon = null!;

        // ── 포스트잇 모드 관련 필드 ───────────────────────────────────
        public static readonly RoutedCommand ToggleStickyNoteModeCommand = new RoutedCommand();
        private bool _isStickyNoteMode = false;
        private WindowStyle _prevStickyWindowStyle;
        private ResizeMode _prevStickyResizeMode;
        private double _prevStickyWidth;
        private double _prevStickyHeight;
        private double _prevStickyLeft;
        private double _prevStickyTop;
        private Visibility _prevTopPanelVisibility;
        private Visibility _prevStatusBarVisibility;
        private bool _isDraggingSticky = false;
        private Point _dragStartPoint;

        // ── 마크다운 미리보기 관련 필드 ───────────────────────────────
        public static readonly RoutedCommand ToggleMarkdownPreviewCommand = new RoutedCommand();
        private bool _isMarkdownPreviewMode = false;
        private DispatcherTimer? _markdownUpdateTimer;

        // ── 생성자 ───────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();

            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();

            _autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;
            _autoSaveTimer.Start();

            LoadTheme();
            RefreshRecentFilesMenu();
        }

        private void LoadTheme()
        {
            try
            {
                if (File.Exists("theme_setting.txt") &&
                    File.ReadAllText("theme_setting.txt").Trim() == "Dark")
                {
                    // isDarkMode는 false이므로 ThemeButton_Click이 true로 전환
                    ThemeButton_Click(this, new RoutedEventArgs());
                }
            }
            catch { }
        }

        // ── [Ctrl+N] 새 문서 ─────────────────────────────────────────
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainTextBox.Clear();
        }

        // ── [Ctrl+O] 열기 ─────────────────────────────────────────────
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenButton_Click(sender, e);
        }

        // ── [Ctrl+S] 저장 ─────────────────────────────────────────────
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
        }

        // ── [Ctrl+P] 인쇄 ─────────────────────────────────────────────
        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new System.Windows.Controls.PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var doc = new System.Windows.Documents.FlowDocument(
                        new System.Windows.Documents.Paragraph(
                            new System.Windows.Documents.Run(MainTextBox.Text)))
                    {
                        PagePadding = new Thickness(50)
                    };
                    var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator;
                    paginator.PageSize = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
                    dlg.PrintDocument(paginator, "ByeVS-Memo 인쇄");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"인쇄 중 오류가 발생했습니다:\n{ex.Message}", "인쇄 오류",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ── [열기] 버튼 ───────────────────────────────────────────────
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일(*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                MainTextBox.Text = File.ReadAllText(dlg.FileName);
                RecentFilesStore.Add(dlg.FileName);
                RefreshRecentFilesMenu();
            }
        }

        // ── [저장] 버튼 ───────────────────────────────────────────────
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, MainTextBox.Text);
                    TryHistoryBackup(dlg.FileName);
                    RecentFilesStore.Add(dlg.FileName);
                    RefreshRecentFilesMenu();
                    MessageBox.Show("저장이 완료되었습니다!", "알림");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"저장 중 오류가 발생했습니다:\n{ex.Message}", "저장 오류",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ── [통계] 버튼 ───────────────────────────────────────────────
        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MainTextBox.Text;
            int wordCount = string.IsNullOrWhiteSpace(text) ? 0
                : text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int byteCount = Encoding.UTF8.GetByteCount(text);
            int lineCount = text.Length == 0 ? 1 : text.Split('\n').Length;
            MessageBox.Show(
                $"단어 수: {wordCount}\n바이트 크기 (UTF-8): {byteCount} Bytes\n줄 수: {lineCount}",
                "문서 통계", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── [자동 줄바꿈] 체크박스 ───────────────────────────────────
        private void WordWrapCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (MainTextBox == null) return;
            MainTextBox.TextWrapping = WordWrapCheckBox.IsChecked == true
                ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        // ── 안전 백업 ────────────────────────────────────────────────
        private void TryHistoryBackup(string originalFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalFilePath) || !File.Exists(originalFilePath))
                    return;

                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string backupDir = Path.Combine(appDir, "Backups");
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);

                string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
                string ext = Path.GetExtension(originalFilePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                string backupPath = Path.Combine(backupDir, $"{fileName}_{timestamp}{ext}");
                File.Copy(originalFilePath, backupPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("백업 폴더 생성 또는 파일 복사 권한이 없습니다.\n관리자 권한으로 실행하거나 폴더 권한을 확인하세요.",
                    "백업 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HistoryBackup] {ex.Message}");
            }
        }

        // ── 최근 파일 메뉴 ───────────────────────────────────────────
        private void RefreshRecentFilesMenu()
        {
            RecentFilesMenu.Items.Clear();
            foreach (string path in RecentFilesStore.Load())
            {
                var item = new MenuItem { Header = Path.GetFileName(path), ToolTip = path, Tag = path };
                item.Click += RecentFileMenuItem_Click;
                RecentFilesMenu.Items.Add(item);
            }
            if (RecentFilesMenu.Items.Count == 0)
                RecentFilesMenu.Items.Add(new MenuItem { Header = "최근 파일 없음", IsEnabled = false });
        }

        private void RecentFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem mi || mi.Tag is not string path) return;
            if (!File.Exists(path))
            {
                MessageBox.Show($"파일을 찾을 수 없습니다:\n{path}", "경고",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                RecentFilesStore.Remove(path);
                RefreshRecentFilesMenu();
                return;
            }
            MainTextBox.Text = File.ReadAllText(path);
            RecentFilesStore.Add(path);
            RefreshRecentFilesMenu();
        }

        // ── [다크 모드] 버튼 ─────────────────────────────────────────
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                MainGrid.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                MainTextBox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                MainTextBox.Foreground = new SolidColorBrush(Colors.White);
                TopPanel.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                MainMenu.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                MainMenu.Foreground = new SolidColorBrush(Colors.White);
                ThemeButton.Content = "라이트 모드";
                MainStatusBar.Background = new SolidColorBrush(Color.FromRgb(37, 37, 38));
                CursorPosText.Foreground = new SolidColorBrush(Colors.White);
                CharCountText.Foreground = new SolidColorBrush(Colors.White);
                ClockText.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                MainGrid.Background = new SolidColorBrush(Colors.White);
                MainTextBox.Background = new SolidColorBrush(Colors.White);
                MainTextBox.Foreground = new SolidColorBrush(Colors.Black);
                TopPanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                MainMenu.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                MainMenu.Foreground = new SolidColorBrush(Colors.Black);
                ThemeButton.Content = "다크 모드";
                MainStatusBar.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                CursorPosText.Foreground = new SolidColorBrush(Colors.Black);
                CharCountText.Foreground = new SolidColorBrush(Colors.Black);
                ClockText.Foreground = new SolidColorBrush(Colors.Black);
            }

            ApplySearchPanelTheme(isDarkMode);
            ApplyLineNumberTheme(isDarkMode);
            if (_isMarkdownPreviewMode) UpdateMarkdownPreview();
            File.WriteAllText("theme_setting.txt", isDarkMode ? "Dark" : "Light");
        }

        // ── 투명도 슬라이더 ──────────────────────────────────────────
        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = e.NewValue;
        }

        // ── 시스템 트레이 ────────────────────────────────────────────
        private void InitializeNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Text = "ByeVS-Memo",
                Visible = false
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            var openItem = new System.Windows.Forms.ToolStripMenuItem("열기");
            var exitItem = new System.Windows.Forms.ToolStripMenuItem("종료");
            openItem.Click += (s, ev) => RestoreFromTray();
            exitItem.Click += (s, ev) => System.Windows.Application.Current.Shutdown();
            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, ev) => RestoreFromTray();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized && !_isFocusMode)
            {
                Hide();
                ShowInTaskbar = false;
                _notifyIcon.Visible = true;
            }
            base.OnStateChanged(e);
        }

        private void RestoreFromTray()
        {
            Show();
            ShowInTaskbar = true;
            WindowState = WindowState.Normal;
            _notifyIcon.Visible = false;
            Activate();
        }

        protected override void OnClosed(EventArgs e)
        {
            _clockTimer.Stop();
            _autoSaveTimer.Stop();
            _markdownUpdateTimer?.Stop();
            _notifyIcon.Dispose();
            base.OnClosed(e);
        }

        // ── [F5] 타임스탬프 삽입 ─────────────────────────────────────
        private void InsertTimestampCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string ts = $" [{DateTime.Now:yyyy-MM-dd HH:mm}] ";
            int caret = MainTextBox.CaretIndex;
            MainTextBox.Text = MainTextBox.Text.Insert(caret, ts);
            MainTextBox.CaretIndex = caret + ts.Length;
            e.Handled = true;
        }

        // ── [F11] 몰입 모드 / Esc 복귀 ───────────────────────────────
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11) { ToggleFocusMode(); e.Handled = true; }
            else if (e.Key == Key.Escape && _isFocusMode) { ToggleFocusMode(); e.Handled = true; }
        }

        private void ToggleFocusMode()
        {
            _isFocusMode = !_isFocusMode;
            if (_isFocusMode)
            {
                _prevWindowState = WindowState;
                _prevWindowStyle = WindowStyle;
                TopPanelContainer.Visibility = Visibility.Collapsed;
                MainStatusBar.Visibility = Visibility.Collapsed;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                MainTextBox.Focus();
            }
            else
            {
                TopPanelContainer.Visibility = Visibility.Visible;
                MainStatusBar.Visibility = Visibility.Visible;
                WindowStyle = _prevWindowStyle;
                WindowState = _prevWindowState;
            }
        }

        // ── [항상 위로] 토글 ─────────────────────────────────────────
        private void TopmostButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = TopmostButton.IsChecked == true;
        }

        // ── [F12] 포스트잇 모드 ──────────────────────────────────────
        private void ToggleStickyNoteModeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleStickyNoteMode();
            e.Handled = true;
        }

        private void StickyNoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleStickyNoteMode();
        }

        private void ToggleStickyNoteMode()
        {
            if (_isStickyNoteMode)
            {
                WindowStyle = _prevStickyWindowStyle;
                ResizeMode = _prevStickyResizeMode;
                Width = _prevStickyWidth;
                Height = _prevStickyHeight;
                Left = _prevStickyLeft;
                Top = _prevStickyTop;
                TopPanelContainer.Visibility = _prevTopPanelVisibility;
                MainStatusBar.Visibility = _prevStatusBarVisibility;
                StickyNoteButton.Content = "🗒️ 포스트잇";
                MainTextBox.Cursor = Cursors.IBeam;
                _isStickyNoteMode = false;
            }
            else
            {
                _prevStickyWindowStyle = WindowStyle;
                _prevStickyResizeMode = ResizeMode;
                _prevStickyWidth = Width;
                _prevStickyHeight = Height;
                _prevStickyLeft = Left;
                _prevStickyTop = Top;
                _prevTopPanelVisibility = TopPanelContainer.Visibility;
                _prevStatusBarVisibility = MainStatusBar.Visibility;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Width = 300;
                Height = 300;
                Left = (SystemParameters.WorkArea.Width - 300) / 2 + SystemParameters.WorkArea.Left;
                Top = (SystemParameters.WorkArea.Height - 300) / 2 + SystemParameters.WorkArea.Top;
                TopPanelContainer.Visibility = Visibility.Collapsed;
                MainStatusBar.Visibility = Visibility.Collapsed;
                StickyNoteButton.Content = "🗒️ 일반모드";
                MainTextBox.Cursor = Cursors.SizeAll;
                _isStickyNoteMode = true;
            }
        }

        // ── 포스트잇 모드: 드래그로 창 이동 ─────────────────────────
        private void MainTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isStickyNoteMode && e.ButtonState == MouseButtonState.Pressed)
            {
                _isDraggingSticky = true;
                _dragStartPoint = e.GetPosition(this);
                MainTextBox.CaptureMouse();
            }
        }

        private void MainTextBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isStickyNoteMode && _isDraggingSticky && e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void MainTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isStickyNoteMode)
            {
                _isDraggingSticky = false;
                MainTextBox.ReleaseMouseCapture();
            }
        }

        // ── Ctrl + 마우스 휠: 글꼴 크기 조절 ────────────────────────
        private void MainTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double newSize = MainTextBox.FontSize + (e.Delta > 0 ? 1 : -1);
                MainTextBox.FontSize = Math.Clamp(newSize, 8, 72);
                e.Handled = true;
            }
        }

        // ── 드래그 앤 드롭 파일 열기 ─────────────────────────────────
        private void MainTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void MainTextBox_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null || files.Length == 0) return;
                string path = files[0];
                if (!File.Exists(path))
                {
                    MessageBox.Show($"파일을 찾을 수 없습니다:\n{path}", "경고",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MainTextBox.Text = File.ReadAllText(path);
                RecentFilesStore.Add(path);
                RefreshRecentFilesMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일을 여는 중 오류가 발생했습니다:\n{ex.Message}", "오류",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── 컨텍스트 메뉴: 텍스트 변환 ──────────────────────────────
        private void MenuItem_Uppercase_Click(object sender, RoutedEventArgs e) =>
            ApplyTextTransform(s => s.ToUpper());

        private void MenuItem_Lowercase_Click(object sender, RoutedEventArgs e) =>
            ApplyTextTransform(s => s.ToLower());

        private void MenuItem_RemoveEmptyLines_Click(object sender, RoutedEventArgs e) =>
            ApplyTextTransform(s => string.Join("\n",
                s.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line))));

        private void ApplyTextTransform(Func<string, string> transform)
        {
            var tb = MainTextBox;
            if (tb == null) return;
            string result;
            if (tb.SelectionLength > 0)
            {
                int selStart = tb.SelectionStart, selLen = tb.SelectionLength;
                result = transform(tb.SelectedText);
                tb.Text = tb.Text.Remove(selStart, selLen).Insert(selStart, result);
                tb.Select(selStart, result.Length);
            }
            else
            {
                result = transform(tb.Text);
                tb.Text = result;
                tb.Select(0, tb.Text.Length);
            }
            MainTextBox_TextChanged(tb, null!);
            MainTextBox_SelectionChanged(tb, null!);
        }

        // ── 상태바: 커서 위치 ────────────────────────────────────────
        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateCursorPosition();
        }

        // ── 상태바: 글자 수 + 줄 번호 + MD 미리보기 트리거 ──────────
        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCharCount();
            UpdateLineNumbers();
            if (_isMarkdownPreviewMode && _markdownUpdateTimer != null)
            {
                _markdownUpdateTimer.Stop();
                _markdownUpdateTimer.Start();
            }
        }

        private void UpdateCursorPosition()
        {
            int caretIndex = MainTextBox.CaretIndex;
            int lineIndex = MainTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            if (lineIndex < 0) return;
            int col = caretIndex - MainTextBox.GetCharacterIndexFromLineIndex(lineIndex) + 1;
            CursorPosText.Text = $"줄: {lineIndex + 1}, 칸: {col}";
        }

        private void UpdateCharCount()
        {
            CharCountText.Text = $"글자 수: {MainTextBox.Text.Length}";
        }

        // ── 줄 번호 ──────────────────────────────────────────────────
        private void UpdateLineNumbers()
        {
            string text = MainTextBox.Text;
            int lineCount = 1;
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\n') lineCount++;
            var sb = new StringBuilder(lineCount * 3);
            for (int i = 1; i <= lineCount; i++)
            {
                if (i > 1) sb.Append('\n');
                sb.Append(i);
            }
            LineNumbersText.Text = sb.ToString();
        }

        private void MainTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LineNumberScroller.ScrollToVerticalOffset(e.VerticalOffset);
        }

        // ── 시계 타이머 ──────────────────────────────────────────────
        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // ── 자동 저장 타이머 ─────────────────────────────────────────
        private async void AutoSaveTimer_Tick(object? sender, EventArgs e)
        {
            string content = MainTextBox.Text;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "autosave_temp.txt");
            await Task.Run(() => File.WriteAllText(path, content));
        }

        // ── 스마트 상용구 (Text Snippets) ───────────────────────────
        private void MainTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;

            int caret = MainTextBox.CaretIndex;
            string textBefore = MainTextBox.Text.Substring(0, caret);

            string? snippet = null;
            string? expansion = null;

            if (textBefore.EndsWith("/date"))
            {
                snippet = "/date";
                expansion = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            else if (textBefore.EndsWith("/todo"))
            {
                snippet = "/todo";
                expansion = "[ ] ";
            }
            else if (textBefore.EndsWith("/sign"))
            {
                snippet = "/sign";
                expansion = $"- 작성자: {Environment.UserName} -";
            }

            if (snippet == null) return;

            int snippetStart = caret - snippet.Length;
            string newText = MainTextBox.Text.Remove(snippetStart, snippet.Length).Insert(snippetStart, expansion!);
            MainTextBox.Text = newText;
            MainTextBox.CaretIndex = snippetStart + expansion!.Length;
            e.Handled = true;
        }

        // ── 테마: 줄 번호 영역 ───────────────────────────────────────
        private void ApplyLineNumberTheme(bool dark)
        {
            LineNumberBorder.Background = dark
                ? new SolidColorBrush(Color.FromRgb(37, 37, 38))
                : new SolidColorBrush(Color.FromRgb(232, 232, 232));
            LineNumbersText.Foreground = dark
                ? new SolidColorBrush(Color.FromRgb(133, 133, 133))
                : new SolidColorBrush(Color.FromRgb(119, 119, 119));
        }

        // ── 테마: 검색 패널 ──────────────────────────────────────────
        private void ApplySearchPanelTheme(bool dark)
        {
            if (dark)
            {
                SearchPanel.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                SearchPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                FindLabel.Foreground = new SolidColorBrush(Colors.White);
                ReplaceLabel.Foreground = new SolidColorBrush(Colors.White);
                SearchStatusText.Foreground = new SolidColorBrush(Color.FromRgb(160, 160, 160));
                FindTextBox.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                FindTextBox.Foreground = new SolidColorBrush(Colors.White);
                FindTextBox.CaretBrush = new SolidColorBrush(Colors.White);
                ReplaceTextBox.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                ReplaceTextBox.Foreground = new SolidColorBrush(Colors.White);
                ReplaceTextBox.CaretBrush = new SolidColorBrush(Colors.White);
            }
            else
            {
                SearchPanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                SearchPanel.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                FindLabel.Foreground = new SolidColorBrush(Colors.Black);
                ReplaceLabel.Foreground = new SolidColorBrush(Colors.Black);
                SearchStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                FindTextBox.Background = new SolidColorBrush(Colors.White);
                FindTextBox.Foreground = new SolidColorBrush(Colors.Black);
                FindTextBox.CaretBrush = new SolidColorBrush(Colors.Black);
                ReplaceTextBox.Background = new SolidColorBrush(Colors.White);
                ReplaceTextBox.Foreground = new SolidColorBrush(Colors.Black);
                ReplaceTextBox.CaretBrush = new SolidColorBrush(Colors.Black);
            }
        }

        // ── [Ctrl+F/H] 찾기 / 바꾸기 패널 ───────────────────────────
        private void ToggleSearchPanel(bool showReplace)
        {
            bool isOpen = SearchPanel.Visibility == Visibility.Visible;
            bool isReplaceMode = ReplaceLabel.Visibility == Visibility.Visible;
            if (isOpen && isReplaceMode == showReplace)
            {
                SearchPanel.Visibility = Visibility.Collapsed;
                MainTextBox.Focus();
                return;
            }
            SearchPanel.Visibility = Visibility.Visible;
            var replaceVis = showReplace ? Visibility.Visible : Visibility.Collapsed;
            ReplaceLabel.Visibility = replaceVis;
            ReplaceTextBox.Visibility = replaceVis;
            ReplaceButton.Visibility = replaceVis;
            ReplaceAllButton.Visibility = replaceVis;
            FindTextBox.Focus();
            FindTextBox.SelectAll();
            SearchStatusText.Text = string.Empty;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.F) { ToggleSearchPanel(showReplace: false); e.Handled = true; }
                else if (e.Key == Key.H) { ToggleSearchPanel(showReplace: true); e.Handled = true; }
            }
            base.OnPreviewKeyDown(e);
        }

        private void FindTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { FindNext_Click(sender, e); e.Handled = true; }
            else if (e.Key == Key.Escape) { CloseSearch_Click(sender, e); e.Handled = true; }
        }

        private void ReplaceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Replace_Click(sender, e); e.Handled = true; }
            else if (e.Key == Key.Escape) { CloseSearch_Click(sender, e); e.Handled = true; }
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;
            string content = MainTextBox.Text;
            int startIndex = MainTextBox.SelectionStart + MainTextBox.SelectionLength;
            int found = content.IndexOf(searchText, startIndex, StringComparison.OrdinalIgnoreCase);
            bool wrapped = false;
            if (found < 0 && startIndex > 0)
            {
                found = content.IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase);
                wrapped = found >= 0;
            }
            if (found >= 0)
            {
                MainTextBox.Focus();
                MainTextBox.Select(found, searchText.Length);
                int lineIdx = MainTextBox.GetLineIndexFromCharacterIndex(found);
                if (lineIdx >= 0) MainTextBox.ScrollToLine(lineIdx);
                SearchStatusText.Text = wrapped ? "(처음부터 다시 검색)" : string.Empty;
            }
            else
            {
                SearchStatusText.Text = "찾을 수 없음";
            }
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextBox.Text;
            string replaceText = ReplaceTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;
            if (string.Equals(MainTextBox.SelectedText, searchText, StringComparison.OrdinalIgnoreCase))
            {
                int selStart = MainTextBox.SelectionStart;
                MainTextBox.SelectedText = replaceText;
                MainTextBox.SelectionStart = selStart + replaceText.Length;
                MainTextBox.SelectionLength = 0;
            }
            FindNext_Click(sender, e);
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextBox.Text;
            string replaceText = ReplaceTextBox.Text;
            if (string.IsNullOrEmpty(searchText)) return;
            string original = MainTextBox.Text;
            int count = 0;
            var sb = new StringBuilder();
            int idx = 0;
            while (idx <= original.Length)
            {
                int found = original.IndexOf(searchText, idx, StringComparison.OrdinalIgnoreCase);
                if (found < 0) { sb.Append(original, idx, original.Length - idx); break; }
                sb.Append(original, idx, found - idx);
                sb.Append(replaceText);
                idx = found + searchText.Length;
                count++;
            }
            if (count > 0)
            {
                MainTextBox.Text = sb.ToString();
                SearchStatusText.Text = $"{count}개 바꿨습니다";
            }
            else
            {
                SearchStatusText.Text = "찾을 수 없음";
            }
        }

        private void CloseSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.Visibility = Visibility.Collapsed;
            MainTextBox.Focus();
        }

        // ── [F8] 마크다운 미리보기 ───────────────────────────────────
        private void ToggleMarkdownPreviewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleMarkdownPreview();
            e.Handled = true;
        }

        private void MdPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            bool wantOn = MdPreviewButton.IsChecked == true;
            if (wantOn != _isMarkdownPreviewMode)
                ToggleMarkdownPreview();
        }

        private void ToggleMarkdownPreview()
        {
            _isMarkdownPreviewMode = !_isMarkdownPreviewMode;
            MdPreviewButton.IsChecked = _isMarkdownPreviewMode;

            if (_isMarkdownPreviewMode)
            {
                SplitterColumn.Width = new GridLength(4);
                PreviewColumn.Width = new GridLength(1, GridUnitType.Star);
                PreviewSplitter.Visibility = Visibility.Visible;
                MarkdownPreviewBrowser.Visibility = Visibility.Visible;

                if (_markdownUpdateTimer == null)
                {
                    _markdownUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                    _markdownUpdateTimer.Tick += MarkdownUpdateTimer_Tick;
                }
                UpdateMarkdownPreview();
            }
            else
            {
                SplitterColumn.Width = new GridLength(0);
                PreviewColumn.Width = new GridLength(0);
                PreviewSplitter.Visibility = Visibility.Collapsed;
                MarkdownPreviewBrowser.Visibility = Visibility.Collapsed;
                _markdownUpdateTimer?.Stop();
            }
        }

        private void MarkdownUpdateTimer_Tick(object? sender, EventArgs e)
        {
            _markdownUpdateTimer?.Stop();
            UpdateMarkdownPreview();
        }

        private void UpdateMarkdownPreview()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string html = Markdown.ToHtml(MainTextBox.Text, pipeline);

            bool dark = isDarkMode;
            string bg     = dark ? "#1e1e1e" : "#ffffff";
            string fg     = dark ? "#d4d4d4" : "#333333";
            string codeBg = dark ? "#2d2d2d" : "#f4f4f4";
            string border = dark ? "#444444" : "#dddddd";

            string fullHtml = $@"<!DOCTYPE html>
<html><head>
<meta charset=""utf-8"">
<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
<style>
  body{{font-family:'Segoe UI',Tahoma,sans-serif;padding:16px 20px;line-height:1.7;background:{bg};color:{fg};margin:0;font-size:14px}}
  h1,h2,h3,h4,h5,h6{{border-bottom:1px solid {border};padding-bottom:.3em;margin-top:1.2em;margin-bottom:.5em}}
  code{{background:{codeBg};padding:2px 5px;border-radius:3px;font-family:Consolas,'Courier New',monospace;font-size:.88em}}
  pre{{background:{codeBg};padding:12px 16px;border-radius:5px;overflow-x:auto;margin:1em 0}}
  pre code{{padding:0;background:none;font-size:.9em}}
  blockquote{{border-left:4px solid {border};margin:1em 0;padding:.5em 1em;color:#888;background:{codeBg};border-radius:0 4px 4px 0}}
  table{{border-collapse:collapse;width:100%;margin:1em 0}}
  th,td{{border:1px solid {border};padding:6px 12px;text-align:left}}
  th{{background:{codeBg};font-weight:600}}
  tr:nth-child(even){{background:{(dark ? "#252525" : "#f9f9f9")}}}
  a{{color:{(dark ? "#6aa9f4" : "#0066cc")}}}
  img{{max-width:100%;height:auto}}
  hr{{border:none;border-top:1px solid {border};margin:1.5em 0}}
  ul,ol{{padding-left:1.5em}}
  li{{margin:.2em 0}}
</style>
</head><body>{html}</body></html>";

            MarkdownPreviewBrowser.NavigateToString(fullHtml);
        }
    }
}
