using MicroMvvm;
namespace BooksLibrary
{
    public class BooksViewModel : ObservableObject
    {
        #region Construction
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public BooksViewModel()
        {
            _booksSet = new BooksSet {Id=1, Title = "DTitle", Author = "DAuthor", Publisher = "DPublisher", Year = 1000, Note = "DNote" };
        }
        #endregion

        #region Members
        BooksSet _booksSet;
        #endregion

        #region Properties
        //Поля, которые мы хотим отображать во View
        public BooksSet Book
        {
            get
            {
                return _booksSet;
            }
            set
            {
                _booksSet = value;
                //RaisePropertyChanged("Book");
                //Сделал так, все работает. Но, возможно, это не совсем корректно?
                RaisePropertyChanged("Title");
                RaisePropertyChanged("Author");
            }
        }

        public string Title
        {
            get { return Book.Title; }
            set
            {
                if (Book.Title != value)
                {
                    Book.Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public string Author
        {
            get { return Book.Author; }
            set
            {
                if (Book.Author != value)
                {
                    Book.Author = value;
                    RaisePropertyChanged("Author");
                }
            }
        }

        #endregion
    }
}
