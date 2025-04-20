using lab3;
namespace lab3
{
    public struct BookCatalog
    {
        private int id;
        private string title;
        private string author;
        private int year;
        private int price;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public BookCatalog(int id, string title, string author, int year, int price)
        {
            this.id = id;
            this.title = title;
            this.author = author;
            this.year = year;
            this.price = price;
        }
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        int numberOperation;
        int count = 20;
        Chekcer checker = new Chekcer();
        string filePath = checker.GetValidFilePath();
        List<BookCatalog> catalog = new List<BookCatalog>();
        BooksOperation booksOperation = new BooksOperation(filePath, catalog);
        booksOperation.Generate(count);
        booksOperation.Serialize();
        while (true)
        {
            Console.WriteLine("\nВыберите действие с файлом:");
            Console.WriteLine("1. Загрузить данные из файла (десериализовать)");
            Console.WriteLine("2. Просмотреть все книги");
            Console.WriteLine("3. Добавить новую книгу");
            Console.WriteLine("4. Удалить книгу по ID");
            Console.WriteLine("5. Найти книги по автору");
            Console.WriteLine("6. Найти книги, изданные после указанного года");
            Console.WriteLine("7. Показать среднюю цену книг");
            Console.WriteLine("8. Найти самую дорогую книгу автора");
            Console.WriteLine("9. Выйти из программы");
            Console.Write("Ваш выбор: ");
            numberOperation = checker.GetValidWholeNumber();
            switch (numberOperation)
            {
                case 1:
                {
                    booksOperation.Deserialize();
                    Console.WriteLine("\nДанные успешно загружены из файла!");
                    break;
                }
                case 2:
                {
                    booksOperation.Deserialize();
                    Console.WriteLine(booksOperation);
                    break;
                }
                case 3:
                {
                    booksOperation.Deserialize();
                    Console.WriteLine("Введите данные новой книги:");
                    Console.Write("Название: ");
                    string title = Console.ReadLine();
                    Console.Write("Автор: ");
                    string author = Console.ReadLine();
                    Console.Write("Год издания: ");
                    int year = checker.GetValidWholeNumber();
                    Console.Write("Цена: ");
                    int price = checker.GetValidWholeNumber();
                    booksOperation.AddNewBookInteractive(count, title, author, year, price);
                    booksOperation.Serialize();
                    Console.Write("\nКнига добавлена в каталог!");
                    break;
                }
                case 4:
                {
                    booksOperation.Deserialize();
                    Console.Write("\nВведите id: ");
                    int id = checker.GetValidWholeNumber();
                    try
                    {
                        booksOperation.removeFromID(id);
                        booksOperation.Serialize();
                        Console.WriteLine("\nКнига удалена из каталога!");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("\nОшибка: книги с таким ID не существует!");
                    }
                    break;
                }
                case 5:
                {
                    booksOperation.Deserialize();
                    Console.Write("Введите имя автора: ");
                    string author = Console.ReadLine();
                    var books = booksOperation.GetBooksByAuthor(author);
                    Console.WriteLine($"\nКниги автора {author}:");
                    foreach (var book in books) 
                    {
                        Console.WriteLine($"{book.Title} ({book.Year}), {book.Price} руб.");
                    }
                    break;
                }
                case 6:
                {
                    booksOperation.Deserialize();
                    Console.Write("Введите год: ");
                    int year = checker.GetValidWholeNumber();
                    var books = booksOperation.GetBooksPublishedAfterYear(year);
                    Console.WriteLine($"\nКниги, изданные после {year} года:");
                    foreach (var book in books)
                    {
                        Console.WriteLine($"{book.Title} ({book.Year}), {book.Price} руб.");
                    }
                    break;
                }
                case 7:
                {
                    booksOperation.Deserialize();
                    double avgPrice = booksOperation.GetAverageBookPrice();
                    Console.WriteLine($"\nСредняя цена книг: {avgPrice:F2} руб.");
                    break;
                }
                case 8:
                {
                    booksOperation.Deserialize();
                    Console.Write("Введите имя автора: ");
                    string author = Console.ReadLine();
                    var book = booksOperation.GetMostExpensiveBookByAuthor(author);
                    if (book != null)
                    {
                        Console.WriteLine($"\nСамая дорогая книга автора {author}:");
                        Console.WriteLine($"{book.Value.Title} ({book.Value.Year}), {book.Value.Price} руб.");
                    }
                    else
                    {
                        Console.WriteLine($"\nКниги автора {author} не найдены.");
                    }
                    break;
                }
                case 9:
                {
                    Console.WriteLine("Выход из программы...");
                    return;
                }
                default:
                {
                    Console.WriteLine("Неверный номер операции!");
                    break;    
                }
            }
        }
    }
}

// /Users/stepanivanov/Documents/project/test/test.xml