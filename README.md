BookLibraryWPF
==============

WPF(MVVM)+EF application on .net 4.5 

Приложение позволяет пользователю управлять своей домашней билблиотекой из книг.
Пользователю доступен функционал по добавлению, удалению и редакторованию книг.
При запуске приложения происходит подгрузка из бд книг в фоновом потоке, статус загрузки отображается в прогресс баре.

Технологии:
.net 4.5
WPF
EF
MVVM pattern

(конкретно в .net 4.5 использовал BindingOperations.EnableCollectionSynchronization для обновления коллекций, на которые есть байдинг из контролов UI)
(MVVM - http://www.codeproject.com/Articles/165368/WPF-MVVM-Quick-Start-Tutorial )
