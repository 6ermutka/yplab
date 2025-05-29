public class Error
{
    public TextPosition Position { get; set; }
    public int Code { get; set; }
    public Error(int code, TextPosition position)
    {
        Code = code;
        Position = position;
    }
    public string GetDescription()
    {
        return Code switch
        {        
            203 => "Целое число превышает предел (32767)",
            204 => "Вещественное число превышает предел",
            205 => "Незакрытая символьная константа",
            206 => "Пустая символьная константа",
            207 => "Недопустимый символ в константе",
            208 => "Недопустимый символ в коде",
            _ => "Неизвестная лексическая ошибка"
        };
    }
}