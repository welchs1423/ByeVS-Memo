using System.Globalization;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace ByeVS_Memo
{
    public partial class MainWindow : Window
    {


        // ...기존 코드 계속...
    }
}

// (클래스/네임스페이스 선언 바깥의 잘못된 코드 완전 삭제)
            }
        }

        // [통계] 버튼
        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MainTextBox.Text;

            // 단어 수 (공백·탭·줄바꿈 기준)
            int wordCount = string.IsNullOrWhiteSpace(text)
                ? 0
                : text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            // 바이트 크기 (UTF-8: 영문 1B, 한글 3B)
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(text);

            // 줄 수 (논리 줄 기준)
            int lineCount = text.Length == 0 ? 1 : text.Split('\n').Length;

            MessageBox.Show(
                $"단어 수: {wordCount}\n바이트 크기 (UTF-8): {byteCount} Bytes\n줄 수: {lineCount}",
                "문서 통계",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

                        // 포스트잇 모드 상태
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

                        // MainTextBox 드래그 이동용
                        private bool _isDraggingSticky = false;
                        private Point _dragStartPoint;
        // [열기] Ctrl+O
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenButton_Click(sender, e);
        }

        // [저장] Ctrl+S
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
        }

        // 포스트잇 모드 토글 (F12/버튼)
        private void ToggleStickyNoteModeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleStickyNoteMode();
        }

        private void StickyNoteButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleStickyNoteMode();
        }

        private void ToggleStickyNoteMode()

                                // ── MainTextBox 컨텍스트 메뉴 핸들러 ─────────────────────────────
                                private void MenuItem_Uppercase_Click(object sender, RoutedEventArgs e)
                                {
                                    ApplyTextTransform(s => s.ToUpper());
                                }

                                private void MenuItem_Lowercase_Click(object sender, RoutedEventArgs e)
                                {
                                    ApplyTextTransform(s => s.ToLower());
                                }

                                private void MenuItem_RemoveEmptyLines_Click(object sender, RoutedEventArgs e)
                                {
                                    ApplyTextTransform(s => string.Join("\n", s.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line))));
                                }

                                private void ApplyTextTransform(Func<string, string> transform)
                                {
                                    var tb = MainTextBox;
                                    if (tb == null) return;
                                    string result;
                                    if (tb.SelectionLength > 0)
                                    {
                                        int selStart = tb.SelectionStart;
                                        int selLen = tb.SelectionLength;
                                        string selected = tb.SelectedText;
                                        result = transform(selected);
                                        tb.Text = tb.Text.Remove(selStart, selLen).Insert(selStart, result);
                                        tb.Select(selStart, result.Length);
                                    }
                                    else
                                    {
                                        result = transform(tb.Text);
                                        tb.Text = result;
                                        tb.Select(0, tb.Text.Length);
                                    }
                                    // 텍스트 변경 이벤트 강제 발생 (상태바/테마 등 연동)
                                    MainTextBox_TextChanged(tb, null);
                                    MainTextBox_SelectionChanged(tb, null);
                                }
        {
            if (_isStickyNoteMode)
            {
                // 복구
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
                // 현재 상태 저장
                _prevStickyWindowStyle = WindowStyle;
                _prevStickyResizeMode = ResizeMode;
                _prevStickyWidth = Width;
                _prevStickyHeight = Height;
                _prevStickyLeft = Left;
                _prevStickyTop = Top;
                _prevTopPanelVisibility = TopPanelContainer.Visibility;
                _prevStatusBarVisibility = MainStatusBar.Visibility;

                // 포스트잇 모드 적용
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Width = 300;
                Height = 300;
                                // 화면 중앙 배치
                                Left = (SystemParameters.WorkArea.Width - 300) / 2 + SystemParameters.WorkArea.Left;
                                Top = (SystemParameters.WorkArea.Height - 300) / 2 + SystemParameters.WorkArea.Top;
                                TopPanelContainer.Visibility = Visibility.Collapsed;
                                MainStatusBar.Visibility = Visibility.Collapsed;
                                StickyNoteButton.Content = "🗒️ 일반모드";
                                MainTextBox.Cursor = Cursors.SizeAll;
                                _isStickyNoteMode = true;
                            }
                        }

                        // MainTextBox 드래그로 창 이동
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
                                try
                                {
                                    DragMove();
                                }
                                catch { }
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
        // [폰트 크기] Ctrl + 마우스 휠
        private void MainTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double newSize = MainTextBox.FontSize + (e.Delta > 0 ? 1 : -1);
                MainTextBox.FontSize = Math.Clamp(newSize, 8, 72);
                e.Handled = true;
            }
        }

        // ── 드래그 앤 드롭 파일 열기 ────────────────────────────────
        private void MainTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void MainTextBox_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                    return;

                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null || files.Length == 0)
                    return;

                // 여러 파일을 동시에 드롭해도 첫 번째 파일만 열기
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

        // [열기] 버튼 로직
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                MainTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                RecentFilesStore.Add(openFileDialog.FileName);
                RefreshRecentFilesMenu();
            }
        }

        // [저장] 버튼 로직
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, MainTextBox.Text);
                    // 안전 백업 시스템 호출
                    TryHistoryBackup(saveFileDialog.FileName);
                    RecentFilesStore.Add(saveFileDialog.FileName);
                    RefreshRecentFilesMenu();
                    MessageBox.Show("저장이 완료되었습니다!", "알림");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"저장 중 오류가 발생했습니다:\n{ex.Message}", "저장 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 안전 백업 시스템: 저장 시 백업본을 Backups 폴더에 타임스탬프 파일명으로 남김
        /// </summary>
        /// <param name="originalFilePath">저장하려는 원본 파일 경로</param>
        private void TryHistoryBackup(string originalFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalFilePath) || !File.Exists(originalFilePath))
                {
                    // '제목 없음' 등 임시/미저장 상태는 무시
                    return;
                }

                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string backupDir = Path.Combine(appDir, "Backups");
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
                string ext = Path.GetExtension(originalFilePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                string backupFileName = $"{fileName}_{timestamp}{ext}";
                string backupPath = Path.Combine(backupDir, backupFileName);
                File.Copy(originalFilePath, backupPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("백업 폴더 생성 또는 파일 복사 권한이 없습니다.\n관리자 권한으로 실행하거나 폴더 권한을 확인하세요.", "백업 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // 기타 예외는 조용히 무시(저장 자체는 성공해야 함)
                System.Diagnostics.Debug.WriteLine($"[HistoryBackup] {ex.Message}");
            }
        }

        private void RefreshRecentFilesMenu()
        {
            RecentFilesMenu.Items.Clear();
            foreach (string path in RecentFilesStore.Load())
            {
                var item = new MenuItem
                {
                    Header = Path.GetFileName(path),
                    ToolTip = path,
                    Tag = path,
                };
                item.Click += RecentFileMenuItem_Click;
                RecentFilesMenu.Items.Add(item);
            }

            if (RecentFilesMenu.Items.Count == 0)
            {
                RecentFilesMenu.Items.Add(new MenuItem { Header = "최근 파일 없음", IsEnabled = false });
            }
        }

        private void RecentFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem mi || mi.Tag is not string path)
                return;

            if (!File.Exists(path))
            {
                MessageBox.Show($"파일을 찾을 수 없습니다:\n{path}", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                RecentFilesStore.Remove(path);
                RefreshRecentFilesMenu();
                return;
            }

            MainTextBox.Text = File.ReadAllText(path);
            RecentFilesStore.Add(path);
            RefreshRecentFilesMenu();
        }

        // [다크 모드] 버튼 로직
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                // 다크 모드 색상 적용
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
                // 라이트 모드 (기존 색상) 복귀
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
            string currentTheme = isDarkMode ? "Dark" : "Light";
            File.WriteAllText("theme_setting.txt", currentTheme);
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

            openItem.Click += (s, e) => RestoreFromTray();
            exitItem.Click += (s, e) => System.Windows.Application.Current.Shutdown();

            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenuStrip = contextMenu;

            _notifyIcon.DoubleClick += (s, e) => RestoreFromTray();
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

        // ── 타임스탬프 삽입 (F5) ─────────────────────────────────────
        private void InsertTimestampCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string timestamp = $" [{DateTime.Now:yyyy-MM-dd HH:mm}] ";
            int caretIndex = MainTextBox.CaretIndex;
            MainTextBox.Text = MainTextBox.Text.Insert(caretIndex, timestamp);
            MainTextBox.CaretIndex = caretIndex + timestamp.Length;
            e.Handled = true;
        }

        // ── 몰입 모드 (F11 / Esc) ────────────────────────────────────
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFocusMode();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && _isFocusMode)
            {
                ToggleFocusMode();
                e.Handled = true;
            }
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
            _notifyIcon.Dispose();
            base.OnClosed(e);
        }

        // ── 항상 위로 고정 ───────────────────────────────────────────
        private void TopmostButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = TopmostButton.IsChecked == true;
        }

        // ── 상태 표시줄: 커서 위치 ───────────────────────────────────
        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateCursorPosition();
        }

        // ── 상태 표시줄: 글자 수 ─────────────────────────────────────
        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCharCount();
            UpdateLineNumbers();
        }

        private void UpdateCursorPosition()
        {
            int caretIndex = MainTextBox.CaretIndex;
            int lineIndex = MainTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            if (lineIndex < 0) return;
            int lineStartIndex = MainTextBox.GetCharacterIndexFromLineIndex(lineIndex);
            int col = caretIndex - lineStartIndex + 1;
            CursorPosText.Text = $"줄: {lineIndex + 1}, 칸: {col}";
        }

        private void UpdateCharCount()
        {
            CharCountText.Text = $"글자 수: {MainTextBox.Text.Length}";
        }

        // ── 줄 번호 업데이트 ─────────────────────────────────────────
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

        // ── 줄 번호 스크롤 동기화 ────────────────────────────────────
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

        // ── 찾기 및 바꾸기 ───────────────────────────────────────────
        // ── 줄 번호 영역 테마 ─────────────────────────────────────
        private void ApplyLineNumberTheme(bool dark)
        {
            if (dark)
            {
                LineNumberBorder.Background = new SolidColorBrush(Color.FromRgb(37, 37, 38));
                LineNumbersText.Foreground = new SolidColorBrush(Color.FromRgb(133, 133, 133));
            }
            else
            {
                LineNumberBorder.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
                LineNumbersText.Foreground = new SolidColorBrush(Color.FromRgb(119, 119, 119));
            }
        }

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
            Visibility replaceVis = showReplace ? Visibility.Visible : Visibility.Collapsed;
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
                if (e.Key == Key.F)
                {
                    ToggleSearchPanel(showReplace: false);
                    e.Handled = true;
                }
                else if (e.Key == Key.H)
                {
                    ToggleSearchPanel(showReplace: true);
                    e.Handled = true;
                }
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
                if (found < 0)
                {
                    sb.Append(original, idx, original.Length - idx);
                    break;
                }
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
    }
} // ★수정됨: 네임스페이스 닫는 중괄호