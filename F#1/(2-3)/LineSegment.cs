namespace ConsoleApp10
{
    /// <summary>
    /// Класс, представляющий отрезок на координатной прямой
    /// </summary>
    public class LineSegment
    {
        private double x;
        private double y;
        /// <summary>
        /// Конструктор по умолчанию, инициализирует отрезок [0, 0]
        /// </summary>
        public LineSegment()
        {
            this.x = 0;
            this.y = 0;
        }
        /// <summary>
        /// Конструктор с параметрами для инициализации отрезка
        /// </summary>
        /// <param name="x">Начало отрезка</param>
        /// <param name="y">Конец отрезка</param>
        /// <exception cref="ArgumentException">Выбрасывается, если конец отрезка меньше начала</exception>
        public LineSegment(double x, double y)
        {
            if (y < x)
            {
                throw new ArgumentException("Конец отрезка не может быть меньше начала");
            }
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Начало отрезка
        /// </summary>
        public double X
        { 
            get { return this.x; } 
        }
        /// <summary>
        /// Конец отрезка
        /// </summary>
        public double Y
        { 
            get { return this.y; } 
        }
        /// <summary>
        /// Проверяет, принадлежит ли число отрезку
        /// </summary>
        /// <param name="number">Число для проверки</param>
        /// <returns>True, если число принадлежит отрезку, иначе False</returns>
        public bool Contains(double number)
        {
            return number >= x && number <= y;
        }
        /// <summary>
        /// Возвращает строковое представление отрезка
        /// </summary>
        /// <returns>Строка в формате [x, y]</returns>
        public override string ToString()
        {
            return $"Отрезок [{x}, {y}]";
        }
        
        // Унарные операции
        /// <summary>
        /// Оператор ! вычисляет длину отрезка
        /// </summary>
        public static double operator !(LineSegment segment)
        {
            return segment.y - segment.x;
        }
        
        /// <summary>
        /// Оператор ++ увеличивает координаты границ отрезка на 1
        /// </summary>
        public static LineSegment operator ++(LineSegment segment)
        {
            return new LineSegment(segment.x + 1, segment.y + 1);
        }
        
        // Операции приведения типа
        /// <summary>
        /// Явное приведение к int (целая часть координаты x)
        /// </summary>
        public static explicit operator int(LineSegment segment)
        {
            return (int)segment.x;
        }
        
        /// <summary>
        /// Неявное приведение к double (координата y)
        /// </summary>
        public static implicit operator double(LineSegment segment)
        {
            return segment.y;
        }
        
        // Бинарные операции
        /// <summary>
        /// Оператор + добавляет число d к границам отрезка
        /// </summary>
        public static LineSegment operator +(LineSegment segment, int d)
        {
            return new LineSegment(segment.x + d, segment.y);
        }
            
        /// <summary>
        /// Оператор + добавляет число d к границам отрезка (правосторонняя версия)
        /// </summary>
        public static LineSegment operator +(int d, LineSegment segment)
        {
            return new LineSegment(segment.x, segment.y + d);
        }
        /// <summary>
        /// Оператор < проверяет, попадает ли число в отрезок
        /// </summary>
        public static bool operator <(LineSegment segment, int number)
        {
            return segment.Contains(number);
        }
        /// <summary>
        /// Оператор > 
        /// </summary>
        public static bool operator >(LineSegment segment, int number)
        {
            throw new NotImplementedException("Оператор > ...");
        }
    }
}