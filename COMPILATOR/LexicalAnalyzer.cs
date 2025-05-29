using System.Text;
class LexicalAnalyzer
{
    private IOHandler io;
    public LexicalAnalyzer(IOHandler ioHandler)
    {
        io = ioHandler;
    }

    public const byte
        star = 21,
        slash = 60,
        equal = 16,
        comma = 20,
        semicolon = 14,
        colon = 5,
        point = 61,
        arrow = 62,
        leftpar = 9,
        rightpar = 4,
        lbracket = 11,
        rbracket = 12,
        later = 65,
        greater = 66,
        laterequal = 67,
        greaterequal = 68,
        latergreater = 69,
        plus = 70,
        minus = 71,
        assign = 51,
        twopoints = 74,
        ident = 2,
        floatc = 82,
        intc = 15,
        charc = 83,
        casesy = 31,
        elsesy = 32,
        filesy = 57,
        gotosy = 33,
        thensy = 52,
        typesy = 34,
        untilsy = 53,
        dosy = 54,
        withsy = 37,
        ifsy = 56,
        insy = 100,
        ofsy = 101,
        orsy = 102,
        tosy = 103,
        endsy = 104,
        varsy = 105,
        divsy = 106,
        andsy = 107,
        notsy = 108,
        forsy = 109,
        modsy = 110,
        nilsy = 111,
        setsy = 112,
        beginsy = 113,
        whilesy = 114,
        arraysy = 115,
        constsy = 116,
        labelsy = 117,
        downtosy = 118,
        packedsy = 119,
        recordsy = 120,
        repeatsy = 121,
        programsy = 122,
        functionsy = 123,
        procedurensy = 124,
        banintfloatcharc = 207,
        maxfloatc = 204,
        maxintc = 203,
        unclosedstr = 205,
        emptystr = 206,
        bansymbol = 208,
        stringc = 84;

    private byte symbol;
    private TextPosition token;
    private string addrName;
    private int nmb_int;
    private double nmb_float;
    private char one_symbol;

    public byte NextSym()
    {
        token = io.CurrentPosition;
        while (char.IsWhiteSpace(io.CurrentChar))
        {
            io.NextChar();
        }
        if (io.CurrentChar == '{')
        {
            io.NextChar();
            while (io.CurrentChar != '}' && io.CurrentChar != '\0')
            {
                io.NextChar();
            }
            if (io.CurrentChar == '}')
            {
                io.NextChar();
                return NextSym();
            }
        }
        if (io.CurrentChar == '\'')
        {
            io.NextChar();
            bool isString = false;
            StringBuilder strBuilder = new StringBuilder();
            while (true)
            {
                if (io.CurrentChar == '\0')
                {
                    io.Error(205, token);
                    symbol = unclosedstr;
                    return symbol;
                }
            
                if (io.CurrentChar == '\'')
                {
                    io.NextChar();
                    if (io.CurrentChar == '\'')
                    {
                        strBuilder.Append('\'');
                        io.NextChar();
                        isString = true;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                strBuilder.Append(io.CurrentChar);
                isString = isString || strBuilder.Length > 1;
                io.NextChar();
            }

            if (strBuilder.Length == 0)
            {
                io.Error(206, token);
                symbol = emptystr;
            }
            else if (!isString)
            {
                if (strBuilder.Length == 1)
                {
                    one_symbol = strBuilder[0];
                    symbol = charc;
                }
                else
                {
                    io.Error(207, token);
                    symbol = banintfloatcharc;
                }
            }
            else
            {
                addrName = strBuilder.ToString();
                symbol = stringc;
            }
            return symbol;
        }
        
        
        if (char.IsLetter(io.CurrentChar))
        {
            string name = "";
            while (char.IsLetterOrDigit(io.CurrentChar))
            {
                name += io.CurrentChar;
                io.NextChar();
            }
            addrName = name;
            byte lname = (byte)Math.Min(name.Length, 9);
            Keywords kw = new Keywords();
            if (kw.Kw.ContainsKey(lname) && kw.Kw[lname].ContainsKey(name.ToLower()))
                symbol = kw.Kw[lname][name.ToLower()];
            else
                symbol = ident;
            return symbol;
        }
        
        if (char.IsDigit(io.CurrentChar))
        {
            byte digit;
            Int16 maxInt = Int16.MaxValue;
            nmb_int = 0;
            nmb_float = 0;
            bool isFloat = false;
            bool twopoin = false;
            bool intOverflow = false;
            double divisor = 1;
            while (char.IsDigit(io.CurrentChar))
            {
                digit = (byte)(io.CurrentChar - '0');
        
                if (!isFloat && !intOverflow)
                {
                    if (nmb_int > maxInt / 10 || (nmb_int == maxInt / 10 && digit > maxInt % 10))
                    {
                        intOverflow = true;
                        io.Error(203, token);
                        symbol = maxintc;
                    }
                    else
                    {
                        nmb_int = 10 * nmb_int + digit;
                    }
                }
                nmb_float = 10 * nmb_float + digit;
                io.NextChar();
            }
            if (char.IsLetter(io.CurrentChar))
            {
                io.Error(207, io.CurrentPosition);
                symbol = banintfloatcharc;
                while (char.IsLetterOrDigit(io.CurrentChar))
                {
                    io.NextChar();
                }
                return symbol;
            }
            if (io.CurrentChar == '.')
            {
                
                isFloat = true;
                io.NextChar();
                if (io.CurrentChar == '.')
                {
                    symbol = twopoints;
                    return symbol;
                }
                if (!char.IsDigit(io.CurrentChar))
                {
                    io.Error(207, io.CurrentPosition);
                    symbol = banintfloatcharc;
                    io.NextChar();
                    return symbol;
                }
                while (char.IsDigit(io.CurrentChar))
                {
                    digit = (byte)(io.CurrentChar - '0');
                    divisor *= 10;
                    nmb_float += digit / divisor;
                    io.NextChar();
                }
            }
            if (char.ToLower(io.CurrentChar) == 'e')
            {
                isFloat = true;
                TextPosition expStart = io.CurrentPosition;
                io.NextChar();
                int exponent = 0;
                int sign = 1;
                if (io.CurrentChar == '+')
                {
                    io.NextChar();
                }
                else if (io.CurrentChar == '-')
                {
                    sign = -1;
                    io.NextChar();
                }
                if (!char.IsDigit(io.CurrentChar))
                {
                    io.Error(207, expStart);
                    symbol = banintfloatcharc;
                    return symbol;
                }
                while (char.IsDigit(io.CurrentChar))
                {
                    exponent = exponent * 10 + (io.CurrentChar - '0');
                    double testValue = nmb_float * Math.Pow(10, sign * exponent);
                    if (testValue > 1.7e38 || testValue < -1.7e38 || (testValue != 0 && Math.Abs(testValue) < 2.9e-39))
                    {
                        io.Error(204, expStart);
                        symbol = maxfloatc;
                        while (char.IsDigit(io.CurrentChar))
                        {
                            io.NextChar();
                        }
                        return symbol;
                    }
                    io.NextChar();
                }
                nmb_float *= Math.Pow(10, sign * exponent);
            }
            if (isFloat)
            {
                if (nmb_float > 1.7e38 || nmb_float < -1.7e38 || (nmb_float != 0 && Math.Abs(nmb_float) < 2.9e-39))
                {
                    io.Error(204, token);
                    symbol = maxfloatc;
                }
                else if (!intOverflow)
                {
                    symbol = floatc;
                }
            }
            else if (!intOverflow)
            {
                symbol = intc;
            }
            return symbol;
        }
        switch (io.CurrentChar)
        {
            case '<':
                io.NextChar();
                if (io.CurrentChar == '=')
                {
                    symbol = laterequal;
                    io.NextChar();
                }
                else if (io.CurrentChar == '>')
                {
                    symbol = latergreater;
                    io.NextChar();
                }
                else
                {
                    symbol = later;
                }
                break;
            case '>':
                io.NextChar();
                if (io.CurrentChar == '=')
                {
                    symbol = greaterequal;
                    io.NextChar();
                }
                else
                {
                    symbol = greater;
                }
                break;

            case ':':
                io.NextChar();
                if (io.CurrentChar == '=')
                {
                    symbol = assign;
                    io.NextChar();
                }
                else
                {
                    symbol = colon;
                }
                break;

            case ';':
                symbol = semicolon;
                io.NextChar();
                break;

            case '.':
                io.NextChar();
                if (io.CurrentChar == '.')
                {
                    symbol = twopoints;
                    io.NextChar();
                }
                else
                {
                    symbol = point;
                }
                break;

            case ',':
                symbol = comma;
                io.NextChar();
                break;

            case '+':
                symbol = plus;
                io.NextChar();
                break;

            case '-':
                symbol = minus;
                io.NextChar();
                break;

            case '*':
                symbol = star;
                io.NextChar();
                break;

            case '/':
                symbol = slash;
                io.NextChar();
                break;

            case '=':
                symbol = equal;
                io.NextChar();
                break;

            case '(':
                symbol = leftpar;
                io.NextChar();
                break;

            case ')':
                symbol = rightpar;
                io.NextChar();
                break;

            case '[':
                symbol = lbracket;
                io.NextChar();
                break;

            case ']':
                symbol = rbracket;
                io.NextChar();
                break;

            case '^':
                symbol = arrow;
                io.NextChar();
                break;

            case '\0':
                symbol = 0;
                break;

            default:
                io.Error(208, io.CurrentPosition);
                symbol = bansymbol;
                io.NextChar();
                break;
        }
        return symbol;
    }
}