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

namespace BooksLibrary.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text == string.Empty || AuthorTextBox.Text == string.Empty)
            {
                MessageBox.Show("Необходимо заполнить хотя бы название и автора книги");
            }
            else
            {
                int year;
                int.TryParse(YearTextBox.Text,out year);

                BooksSet book = new BooksSet
                {
                    Title = NameTextBox.Text,
                    Author = AuthorTextBox.Text,
                    Publisher = PublisherTextBox.Text,
                    Year = year,
                    Note = NoteTextBox.Text
                };

                Application.Current.Properties.Add("newBook", book);
                DialogResult = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
