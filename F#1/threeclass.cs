namespace ConsoleApp9;

/// <summary>
/// Базовый класс для хранения трех целых чисел.
/// </summary>
public class ThreeNumbers
{
    private int a; 
    private int b; 
    private int c;
    /// <summary>
    /// Конструктор по умолчанию, инициализирует числа нулями.
    /// </summary>
    public ThreeNumbers()
    {
        this.a = 0;
        this.b = 0;
        this.c = 0;
    }
    /// <summary>
    /// Конструктор с параметрами для инициализации трех чисел.
    /// </summary>
    /// <param name="a">Первое число.</param>
    /// <param name="b">Второе число.</param>
    /// <param name="c">Третье число.</param>
    public ThreeNumbers(int a, int b, int c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    /// <summary>
    /// Конструктор копирования.
    /// </summary>
    /// <param name="source">Объект для копирования.</param>
    public ThreeNumbers(ThreeNumbers source)
    {
        this.a = source.a;
        this.b = source.b;
        this.c = source.c;
    }
    /// <summary>
    /// Свойство для получения первого числа.
    /// </summary>
    public int A
    { get { return this.a; } }
    /// <summary>
    /// Свойство для получения второго числа.
    /// </summary>
    public int B
    { get { return this.b; } }
    /// <summary>
    /// Свойство для получения третьего числа.
    /// </summary>
    public int C
    { get { return this.c; } }
    /// <summary>
    /// Возвращает максимальную последнюю цифру из трех чисел.
    /// </summary>
    /// <returns>Максимальная последняя цифра.</returns>
    public int GetMaxDigit()
    {
        int lastDigit1 = Math.Abs(a % 10);
        int lastDigit2 = Math.Abs(b % 10);
        int lastDigit3 = Math.Abs(c % 10);
        return Math.Max(Math.Max(lastDigit1, lastDigit2), lastDigit3);
    }
    /// <summary>
    /// Возвращает строковое представление трех чисел.
    /// </summary>
    /// <returns>Строка с тремя числами.</returns>
    public override string ToString()
    {
        return $"Число a: {a}, b: {b}, c: {c}";
    }
}