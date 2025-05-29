public struct TextPosition
{
    private int lineNumber;
    private int charNumber;
    public TextPosition(int line, int ch)
    {
        lineNumber = line;
        charNumber = ch;
    }
    public int LineNumber
    {
        get { return lineNumber; }
        set { lineNumber = value; }
    }
    public int CharNumber
    {
        get { return charNumber; }
        set { charNumber = value; }
    }
}