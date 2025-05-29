using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("Работает Pascal-компилятор");
        string inputPath = "/Users/stepanivanov/RiderProjects/ConsoleApp13/ConsoleApp13/input.pas";
        string outputPath = "/Users/stepanivanov/RiderProjects/ConsoleApp13/ConsoleApp13/output.lex";
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Файл {inputPath} не найден.");
            return;
        }
        List<string> inputLines = new List<string>(File.ReadAllLines(inputPath));
        IOHandler io = new IOHandler(inputLines);
        LexicalAnalyzer lexer = new LexicalAnalyzer(io);
        List<byte> symbolCodes = new();
        using StreamWriter writer = new StreamWriter(outputPath);
        int tokenCount = 0;
        while (io.CurrentChar != '\0')
        {
            byte symbol = lexer.NextSym();
            tokenCount++;
            symbolCodes.Add(symbol);
            writer.Write($"{symbol} ");
        }
        Console.WriteLine("\nКомпиляция завершена.\n");
        Console.WriteLine("Цифровая последовательность:");
        Console.WriteLine(string.Join(" ", symbolCodes));
    }
}