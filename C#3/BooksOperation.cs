using System.Xml.Serialization;
using System.Text;
namespace lab3;

public class BooksOperation
{
    private string filePath;
    private List<BookCatalog> books;
    public BooksOperation(string filePath, List<BookCatalog> books)
    {
        this.filePath = filePath;
        this.books = books;
    }
    public void Generate(int count)
    {
        Random random = new Random();
        string[] titles = {
            "Колобок", "Теремок", "Репка", "Курочка Ряба", "Маша и Медведь",
            "Айболит", "Мойдодыр", "Тараканище", "Федорино горе", "Телефон",
            "Доктор Айболит", "Буратино", "Золотой ключик", "Незнайка на Луне",
            "Винни-Пух и все-все-все", "Карлсон, который живёт на крыше", 
            "Денискины рассказы", "Приключения Чиполлино", "Снежная королева",
            "Гадкий утёнок", "Дюймовочка", "Русалочка", "Кот в сапогах", 
            "Красная Шапочка", "Золушка", "Бременские музыканты", "Пеппи Длинныйчулок"
        };
        string[] authors =
        {
            "Корней Чуковский", "Самуил Маршак", "Агния Барто", "Сергей Михалков",
            "Николай Носов", "Александр Пушкин", "Ганс Христиан Андерсен",
            "Шарль Перро", "Братья Гримм", "Астрид Линдгрен", "Эдуард Успенский",
            "Григорий Остер", "Виктор Драгунский", "Джанни Родари", "Алан Милн"
        };
        for (int i = 0; i < count; i++)
        {
            int id = i + 1;
            string title = titles[random.Next(titles.Length)];
            string author = authors[random.Next(authors.Length)];
            int year = random.Next(2000, 2024);
            int price = random.Next(100, 1000);
            books.Add(new BookCatalog(id, title, author, year, price));
        }
    }
    public void AddNewBookInteractive(int count, string title, string author, int year, int price)
    {
        BookCatalog newBook = new BookCatalog(count + 1, title, author, year, price);
        books.Add(newBook);
    }
    public void removeFromID(int id)
    {
        books.RemoveAt(id - 1);
        for (int i = id - 1; i < books.Count; i++)
        {
            BookCatalog book = books[i];
            book.Id = i + 1;
            books[i] = book;
        }
    }
    public void Serialize()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<BookCatalog>));
        using (StreamWriter writer = new StreamWriter(filePath))
        { 
            serializer.Serialize(writer, books);
        }
    }
    public void Deserialize()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<BookCatalog>));
        using (StreamReader reader = new StreamReader(filePath))
        {
            books = (List<BookCatalog>)serializer.Deserialize(reader);
        }
    }
    public List<BookCatalog> GetBooksByAuthor(string author)
    {
        return books.Where(book => book.Author.Equals(author, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public List<BookCatalog> GetBooksPublishedAfterYear(int year)
    {
        return books.Where(book => book.Year > year).OrderBy(book => book.Year).ToList();
    }
    
    public double GetAverageBookPrice()
    {
        return books.Average(book => book.Price);
    }
    public BookCatalog? GetMostExpensiveBookByAuthor(string author)
    {
        return books.Where(book => book.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(book => book.Price)
            .FirstOrDefault();
    }
    public override string ToString()
    {
        if (books == null || books.Count == 0)
        {
            return "Каталог книг пуст.";
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== КАТАЛОГ КНИГ ===");
        sb.AppendLine($"{"ID",-5} {"Название",-30} {"Автор",-25} {"Год",-6} {"Цена",-6}");
        sb.AppendLine(new string('-', 75));
        foreach (var book in books)
        {
            sb.AppendLine($"{book.Id,-5} {book.Title,-30} {book.Author,-25} {book.Year,-6} {book.Price,-6} руб.");
        }
        sb.AppendLine($"\nВсего книг: {books.Count}");
        return sb.ToString();
    }
}