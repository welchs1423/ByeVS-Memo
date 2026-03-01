using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ByeVS_Memo;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
}