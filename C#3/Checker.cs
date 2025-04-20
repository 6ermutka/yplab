namespace lab3;

public class Chekcer
{
    private string messagewhole;
    private string messagedouble;
    private string messagefilepath;
    private string errorwhole;
    private string errordouble;
    private string errorfilepath;
    private string errorsubname;
    private string errorini;
    private string errorformatnumber;
    public Chekcer()
    {
        this.messagewhole = "Введите целочисленное число: ";
        this.errorwhole = "Ошибка: введено не целочисленное число!";
        this.messagedouble = "Введите вещественное число: ";
        this.errordouble = "Ошибка: введено не вещественное число!";
        this.messagefilepath = "Введите путь к файлу: ";
        this.errorfilepath = "Ошибка: введен невалидный путь или файл не существует!";
        this.errorsubname = "Ошибка: введена неприавльно фамилия";
        this.errorini = "Ошибка: введены неправильные инициалы";
        this.errorformatnumber = "Ошибка: введен неправильный номер телефона!";
    }
    public int GetValidWholeNumber()
    {
        while (true)
        {
            Console.Write(messagewhole);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int number))
            {
                return number;
            }

            Console.WriteLine(errorwhole);
        }
    }
    public double GetValidDoubleNumber()
    {
        while (true)
        {
            Console.Write(messagedouble);
            string input = Console.ReadLine();
            if (double.TryParse(input, out double number))
            {
                return number;
            }
            Console.WriteLine(errordouble);
        }
    }
    public string GetValidFilePath()
    {
        while (true)
        {
            Console.Write(messagefilepath);
            string input = Console.ReadLine();
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine(errorfilepath);
                    continue;
                }
                string fullPath = Path.GetFullPath(input);
                if (File.Exists(fullPath)) return fullPath;
                else Console.WriteLine(errorfilepath);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException || ex is NotSupportedException)
            {
                Console.WriteLine($"Ошибка: {errorfilepath}");
            }
        }
    }
    public string GetValidSubname()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || input.Length > 20 || input.Any(char.IsDigit))
            {
                Console.Write(errorsubname);
                Console.WriteLine("");
            }
            else return input;
        }
    }
    public string GetValidInicial()
    {
        while (true)
        {
            string input = Console.ReadLine();
            bool isInitials = input.Length == 4 && char.IsUpper(input[0]) && input[1] == '.' && char.IsUpper(input[2]) && input[3] == '.';
            if (!isInitials)
            {
                Console.WriteLine(errorini);
            }
            else return input;
        }
    }
    public string GetValidFormattedNumber()
    {
        while (true)
        {
            string input = Console.ReadLine();
            bool isValid = input.Length == 9 && input[3] == '-' && input[6] == '-' && input.Remove(3, 1).Remove(5, 1).All(char.IsDigit);
            if (!isValid)
            {
                Console.WriteLine(errorformatnumber);
                Console.WriteLine("");
            }
            else return input;
        }
    }
}