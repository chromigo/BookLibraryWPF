using BooksLibrary.Windows;
using MicroMvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BooksLibrary
{
    class LibraryViewModel
    {
        #region Members
        static ObservableCollection<BooksViewModel> _books = new ObservableCollection<BooksViewModel>();
        private readonly object _booksTypesLock = new object();
        #endregion

        #region Properties
        public static ObservableCollection<BooksViewModel> Books
        {
            get
            {
                return _books;
            }
            set
            {
                _books = value;
            }
        }
        #endregion

        //объект для блокировки (см. BindingOperations.EnableCollectionSynchronization in .net 4.5)
        //The lock object is nothing special, just an instance of the CLR object type.
        //This single method makes cross-thread collection change notification possible.
        private object _booksLock = new object();

        #region Construction
        public LibraryViewModel()
        {
            Books = new ObservableCollection<BooksViewModel>();
            //позволяет корректно изменять данные и отображать в фоновом потоке.(в данном случае для асинхронной загрузки данных вначале)
            BindingOperations.EnableCollectionSynchronization(Books, _booksLock);
            //сам вызов делаю в MainWindows.xaml.cs
        }
        #endregion

        #region Commands
        #region DeleteCommand

        public ICommand DeleteBook { get { return new RelayCommand<BooksViewModel>(DeleteBookExecute, CanDeleteBookExecute);} }

        void DeleteBookExecute(BooksViewModel bookVM)
        {
            if (_books == null)
                return;
            try
            {
                //Удаляем из бд
                DeleteBookDB(bookVM.Book);
                //затем из ObservableCollection
                Books.Remove(bookVM);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось удалить данные. \r\nОшибка: " + exc.Message);
            }

        }
        /// <summary>
        /// Удаляет книгу из БД
        /// </summary>
        /// <param name="bookVM">Удаляемая книга</param>
        private static void DeleteBookDB(BooksSet book)
        {
            using (SimpleLibDBEntities context = new SimpleLibDBEntities())
            {
                BooksSet bookToDelete = context.BooksSet.First(b => b.Id == book.Id);
                context.BooksSet.Remove(bookToDelete);
                context.SaveChanges();
            }
        }

        bool CanDeleteBookExecute(BooksViewModel param)
        {
            return true;
        }
        #endregion

        #region EditCommand

        public ICommand EditBook { get { return new RelayCommand<BooksViewModel>(EditBookExecute,CanEditBookExecute); } }

        private void EditBookExecute(BooksViewModel bookVM)
        {
            if (_books == null)
                return;

            //записываем нашу сущность в память
            Application.Current.Properties.Add("editBook", bookVM.Book);

            try
            {
                //вызываем окно редактирования
                EditBookWindow edWindow = new EditBookWindow();
                Nullable<bool> result = edWindow.ShowDialog();

                if (result == true)
                {
                    //Получаем нашу отредактированную сущность
                    BooksSet newBook = Application.Current.Properties["editBook"] as BooksSet;

                    //Сохраняем изменения в БД
                    UpdateDB(newBook);

                    //Сохраняем в набюдаемой коллекции
                    bookVM.Book = newBook;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось изменить данные. \r\nОшибка: " + exc.Message);
            }
            finally
            {
                Application.Current.Properties.Remove("editBook");
            }
        }

        /// <summary>
        /// Обновняет информацию по существущей книге в нашей БД
        /// </summary>
        /// <param name="newBook">Книга с новыми данными(но старым id)</param>
        private static void UpdateDB(BooksSet newBook)
        {
            using (SimpleLibDBEntities context = new SimpleLibDBEntities())
            {
                //Получаем объект и напрямую меняем его св-ва(т.к. он ссылочный и изменения напрямую отразятся в бд)
                BooksSet bookToEdit = context.BooksSet.First(b => b.Id == newBook.Id);
                bookToEdit.Title = newBook.Title;
                bookToEdit.Publisher = newBook.Publisher;
                bookToEdit.Author = newBook.Author;
                bookToEdit.Year = newBook.Year;
                bookToEdit.Note = newBook.Note;

                context.SaveChanges();
            }
        }

        private bool CanEditBookExecute(BooksViewModel book)
        {
            return true;
        }
        #endregion

        #region AddCommand
        /// <summary>
        /// Коммадна для добавления книги
        /// </summary>
        public ICommand AddBook { get { return new RelayCommand(AddBookExecute, CanAddBookExecute); } }
        /// <summary>
        /// Добавляет новую книгу в библиотеку
        /// </summary>
        void AddBookExecute()
        {
            if (_books == null)
                return;

            try
            {
                //Создаем новое окно, где вводим данные новой книги
                AddBookWindow abWindow = new AddBookWindow();
                Nullable<bool> result = abWindow.ShowDialog();
                if (result == true)
                {
                    //Получаем созданную книгу
                    BooksSet newBook = Application.Current.Properties["newBook"] as BooksSet;
                    //Если все правильно сделано, то индексы в БД и наблюдаемой коллекции не должны расходиться.
                    newBook.Id = Books.Count;

                    //Сохраняем в БД
                    AddBookDB(newBook);
                    //Сохраняем в наблюдаемой коллекции
                    Books.Add(new BooksViewModel { Book = newBook });
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось добавить данные. \r\nОшибка: " + exc.Message);
            }
            finally
            {
                Application.Current.Properties.Remove("newBook");
            }

        }
        /// <summary>
        /// Сохраняет заданную книгу в нашей базе данных
        /// </summary>
        /// <param name="newBook">Сохраняемая книга</param>
        private static void AddBookDB(BooksSet newBook)
        {
            using (SimpleLibDBEntities context = new SimpleLibDBEntities())
            {
                //newBook.Id = context.BooksSet.Count();
                context.BooksSet.Add(newBook);
                context.SaveChanges();
            }
            //return newBook;
        }

        bool CanAddBookExecute()
        {
            return true;
        }
        #endregion

        #region ClearCommand
        public ICommand ClearBook { get { return new RelayCommand(ClearBookExecute, CanClearBookExecute); } }

        private void ClearBookExecute()
        {
            //просто очищаем ObservableCollection
            Books.Clear();
        }

        private bool CanClearBookExecute()
        {
            return true;
        }
        #endregion

        #endregion
    }
}
