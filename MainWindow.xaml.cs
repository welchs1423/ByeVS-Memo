using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace ByeVS_Memo
{
    public partial class MainWindow : Window
    {
        // 현재 다크 모드인지 기억하는 스위치 역할
        private bool isDarkMode = false;

        public MainWindow()
        {
            InitializeComponent();

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
                    ThemeButton.Content = "라이트 모드";
                }
                else
                {
                    isDarkMode = false; // ★수정됨: 스위치 동기화

                    MainGrid.Background = new SolidColorBrush(Colors.White);
                    MainTextBox.Background = new SolidColorBrush(Colors.White);
                    MainTextBox.Foreground = new SolidColorBrush(Colors.Black);
                    TopPanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    ThemeButton.Content = "다크 모드";
                }
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
                MessageBox.Show("저장이 완료되었습니다!", "알림");
            }
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
                ThemeButton.Content = "라이트 모드";
            }
            else
            {
                // 라이트 모드 (기존 색상) 복귀
                MainGrid.Background = new SolidColorBrush(Colors.White);
                MainTextBox.Background = new SolidColorBrush(Colors.White);
                MainTextBox.Foreground = new SolidColorBrush(Colors.Black);
                TopPanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                ThemeButton.Content = "다크 모드";
            }

            string currentTheme = isDarkMode ? "Dark" : "Light";
            File.WriteAllText("theme_setting.txt", currentTheme);
        }
    }
} // ★수정됨: 네임스페이스 닫는 중괄호