using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Threading;

namespace BooksLibrary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        async private void MainContext_Loaded(object sender, RoutedEventArgs e)
        {
            //общий размер коллекции
            int totalsize = 0;
            int step = 0;
            //Определяем шаг, чтобы корректно заполнять прогресс бар(от 0 до 100)
            using (SimpleLibDBEntities database = new SimpleLibDBEntities())
            {
                totalsize = database.BooksSet.Count();
                step = totalsize/100;
            }
            LoadLibraryProgressBar.Visibility = Visibility.Visible;
            //запускаем асинхронно задачу
            await LoadDataFromDBandProgressBarStep(step);
            MessageBox.Show("Данные были успешно загружены. \r\n Всего записей: "+totalsize);
            LoadLibraryProgressBar.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Загружает данные из бд и отображает процесс загрузки в progrss bar
        /// </summary>
        /// <param name="step">Шаг progrss bar</param>
        /// <returns></returns>
        private async Task LoadDataFromDBandProgressBarStep(int step)
        {
            await Task.Run(() =>
            {
                using (SimpleLibDBEntities database = new SimpleLibDBEntities())
                {
                    int count = 0;
                    var qw = from item in database.BooksSet select item;
                    
                    foreach (var item in qw)
                    {
                        count++;
                        //раскоментировать, чтобы лучше увидеть эффект заполнения
                        //Thread.Sleep(1);
                        LibraryViewModel.Books.Add(new BooksViewModel { Book = item });

                        //Для работы прогресс бара.
                        if (count == step)
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                             DispatcherPriority.Normal, new Action(() =>
                             {
                                 LoadLibraryProgressBar.Value++;
                             }));
                            count = 0;
                        }
                    }
                }
                
            });
        }

        private void TextBlock_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

    }
}
