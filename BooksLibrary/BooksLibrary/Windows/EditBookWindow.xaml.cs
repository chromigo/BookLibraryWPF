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
    /// Логика взаимодействия для EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        BooksSet newBook;
        public EditBookWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //проверка были ли изменения?

            //Проверяем минимальную заполненость текстовых полей для сохранения данных
            if (NameTextBox.Text == string.Empty || AuthorTextBox.Text == string.Empty)
            {
                MessageBox.Show("Необходимо заполнить хотя бы название и автора книги");
            }
            else
            {
                //отдельно парсим год, т.к. он хранится не строкой, а int
                Nullable<int> year;
                //if (int.TryParse(YearTextBox.Text, out year) == false && YearTextBox.Text!=string.Empty)
                try
                {
                    if (YearTextBox.Text != string.Empty)
                        year = int.Parse(YearTextBox.Text);
                    else
                        year = null;
                }
                catch (Exception)
                {
                    MessageBox.Show("Введите год корректно(например: 1865)");
                    return;
                }

                BooksSet book = new BooksSet
                {
                    Id = newBook.Id,
                    Title = NameTextBox.Text,
                    Author = AuthorTextBox.Text,
                    Publisher = PublisherTextBox.Text,
                    Year = year,
                    Note = NoteTextBox.Text
                };

                //Сохраняем данные для передачи
                Application.Current.Properties["editBook"] = book;
                DialogResult = true;
                this.Close();
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //При загрузке страницы заполняем поля значениями из редактируемого объекта
            newBook = Application.Current.Properties["editBook"] as BooksSet;

            NameTextBox.Text = newBook.Title;
            AuthorTextBox.Text = newBook.Author;
            PublisherTextBox.Text = newBook.Publisher;
            YearTextBox.Text = newBook.Year.ToString();
            NoteTextBox.Text = newBook.Note;
        }
    }
}
