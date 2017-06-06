using EF;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FastReading
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; }

        public List<Book> BooksList { get; set; }
        public Book SelectedBook { get; set; }
        public string Text { get; set; }

        public MainWindow(User u)
        {
            CurrentUser = u;
            InitializeComponent();
            BooksList = new List<Book>();
            SelectedBook = new Book();
            LoadComboboxItems();
        }

        private void LoadComboboxItems()
        {
            using (var db = new Context())
            {
                BooksList = db.Books.ToList();
            }
            booksComboBox.ItemsSource = BooksList;
            booksComboBox.DisplayMemberPath = "Title";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedBook.Title))
            {
                Read read = new Read(CurrentUser, SelectedBook, Text);
                read.Show();
                this.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = booksComboBox.SelectedItem as Book;
            SelectedBook = selectedBook;
            Text = File.ReadAllText(SelectedBook.TextPath, Encoding.GetEncoding(1252));
            var description = SelectedBook.Author + "\n" + SelectedBook.Title + "\n" + Text;
            bookDescription.Text = description;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var logInWindow = new LogInWindow();
            logInWindow.Show();
            this.Close();
        }
    }
}
