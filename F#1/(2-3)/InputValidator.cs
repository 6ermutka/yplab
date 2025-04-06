namespace ConsoleApp10;

/// <summary>
/// Класс для проверки ввода чисел без проверки диапазона
/// </summary>
public class InputValidator
{
    private string messagewhole;
    private string messagedouble;
    private string errorwhole;
    private string errordouble;
    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public InputValidator()
    {
        this.messagewhole = "Введите целочисленное число: ";
        this.errorwhole = "Ошибка: введено не целочисленное число!";
        this.messagedouble = "Введите вещественное число: ";
        this.errordouble = "Ошибка: введено не вещественное число!";
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
    /// <summary>
    /// Запрашивает у пользователя вещественное число пока не будет введено корректное значение
    /// </summary>
    /// <returns>Введенное число</returns>
    public double GetValidDoubleNumber()
    {
        while (true)
        {
            Console.Write(messagedouble);
            string input = Console.ReadLine();
            if (double.TryParse(input, out  double number))
            {
                return number;
            }
            Console.WriteLine(errordouble);
        }
    }
}