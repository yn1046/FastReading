using EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FastReading
{
    /// <summary>
    /// Логика взаимодействия для Read.xaml
    /// </summary>
    public partial class Read : Window
    {
        public User CurrentUser { get; set; }
        public string CurrentText { get; set; }
        public DispatcherTimer Timer { get; set; }
        public List<string> Words { get; set; }
        public int CurrentWord { get; set; }
        public int MaxLength { get; set; }
        public DateTime StartTime { get; set; }

        public Read(User user, Book book, string Text)
        {
            InitializeComponent();
            CurrentText = Text;
            GetWords();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            wordBox.Content = Words[0];
            CurrentWord = 0;
        }

        private void GetWords()
        {
            Words = CurrentText.Split(new char[] { ' ', '\n' }).ToList();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (CurrentWord + 1 < Words.Count)
            {
                if (maxLengthCheckBox.IsChecked != true)
                {
                    wordBox.Content = Words[CurrentWord + 1];
                    CurrentWord++;
                }
                else
                {
                    if (Words[CurrentWord + 1].Length > MaxLength)
                    {
                        wordBox.Content = Words[CurrentWord + 1].Substring(0, MaxLength);
                        Words[CurrentWord + 1] = Words[CurrentWord + 1].Substring(MaxLength, Words[CurrentWord + 1].Length - MaxLength);
                    }
                    else
                    {
                        var str = Words[CurrentWord + 1];

                        while (str.Length < MaxLength)
                        {
                            if (CurrentWord + 2 < Words.Count)
                            {
                                if ((str + ' ' + Words[CurrentWord + 2]).Length <= MaxLength)
                                {
                                    str += ' ' + Words[CurrentWord + 2];
                                    CurrentWord++;
                                }
                                else break;
                            }
                            else break;
                        }
                        wordBox.Content = str;
                        CurrentWord++;
                    }
                }
            }
            else
            {
                Timer.Stop();
                var str = $"Чтение завершено! Затрачено {(DateTime.Now - StartTime).Seconds} с.";
                if (MessageBox.Show(str, "Конец чтения", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    var mainWindow = new MainWindow(CurrentUser);
                    mainWindow.Show();
                    Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                Timer.Start();
                if (StartTime == null) StartTime = DateTime.Now;
                playButton.Content = "Стоп";
            }
            else
            {
                Timer.Stop();
                playButton.Content = "Старт";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            wordBox.FontSize = Convert.ToInt32(selectedItem.Content);
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            wordBox.FontFamily = new FontFamily(selectedItem.Content.ToString());
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            var interval = 1000 / Convert.ToInt32(selectedItem.Content);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
        }

        private void ComboBox_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;

            maxLengthCheckBox.IsEnabled = true;

            MaxLength = Convert.ToInt32(selectedItem.Content);
        }
    }
}
