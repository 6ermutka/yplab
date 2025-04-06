namespace ConsoleApp9;


public class Triangle : ThreeNumbers
{
    /// <summary>
    /// Конструктор по умолчанию, инициализирует стороны нулями.
    /// </summary>
    public Triangle() : base() { }
    /// <summary>
    /// Конструктор с параметрами для инициализации сторон треугольника.
    /// </summary>
    /// <param name="a">Первая сторона.</param>
    /// <param name="b">Вторая сторона.</param>
    /// <param name="c">Третья сторона.</param>
    public Triangle(int a, int b, int c) : base(a, b, c)
    {
        if (a <= 0 || b <= 0 || c <= 0)
        {
            throw new ArgumentException("Все стороны треугольника должны быть положительными числами.");
        }
        if (a + b <= c || a + c <= b || b + c <= a)
        {
            throw new ArgumentException("Сумма любых двух сторон треугольника должна быть больше третьей стороны.");
        }
    }
    /// <summary>
    /// Вычисляет периметр треугольника.
    /// </summary>
    /// <returns>Периметр треугольника.</returns>
    public int CalculatePerimeter()
    {
        return A + B + C;
    }
    /// <summary>
    /// Вычисляет площадь треугольника по формуле Герона.
    /// </summary>
    /// <returns>Площадь треугольника.</returns>
    public double CalculateArea()
    {
        double p = CalculatePerimeter() / 2.0;
        return Math.Sqrt(p * (p - A) * (p - B) * (p - C));
    }
    /// <summary>
    /// Возвращает строковое представление треугольника.
    /// </summary>
    /// <returns>Строка с информацией о сторонах, периметре и площади.</returns>
    public override string ToString()
    {
        return $"{base.ToString()}\nПериметр: {CalculatePerimeter()}, Площадь: {CalculateArea()}";
    }
}