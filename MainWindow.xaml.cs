using System;
using System.IO;
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
        // 현재 다크 모드인지 기억하는 스위치 역할
        private bool isDarkMode = false;
        private readonly DispatcherTimer _clockTimer;
        private readonly DispatcherTimer _autoSaveTimer;

        // 시스템 트레이 아이콘
        private System.Windows.Forms.NotifyIcon _notifyIcon = null!;

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();

            // 설정 파일이 존재하는지 확인
            if (File.Exists("theme_setting.txt"))
            {
                // 파일에 적힌 글자를 읽어옵니다 ("Dark" 또는 "Light")
                string savedTheme = File.ReadAllText("theme_setting.txt");

                if (savedTheme == "Dark")
                {
                    isDarkMode = true; // ★수정됨: 스위치도 다크모드로 확실히 켜줍니다!

                    MainGrid.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    MainTextBox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));    // VS Code 테마 느낌의 어두운 회색
                    MainTextBox.Foreground = new SolidColorBrush(Colors.White); // 글씨는 하얗게
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
                    isDarkMode = false; // ★수정됨: 스위치 동기화

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
            }

            RefreshRecentFilesMenu();

            // 시계 타이머 (1초 간격)
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();

            // 자동 저장 타이머 (1분 간격)
            _autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;
            _autoSaveTimer.Start();

            // 초기 시간 표시
            ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // [새 문서] Ctrl+N
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainTextBox.Text = string.Empty;
        }

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

        // [자동 줄바꿈] 체크박스 토글
        private void WordWrapCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            MainTextBox.TextWrapping = WordWrapCheckBox.IsChecked == true
                ? TextWrapping.Wrap
                : TextWrapping.NoWrap;
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
                File.WriteAllText(saveFileDialog.FileName, MainTextBox.Text);
                RecentFilesStore.Add(saveFileDialog.FileName);
                RefreshRecentFilesMenu();
                MessageBox.Show("저장이 완료되었습니다!", "알림");
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
            if (WindowState == WindowState.Minimized)
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
    }
} // ★수정됨: 네임스페이스 닫는 중괄호