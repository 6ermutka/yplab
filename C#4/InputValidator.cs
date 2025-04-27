using System;

namespace Lab9
{
    public class InputValidator
    {
        public double GetValidDoubleNumber(string input)
        {
            if (double.TryParse(input, out double number))
            {
                return number;
            }
            throw new ArgumentException("Ошибка: введено не вещественное число!");
        }
        
        public int GetValidWholeNumber(string input)
        {
            if (int.TryParse(input, out int number))
            {
                return number;
            }
            throw new ArgumentException("Ошибка: введено не целочисленное число!");
        }
    }
}