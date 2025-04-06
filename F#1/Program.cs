using System;

namespace ConsoleApp9
{
    /// <summary>
    /// Основной класс программы для демонстрации работы с классами ThreeNumbers и Triangle
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        static void Main(string[] args)
        {
            // Создаем экземпляр чекера ввода
            InputValidator checker = new InputValidator();
            
            Console.WriteLine("Введите три целых числа для создания треугольника:");
            
            // Получаем три числа от пользователя с проверкой на корректность
            int num1 = checker.GetValidWholeNumber();
            int num2 = checker.GetValidWholeNumber();
            int num3 = checker.GetValidWholeNumber();

            // Демонстрация работы с основным классом ThreeNumbers.
            // 1. Конструктор по умолчанию.
            ThreeNumbers numbers1 = new ThreeNumbers();
            Console.WriteLine("\nThreeNumbers (конструктор по умолчанию):");
            Console.WriteLine(numbers1);
            Console.WriteLine($"Максимальная последняя цифра: {numbers1.GetMaxDigit()}");

            // 2. Конструктор с параметрами.
            ThreeNumbers numbers2 = new ThreeNumbers(num1, num2, num3);
            Console.WriteLine("\nThreeNumbers (конструктор с параметрами):");
            Console.WriteLine(numbers2);
            Console.WriteLine($"Максимальная последняя цифра: {numbers2.GetMaxDigit()}");

            // 3. Конструктор копирования.
            ThreeNumbers numbers3 = new ThreeNumbers(numbers2);
            Console.WriteLine("\nThreeNumbers (конструктор копирования):");
            Console.WriteLine(numbers3);
            Console.WriteLine($"Максимальная последняя цифра: {numbers3.GetMaxDigit()}");

            // Демонстрация работы с дочерним классом Triangle
            
            // 1. Конструктор по умолчанию
            Triangle triangle1 = new Triangle();
            Console.WriteLine("\nTriangle (конструктор по умолчанию):");
            Console.WriteLine(triangle1);

            try
            {
                // 2. Конструктор с параметрами
                Triangle triangle2 = new Triangle(num1, num2, num3);
                Console.WriteLine("\nTriangle (конструктор с параметрами):");
                Console.WriteLine(triangle2);
                Console.WriteLine($"Максимальная последняя цифра: {triangle2.GetMaxDigit()}");
                
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nОшибка создания треугольника: {ex.Message}");
                Console.WriteLine("Введенные значения не могут быть сторонами треугольника.");
            }
            
        }
    }
}