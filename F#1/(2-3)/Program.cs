using System;

namespace ConsoleApp10
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InputValidator checker = new InputValidator();
                Console.WriteLine("Введите x: ");
                double x = checker.GetValidDoubleNumber();
                Console.WriteLine("Введите y: ");
                double y = checker.GetValidDoubleNumber();
                Console.WriteLine("Введите N: ");
                double N = checker.GetValidDoubleNumber();
                LineSegment segment1 = new LineSegment();
                Console.WriteLine("Дефолтный обьект: ");
                Console.WriteLine(segment1);
                Console.WriteLine("Обьект на основе введенных x, y");
                LineSegment segment2 = new LineSegment(x, y);
                Console.WriteLine(segment2);
                Console.WriteLine($"Содержит {N}: {segment2.Contains(N)}");
                Console.WriteLine($"Длина отрезка: {!segment2}");
                segment2++;
                Console.WriteLine($"Новые координаты(+1): {segment2}");
                int xInt = (int)segment2; // Явное приведение к int
                Console.WriteLine($"Целая часть: {xInt} от {segment2.X}");
                double yDouble = segment2; // Неявное приведение к double
                Console.WriteLine($"double Y: {yDouble}");
                LineSegment shiftedRight = segment2 + 3;
                Console.WriteLine($"Начало отрезка + 3: {shiftedRight}");
                LineSegment shiftedLeft = 2 + segment2;
                Console.WriteLine($"2 + Конец отрезка: {shiftedLeft}");
                int testNumber = checker.GetValidWholeNumber();
                Console.WriteLine($"Проверка {testNumber}: {segment2 < testNumber} (число {testNumber} {(segment2 < testNumber ? "входит" : "не входит")} в отрезок)");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

