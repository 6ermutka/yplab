using System.Collections;
using System.Text;
namespace lab2;

public class filesandcollection
{
    private List<string> list;
    private HashSet<string> allMovies;
    private List<HashSet<string>> viewers;
    private string filePath;
    public filesandcollection(List<string> inputList, HashSet<string> allMovies, List<HashSet<string>> viewers, string path)
    {
        this.list = inputList;
        this.allMovies = allMovies;
        this.viewers = viewers;
        this.filePath = path;
    }
    public static void FillWithRandomNumbers(int count, int minValue, int maxValue, string filePath)
    {
        Random random = new Random();
        StreamWriter f = new StreamWriter(filePath);
        for (int i = 0; i < count; i++)
        {
            int number = random.Next(minValue, maxValue + 1);
            f.WriteLine(number);
        }

        f.Close();
    }
    public static bool ContainsNumber(int number, string filePath)
    {
        StreamReader f = new StreamReader(filePath);
        string line;
        while ((line = f.ReadLine()) != null)
            if (int.TryParse(line, out int currentNumber) && currentNumber == number)
            {
                f.Close();
                return true;
            }

        f.Close();
        return false;
    }
    public static void FillWithRandomNumbersFull(int count, int minValue, int maxValue, string filePath)
    {
        Random random = new Random();
        StreamWriter f = new StreamWriter(filePath);
        for (int i = 0; i < count; i++)
        {
            for (int k = 0; k < random.Next(0, 100); k++)
            {
                int number = random.Next(minValue, maxValue + 1);
                f.Write(number);
                f.Write(" ");
            }

            f.WriteLine();
        }

        f.Close();
    }
    public static int SumOfMultiples(string filePath, int k)
    {
        int sum = 0;
        StreamReader f = new StreamReader(filePath);
        string line;
        while ((line = f.ReadLine()) != null)
        {
            string[] numbers = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string numberStr in numbers)
            {
                if (int.TryParse(numberStr, out int number))
                {
                    if (number % k == 0)
                    {
                        sum += number;
                    }
                }
            }
        }
        f.Close();
        return sum;
    }
    public static void GenerateTextFile(int lineCount, string filePath)
    {
        Random random = new Random();
        char[] chars =
        {
            'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п',
            'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        StreamWriter f = new StreamWriter(filePath);
        for (int i = 0; i < lineCount; i++)
        {
            for (int j = 0; j < random.Next(1, 11); j++)
            {
                for (int k = 0; k < random.Next(1, 11); k++)
                {
                    char randomChar = chars[random.Next(chars.Length)];
                    f.Write(randomChar);
                }
                f.Write(" ");
            }
            f.WriteLine();
        }
        f.Close();
    }
    public static void CopyLinesWithoutDigits(string sourceFilePath, string destinationFilePath)
    {
        StreamReader f1 = new StreamReader(sourceFilePath);
        StreamWriter f2 = new StreamWriter(destinationFilePath);
        string line;
        while ((line = f1.ReadLine()) != null)
        {
            if (!line.Any(char.IsDigit))
            {
                f2.WriteLine(line);
            }
        }
        f1.Close();
        f2.Close();
    }
    public static void GenerateBinaryFile(string filePath, int numbersCount)
    {
        Random random = new Random();
        BinaryWriter f = new BinaryWriter(File.Open(filePath, FileMode.Create));
        for (int i = 0; i < numbersCount; i++)
        {
            int number = random.Next(-100, 101);
            f.Write(number);
        }
        f.Close();
    }
    public static void GenerateUniqueBinaryFile(string sourceFilePath, string destinationFilePath, int numbersCount)
    {
        int[] allNumbers = new int[numbersCount];
        int[] uniqueNumbers = new int[numbersCount];
        int uniqueCount = 0;
        BinaryReader f = new BinaryReader(File.Open(sourceFilePath, FileMode.Open));
        for (int i = 0; i < numbersCount; i++) allNumbers[i] = f.ReadInt32();
        f.Close();
        for (int i = 0; i < numbersCount; i++)
        {
            int currentNumber = allNumbers[i];
            bool isUnique = true;
            for (int j = 0; j < uniqueCount; j++)
            {
                if (uniqueNumbers[j] == currentNumber)
                {
                    isUnique = false;
                    break;
                }
            }
            if (isUnique)
            {
                uniqueNumbers[uniqueCount] = currentNumber;
                uniqueCount++;
            }
        }
        BinaryWriter f1 = new BinaryWriter(File.Open(destinationFilePath, FileMode.Create));
        for (int i = 0; i < uniqueCount; i++) f1.Write(uniqueNumbers[i]);
        f1.Close();
    }
    public void RemoveAllOccurrences(string value)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].ToString() == value)
            {
                list.RemoveAt(i);
            }
        }
    }
    public void ReverseBetweenFirstAndLast(string element)
    {
        int firstIndex = -1;
        int lastIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ToString() == element)
            {
                if (firstIndex == -1)
                    firstIndex = i;
                lastIndex = i;
            }
        }
        if (firstIndex == lastIndex)
        {
            Console.WriteLine($"Элемент '{element}' встречается меньше 2 раз. Изменений не сделано.");
            return;
        }
        int left = firstIndex + 1;
        int right = lastIndex - 1;
        while (left < right)
        {
            string temp = list[left];
            list[left] = list[right];
            list[right] = temp;
            left++;
            right--;
        }
    }
    public override string ToString()
    {
        if (list.Count == 0)
            return "Список пуст";
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        int i = 0;
        for (i = 0; i < list.Count-1; i++)
        {
            sb.Append($"{list[i]}, ");
        }
        sb.Append($"{list[i]}] ");
        return sb.ToString();
    }
    public void AnalyzeMovies()
    {
        var watchedByAll = new HashSet<string>(allMovies);
        foreach (var viewerMovies in viewers)
        {
            watchedByAll.IntersectWith(viewerMovies);
        }
        var watchedBySome = new HashSet<string>();
        foreach (var viewerMovies in viewers)
        {
            watchedBySome.UnionWith(viewerMovies);
        }
        var watchedByNone = new HashSet<string>(allMovies);
        watchedByNone.ExceptWith(watchedBySome);
        Console.WriteLine("Результаты анализа:");
        Console.WriteLine("Фильмы, которые посмотрели все зрители:");
        PrintMovies(watchedByAll);
        Console.WriteLine("Фильмы, которые посмотрели некоторые зрители:");
        PrintMovies(watchedBySome);
        Console.WriteLine("Фильмы, которые не посмотрел ни один зритель:");
        PrintMovies(watchedByNone);
    }
    private void PrintMovies(HashSet<string> movies)
    {
        if (movies.Count == 0)
        {
            Console.WriteLine("(нет таких фильмов)");
        }
        else
        {
            foreach (var movie in movies)
            {
                Console.WriteLine($"- {movie}");
            }
        }
    }
    
    public static void GenerateTextFileFOR9(string filePath)
    {
        Random random = new Random();
        char[] chars =
        {
            'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п',
            'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
        };
        StreamWriter f = new StreamWriter(filePath);
        for (int j = 0; j < random.Next(10, 100); j++)
        {
            for (int k = 0; k < random.Next(1, 11); k++)
            {
                char randomChar = chars[random.Next(chars.Length)];
                f.Write(randomChar);
            }

            f.Write(" ");
        }
        f.Close();
    }
    
    public static void ProcessFile(string filePath)
    {
        string[] words = File.ReadAllText(filePath).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        HashSet<char> allConsonants = new HashSet<char>();
        HashSet<char> repeatedConsonants = new HashSet<char>();
        foreach (string word in words)
        {
            HashSet<char> consonantsInCurrentWord = new HashSet<char>();
            foreach (char c in word)
            {
                if ("бвгджзлмнр".Contains(c))
                    consonantsInCurrentWord.Add(c);
            }
            foreach (char consonant in consonantsInCurrentWord)
            {
                if (allConsonants.Contains(consonant))
                    repeatedConsonants.Add(consonant);
                else
                    allConsonants.Add(consonant);
            }
        }
        List<char> result = new List<char>(repeatedConsonants);
        result.Sort();
        Console.WriteLine("Звонкие согласные, встречающиеся более чем в одном слове:");
        Console.WriteLine(string.Join(" ", result));
    }
    
    public void ProcessData()
    {
        try
        {
            var phoneGroups = new Dictionary<string, int>();
            int totalEmployees = 0;

            using (var reader = new StreamReader(filePath))
            {
                int n;
                if (!int.TryParse(reader.ReadLine(), out n) || n <= 0)
                {
                    Console.WriteLine("Некорректное количество сотрудников");
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    ProcessEmployeeLine(line, phoneGroups);
                    totalEmployees++;
                }
            }
            if (phoneGroups.Count == 0)
            {
                Console.WriteLine("Нет данных о сотрудниках");
                return;
            }
            double average = (double)totalEmployees / phoneGroups.Count;
            Console.WriteLine($"Среднее количество сотрудников в подразделении: {average:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    private void ProcessEmployeeLine(string line, Dictionary<string, int> phoneGroups)
    {
        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
        {
            Console.WriteLine($"Некорректный формат строки: {line}");
            return;
        }
        string phone = parts[2];
        string phonePrefix = GetPhonePrefix(phone);

        if (phoneGroups.ContainsKey(phonePrefix))
        {
            phoneGroups[phonePrefix]++;
        }
        else
        {
            phoneGroups[phonePrefix] = 1;
        }
    }
    private string GetPhonePrefix(string phone)
    {
        return phone.Length >= 6 ? phone.Substring(0, 6) : phone;
    }
}
    
    
