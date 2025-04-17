// See https://aka.ms/new-console-template for more information
using lab2;

namespace lab2
{
    public struct Toy
    {
        private string name;
        private double cost;
        private int age1;
        private int age2;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        public int Age1
        {
            get { return age1; }
            set { age1 = value; }
        }
        public int Age2
        {
            get { return age2; }
            set { age2 = value; }
        }
        public Toy(string name, double cost, int ageFrom, int ageTo)
        {
            this.name = name;
            this.cost = cost;
            this.age1 = ageFrom;
            this.age2 = ageTo;
        }
        public string Print()
        {
            return $"{Name} (Цена: {Cost} руб., Возраст: {age1}-{age2} лет)";
        }
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("-----ЗАДАНИЕ 1:-----");
        InputValidator checker = new InputValidator();
        /*string filepath = checker.GetValidFilePath();
        Console.Write("minValue - ");
        int minValue = checker.GetValidWholeNumber();
        Console.Write("maxValue - ");
        int maxValue = checker.GetValidWholeNumber();
        Console.Write("count - ");
        int countfirst = checker.GetValidWholeNumber();
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        FilesAndCollection.FillWithRandomNumbers(countfirst, minValue, maxValue, filepath);
        Console.Write("b - ");
        int b = checker.GetValidWholeNumber();
        bool check = FilesAndCollection.ContainsNumber(b, filepath);
        Console.WriteLine($"Результат: {check}");
        
        Console.WriteLine("-----ЗАДАНИЕ 2:-----");
        Console.Write("minValue - ");
        int minValue2 = checker.GetValidWholeNumber();
        Console.Write("maxValue - ");
        int maxValue2 = checker.GetValidWholeNumber();
        Console.Write("count - ");
        int count2 = checker.GetValidWholeNumber();
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        FilesAndCollection.FillWithRandomNumbersFull(count2, minValue2, maxValue2, filepath);
        Console.Write("k - ");
        int K = checker.GetValidWholeNumber(); 
        int sum = FilesAndCollection.SumOfMultiples(filepath, K);
        Console.WriteLine($"Реультат: {sum}");
        
        Console.WriteLine("-----ЗАДАНИЕ 3:-----");
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        Console.Write("count - ");
        int count3 = checker.GetValidWholeNumber();
        FilesAndCollection.GenerateTextFile(count3, filepath);
        Console.Write("Filepathnew - ");
        string filepathnew = checker.GetValidFilePath();
        FilesAndCollection.CopyLinesWithoutDigits(filepath, filepathnew);
        Console.WriteLine($"Результат: Строки переписаны в {filepathnew} ");

        Console.WriteLine("-----ЗАДАНИЕ 4:-----");
        string filepathBIN = checker.GetValidFilePath();
        Console.Write("count - ");
        int count4 = checker.GetValidWholeNumber();
        Console.WriteLine("ГЕНЕРАЦИЯ БИНАРНОГО ДОКУМЕНТА...");
        FilesAndCollection.GenerateBinaryFile(filepathBIN, count4);
        Console.Write("Filepathnew - ");
        string filepathnew2 = checker.GetValidFilePath();
        FilesAndCollection.GenerateUniqueBinaryFile(filepathBIN, filepathnew2, count4);
        Console.WriteLine($"Результат: Числа переписаны в {filepathnew2} ");*/
        
        Console.WriteLine("-----ЗАДАНИЕ 5:-----");
        Toy[] toys = new Toy[]
        {
            new Toy("Конструктор", 1500, 5, 12),
            new Toy("Мяч", 500, 3, 10),
            new Toy("Кукла", 800, 2, 8),
            new Toy("Пазл", 450, 4, 6),
            new Toy("Машинка", 300, 2, 5),
            new Toy("Набор доктора", 1200, 3, 7)
        };
        string filepathXML = checker.GetValidFilePath();
        FilesAndCollection.Serialize(toys, filepathXML);
        Console.WriteLine("Массив игрушек успешно сохранен в XML файл.");
        Toy[] deserializedToys = FilesAndCollection.Deserialize(filepathXML);
        // Выполняем задание: получаем игрушки для 5-летних детей с ценой <= k
        Console.Write("Введите максимальную цену (k): ");
        double k = double.Parse(Console.ReadLine());
        List<string> suitableToys = new List<string>();
        foreach (Toy toy in deserializedToys)
        {
            if (toy.Cost <= k && toy.Age1 <= 5 && toy.Age2 >= 5)
            {
                suitableToys.Add(toy.Name);
            }
        }
        Console.WriteLine("\nПодходящие игрушки:");
        if (suitableToys.Count > 0)
        {
            foreach (string toyName in suitableToys)
            {
                Console.WriteLine(toyName);
            }
        }
        else
        {
            Console.WriteLine("Нет подходящих игрушек.");
        }

        /*Console.WriteLine("-----ЗАДАНИЕ 6:-----");
        List<string> userList = new List<string>();
        Console.WriteLine("Введите элементы списка (для завершения введите пустую строку):");
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;
            userList.Add(input);
        }
        Console.WriteLine("Введенный список:");
        Console.WriteLine(FilesAndCollection.PrintList(userList));
        Console.Write("Введите значение для удаления: ");
        string valueToRemove = Console.ReadLine();
        FilesAndCollection.RemoveAllOccurrences(valueToRemove, userList);
        Console.WriteLine("После удаления:");
        Console.WriteLine(FilesAndCollection.PrintList(userList));

        Console.WriteLine("-----ЗАДАНИЕ 7:-----");
        LinkedList<string> userList2 = new LinkedList<string>();
        Console.WriteLine("Введите элементы списка (для завершения введите пустую строку):");
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;
            userList2.AddLast(input);
        }
        Console.WriteLine("Введенный список:");
        Console.WriteLine(FilesAndCollection.PrintNewList(userList2));
        Console.Write("Введите элемент E: ");
        string E = Console.ReadLine();
        FilesAndCollection.ReverseBetweenFirstAndLast(E, userList2);
        Console.WriteLine("После обработки:");
        Console.WriteLine(FilesAndCollection.PrintNewList(userList2));

        Console.WriteLine("-----ЗАДАНИЕ 8:-----");
        Console.Write("Количество фильмов - ");
        int movieCount = checker.GetValidWholeNumber();
        var allMovies = new HashSet<string>();
        Console.WriteLine("Введите названия фильмов (каждое с новой строки):");
        for (int i = 0; i < movieCount; i++)
        {
            string movie = Console.ReadLine();
            allMovies.Add(movie);
        }
        Console.Write("Количество зрителей - ");
        int viewerCount = checker.GetValidWholeNumber();
        var viewers = new List<HashSet<string>>();
        for (int i = 0; i < viewerCount; i++)
        {
            Console.WriteLine($"\nЗритель {i + 1}:");
            Console.Write("Сколько фильмов посмотрел этот зритель?");
            int viewedCount = checker.GetValidWholeNumber();
            var viewerMovies = new HashSet<string>();
            Console.WriteLine("Введите названия просмотренных фильмов (каждое с новой строки):");
            for (int j = 0; j < viewedCount; j++)
            {
                string movie = Console.ReadLine();
                if (!allMovies.Contains(movie))
                {
                    Console.WriteLine($"Ошибка: фильм '{movie}' не существует в общем списке.");
                }
                viewerMovies.Add(movie);
            }
            viewers.Add(viewerMovies);
        }
        FilesAndCollection.AnalyzeMovies(allMovies, viewers);

        Console.WriteLine("-----ЗАДАНИЕ 9:-----");
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        FilesAndCollection.GenerateTextFileFOR9(filepath);
        FilesAndCollection.ProcessFile(filepath);

        Console.WriteLine("-----ЗАДАНИЕ 10:-----");
        Console.Write("Количество сотрудников - ");
        int count = checker.GetValidWholeNumber();
        StreamWriter f = new StreamWriter(filepath);
        for (int i = 0; i < count; i++)
        {
            Console.Write($"Ввод данных сотрудника {i + 1}:");
            Console.Write(" Фамилия (до 20 символов): ");
            string lastName = checker.GetValidSubname();
            Console.Write("Инициалы (формат X.X., до 4 символов): ");
            string initials = checker.GetValidInicial();
            Console.Write("Телефон (формат XXX-XX-XX): ");
            string phone = checker.GetValidFormattedNumber();
            f.WriteLine($"{lastName} {initials} {phone}");
        }
        Console.WriteLine($"Данные успешно сохранены в файл {filepath}");
        f.Close();
        double N = FilesAndCollection.CalculateAverageEmployeesPerDepartment(filepath);
        Console.WriteLine(N);*/
    }
}
///Users/stepanivanov/Documents/project/test/test.txt
///Users/stepanivanov/Documents/project/test/test1.txt


