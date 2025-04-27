using System;

namespace Lab9
{
    public class LineSegment
    {
        private double x;
        private double y;
        
        public LineSegment()
        {
            this.x = 0;
            this.y = 0;
        }
        
        public LineSegment(double x, double y)
        {
            if (y < x)
            {
                throw new ArgumentException("Конец отрезка не может быть меньше начала");
            }
            this.x = x;
            this.y = y;
        }
        
        public double X { get { return this.x; } }
        public double Y { get { return this.y; } }
        
        public bool Contains(double number)
        {
            return number >= x && number <= y;
        }
        
        public override string ToString()
        {
            return $"[{x}, {y}]";
        }
        
        public static double operator !(LineSegment segment)
        {
            return segment.y - segment.x;
        }
        
        public static LineSegment operator ++(LineSegment segment)
        {
            return new LineSegment(segment.x + 1, segment.y + 1);
        }
        
        public static explicit operator int(LineSegment segment)
        {
            return (int)segment.x;
        }
        
        public static implicit operator double(LineSegment segment)
        {
            return segment.y;
        }
        
        public static LineSegment operator +(LineSegment segment, int d)
        {
            return new LineSegment(segment.x + d, segment.y);
        }
            
        public static LineSegment operator +(int d, LineSegment segment)
        {
            return new LineSegment(segment.x, segment.y + d);
        }
        
        public static bool operator <(LineSegment segment, int number)
        {
            return segment.Contains(number);
        }
        
        public static bool operator >(LineSegment segment, int number)
        {
            throw new NotImplementedException("Оператор > не реализован");
        }
    }
}