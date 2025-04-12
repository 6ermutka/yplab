// See https://aka.ms/new-console-template for more information
using System.Collections;
using lab2;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("-----ЗАДАНИЕ 1:-----");
        InputValidator checker = new InputValidator();
        string filepath = checker.GetValidFilePath();
        /*Console.Write("minValue - ");
        int minValue = checker.GetValidWholeNumber();
        Console.Write("maxValue - ");
        int maxValue = checker.GetValidWholeNumber();
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        filesandcollection.FillWithRandomNumbers(10, minValue, maxValue, filepath);
        Console.Write("b - ");
        int b = checker.GetValidWholeNumber();
        bool check = filesandcollection.ContainsNumber(b, filepath);
        Console.WriteLine($"Результат: {check}");
        
        Console.WriteLine("-----ЗАДАНИЕ 2:-----");
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        filesandcollection.FillWithRandomNumbersFull(10, minValue, maxValue, filepath);
        Console.Write("k - ");
        int k = checker.GetValidWholeNumber(); 
        int sum = filesandcollection.SumOfMultiples(filepath, k);
        Console.WriteLine($"Реультат: {sum}");
        
        Console.WriteLine("-----ЗАДАНИЕ 3:-----");
        Console.WriteLine("ГЕНЕРАЦИЯ ТЕКСТОВОГО ДОКУМЕНТА...");
        filesandcollection.GenerateTextFile(100, filepath);
        string filepathnew = checker.GetValidFilePath();
        filesandcollection.CopyLinesWithoutDigits(filepath, filepathnew);*/
        
        /*Console.WriteLine("-----ЗАДАНИЕ 4:-----");
        Console.WriteLine("ГЕНЕРАЦИЯ БИНАРНОГО ДОКУМЕНТА...");
        filesandcollection.GenerateBinaryFile(filepath, 100);
        string filepathnew = checker.GetValidFilePath();
        filesandcollection.GenerateUniqueBinaryFile(filepath, filepathnew, 100);*/
        
        /*Console.WriteLine("-----ЗАДАНИЕ 6:-----");
        ArrayList userList = new ArrayList();
        Console.WriteLine("Введите элементы списка (для завершения введите пустую строку):");
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;
            userList.Add(input);
        }
        filesandcollection list = new filesandcollection(userList);
        Console.Write("Введите значение для удаления: ");
        string valueToRemove = Console.ReadLine();
        list.RemoveAllOccurrences(valueToRemove);
        Console.WriteLine("После удаления:");
        Console.WriteLine(list.ToString());*/
        
        /*Console.WriteLine("-----ЗАДАНИЕ 7:-----");
        List<string> userList2 = new List<string>();
        Console.WriteLine("Введите элементы списка (для завершения введите пустую строку):");
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;
            userList2.Add(input);
        }
        filesandcollection list = new filesandcollection(userList2);
        Console.WriteLine(list.ToString());
        Console.Write("Введите элемент E: ");
        string E = Console.ReadLine();
        list.ReverseBetweenFirstAndLast(E);
        Console.WriteLine("После обработки:");
        Console.WriteLine(list.ToString());*/

        /*Console.WriteLine("-----ЗАДАНИЕ 8:-----");
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
                    Console.WriteLine($"Ошибка: фильм '{movie}' не существует в общем списке. Пропускаем.");
                }
                viewerMovies.Add(movie);
            }
            viewers.Add(viewerMovies);
        }
        List<string> userList2 = new List<string>(); ///////
        var analyzer = new filesandcollection(userList2, allMovies, viewers);
        analyzer.AnalyzeMovies();*/
        
        /*Console.WriteLine("-----ЗАДАНИЕ 9:-----");
        filesandcollection.GenerateTextFileFOR9(filepath); 
        filesandcollection.ProcessFile(filepath);*/
        
        Console.WriteLine("-----ЗАДАНИЕ 10:-----");
        string filePath = "employees.txt";
        List<string> userList2 = new List<string>();
        var allMovies = new HashSet<string>();
        var viewers = new List<HashSet<string>>();
        var processor = new filesandcollection(userList2, allMovies, viewers, filePath);
        processor.ProcessData();
    }
}
///Users/stepanivanov/Documents/project/test/test.txt
///Users/stepanivanov/Documents/project/test/test.txt1
///

