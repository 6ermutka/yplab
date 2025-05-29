using System;
using System.Collections.Generic;
using System.IO;

public class IOHandler
{
    const int MAXLINE = 256;
    const int ERRMAX = 10;
    private List<string> sourceLines;
    private int currentLineIndex = -1;
    private string currentLine = "";
    private char currentChar;
    private TextPosition positionNow;
    private List<Error> errorList = new();
    private bool errorOverflow = false;
    public char CurrentChar
    {
        get { return currentChar; }
        set { currentChar = value; }
    }
    public TextPosition CurrentPosition
    {
        get { return positionNow; }
        set { positionNow = value; }
    }
    public IOHandler(List<string> lines)
    {
        sourceLines = lines;
        ReadNextLine();
    }
    public void NextChar()
    {
        if (positionNow.CharNumber >= currentLine.Length)
        {
            ListThisLine();
            if (errorList.Count > 0)
                ListErrors();
            ReadNextLine();
        }
        else
        {
            positionNow.CharNumber++;
        }
        if (positionNow.CharNumber <= currentLine.Length)
        {
            currentChar = currentLine[positionNow.CharNumber - 1];
        }
    }
    private void ReadNextLine()
    {
        currentLineIndex++;
        if (currentLineIndex >= sourceLines.Count)
        {
            currentLine = "";
            currentChar = '\0';
            return;
        }
        currentLine = sourceLines[currentLineIndex];
        if (currentLine.Length > MAXLINE)
        { 
            currentLine = currentLine.Substring(0, MAXLINE); 
        }
        positionNow.LineNumber = currentLineIndex + 1;
        positionNow.CharNumber = 1;
        if (currentLine.Length > 0)
        {
            currentChar = currentLine[0];
        }
        else
        {
            currentChar = '\n';
        }
        errorList.Clear();
        errorOverflow = false;
    }
    public void Error(int errorCode, TextPosition pos)
    {
        if (errorList.Count >= ERRMAX)
        {
            errorOverflow = true;
        }
        else
        {
            errorList.Add(new Error(errorCode, pos));
        }
    }
    private void ListThisLine()
    {
        Console.WriteLine($"{positionNow.LineNumber,4}: {currentLine}");
    }
    
    private void ListErrors()
    {
        foreach (var err in errorList)
        {
            string markerLine = new string(' ', err.Position.CharNumber + 5) + "^ ошибка код " + err.Code;
            string messageLine = new string('*', 6) + " " + err.GetDescription();
            Console.WriteLine(markerLine);
            Console.WriteLine(messageLine);
        }
        if (errorOverflow)
        {
            Console.WriteLine("** Превышено максимальное количество ошибок в строке!");
        }
    }
}