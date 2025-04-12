using System;
using System.IO;

public class InputValidator
{
    private string messagewhole;
    private string messagedouble;
    private string messagefilepath;
    private string errorwhole;
    private string errordouble;
    private string errorfilepath;
    
    public InputValidator()
    {
        this.messagewhole = "Введите целочисленное число: ";
        this.errorwhole = "Ошибка: введено не целочисленное число!";
        this.messagedouble = "Введите вещественное число: ";
        this.errordouble = "Ошибка: введено не вещественное число!";
        this.messagefilepath = "Введите путь к файлу: ";
        this.errorfilepath = "Ошибка: введен невалидный путь или файл не существует!";
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
}