namespace ConsoleApp9;

/// <summary>
/// Класс для проверки ввода чисел без проверки диапазона
/// </summary>
public class InputValidator
{
    private string messagewhole;
    private string errorwhole;
    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public InputValidator()
    {
        this.messagewhole = "Введите целочисленное число: ";
        this.errorwhole = "Ошибка: введено не целочисленное число!";
    }
    /// <summary>
    /// Запрашивает у пользователя число пока не будет введено корректное значение
    /// </summary>
    /// <returns>Введенное число</returns>
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
}