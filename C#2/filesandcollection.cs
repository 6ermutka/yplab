using System.Collections;
using System.Text;
namespace lab2;

public class FilesAndCollection
{
    private List<string> list;
    private  LinkedList<string> list1;
    private HashSet<string> allMovies;
    private List<HashSet<string>> viewers;
    private readonly string filePath;
    public FilesAndCollection(HashSet<string> allMovies, List<HashSet<string>> viewers)
    {
        this.allMovies = allMovies;
        this.viewers = viewers;
    }
    
    public FilesAndCollection(LinkedList<string> inputList)
    {
        this.list1 = inputList;
    }
    public FilesAndCollection(string filePath)
    {
        this.filePath = filePath;
    }
    public FilesAndCollection(List<string> inputList)
    {
        this.list = inputList;
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
            if (list[i] == value)
            {
                list.RemoveAt(i);
            }
        }
    }
    public void ReverseBetweenFirstAndLast(string value)
    {
        LinkedListNode<string> firstNode = null;
        LinkedListNode<string> lastNode = null;
        var currentNode = list1.First;
        while (currentNode != null)
        {
            if (currentNode.Value == value)
            {
                if (firstNode == null)
                    firstNode = currentNode;
                lastNode = currentNode;
            }
            currentNode = currentNode.Next;
        }
        if (firstNode != null && lastNode != null && firstNode != lastNode)
        {
            var nodesToReverse = new List<string>();
            currentNode = firstNode.Next;
            while (currentNode != null && currentNode != lastNode)
            {
                nodesToReverse.Add(currentNode.Value);
                currentNode = currentNode.Next;
            }
            currentNode = firstNode.Next;
            while (currentNode != null && currentNode != lastNode)
            {
                var nextNode = currentNode.Next;
                list1.Remove(currentNode);
                currentNode = nextNode;
            }
            currentNode = firstNode;
            for (int i = nodesToReverse.Count - 1; i >= 0; i--)
            {
                list1.AddAfter(currentNode, nodesToReverse[i]);
                currentNode = currentNode.Next;
            }
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
    public string PrintNewList()
    {
        if (list1 == null || list1.Count == 0)
            return "Список пуст";
    
        var sb = new StringBuilder();
        sb.Append("[");
    
        var currentNode = list1.First;
        while (currentNode != null)
        {
            sb.Append(currentNode.Value);
            currentNode = currentNode.Next;
            if (currentNode != null)
                sb.Append(", ");
        }
    
        sb.Append("]");
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
    
    public void GenerateTextFileFOR9()
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
    
    public void ProcessFile()
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
    
    public double CalculateAverageEmployeesPerDepartment()
    {
        var departmentCounts = new Dictionary<string, int>();
        int totalEmployees = 0;
        StreamReader f = new StreamReader(filePath);
        string line;
        while ((line = f.ReadLine()) != null)
        {
            string[] parts = line.Split(' ');
            string phone = parts[2];
            string[] phoneParts = phone.Split('-');
            string departmentKey = $"{phoneParts[0]}-{phoneParts[1]}";
            if (departmentCounts.ContainsKey(departmentKey))
                departmentCounts[departmentKey]++;
            else
                departmentCounts[departmentKey] = 1;
            totalEmployees++;
        }
        f.Close();
        return (double)totalEmployees / departmentCounts.Count;
    }
}
    
    
