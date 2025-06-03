

public class SyntaxAnalyzer
{
    private IOHandler io;
    private LexicalAnalyzer lexer;
    private byte symbol;
    private TextPosition token;
    public SyntaxAnalyzer(IOHandler ioHandler, LexicalAnalyzer lexerHandler)
    {
        io = ioHandler;
        lexer = lexerHandler;
        symbol = lexer.NextSym(); // Load the first token
    }
    void accept(byte symbolexpected) 
    {
        if (symbol == symbolexpected) 
        {
            symbol = lexer.NextSym();
        } 
        else 
        {
            io.Error(symbolexpected, io.CurrentPosition);
        }
    }
    //<блок> ::= <раздел меток> <раздел констант> <раздел типов> <раздел переменных> <раздел процедур и функций> <раздел операторов>
    void Block()
    {
        LabelPart();
        ConstPart();
        TypePart();
        VarPart();
        ProcFuncPart();
        StatementPart();
    }
    //<программа> ::= program <имя> (<имя файла> {, <имя файла>}); <блок>
    public void Program()
    {
        accept(LexicalAnalyzer.programsy);  // 'program'
        accept(LexicalAnalyzer.ident);      // '<имя>'
        if (symbol == LexicalAnalyzer.leftpar)           
        {
            accept(LexicalAnalyzer.leftpar);    // '('
            while (symbol != LexicalAnalyzer.rightpar)   
            {
                accept(LexicalAnalyzer.ident);  // '<имя файла>'
                if (symbol == LexicalAnalyzer.rightpar)
                {
                    accept(LexicalAnalyzer.rightpar);   // ')'
                    break;
                }
                accept(LexicalAnalyzer.comma);  // ','
            }
        }
        accept(LexicalAnalyzer.semicolon);      // ';'
        Block();                            // '<блок>'
    }

    //<раздел меток> ::= <пусто> | label <метка> {, <метка>};
    //<метка> ::= <целое без знака>
    void LabelPart()
    {
        if (symbol == LexicalAnalyzer.labelsy) // Либо <Пусто>,  либо идем дальше
        {
            accept(LexicalAnalyzer.labelsy); // 'label'
            accept(LexicalAnalyzer.intc);   // '<метка>'
            while (symbol == LexicalAnalyzer.comma)
            {
                accept(LexicalAnalyzer.comma); // ','
                accept(LexicalAnalyzer.intc); // '<метка>'
            }

            accept(LexicalAnalyzer.semicolon); // ';'
        }
    }
    
    //<раздел констант> ::= <пусто> | const <определение константы>; { <определение константы>;}
    void ConstPart()
    {
        if (symbol == LexicalAnalyzer.constsy)
        {
            accept(LexicalAnalyzer.constsy);
            tempConstPart();
            while (symbol == LexicalAnalyzer.ident)
            {
                tempConstPart();
            }
        }
        
    }
    void tempConstPart()
    {
        ConstOpredelenie();
        accept(LexicalAnalyzer.semicolon); // ';'
    }
    
    //<определение константы> ::= <имя> = <константа>
    void ConstOpredelenie()
    {
        accept(LexicalAnalyzer.ident); // '<имя>'
        accept(LexicalAnalyzer.equal); // '='
        Const();
    }
    
    //<константа> ::= <число без знака> | <знак> <число без знака> | <имя константы> | <знак> <имя константы> | <строка>
    void Const()
    {
        if (symbol == LexicalAnalyzer.plus || symbol == LexicalAnalyzer.minus)
        {
            if (symbol == LexicalAnalyzer.plus)
            {
                accept(LexicalAnalyzer.plus); 
                if (symbol == LexicalAnalyzer.intc)
                {
                    accept(LexicalAnalyzer.intc);
                }
                else if(symbol == LexicalAnalyzer.floatc) 
                {
                    accept(LexicalAnalyzer.floatc);
                }
                else
                {
                    accept(LexicalAnalyzer.ident);
                }
            }
            else
            {
                accept(LexicalAnalyzer.minus);
                if (symbol == LexicalAnalyzer.intc)
                {
                    accept(LexicalAnalyzer.intc);
                }
                else if(symbol == LexicalAnalyzer.floatc)
                {
                    accept(LexicalAnalyzer.floatc);
                }
                else
                {
                    accept(LexicalAnalyzer.ident);
                }
            }
        }
        else if (symbol == LexicalAnalyzer.intc || symbol == LexicalAnalyzer.floatc)
        {
            if (symbol == LexicalAnalyzer.intc)
            {
                accept(LexicalAnalyzer.intc);
            }
            else
            {
                accept(LexicalAnalyzer.floatc);
            }
        }
        else if (symbol == LexicalAnalyzer.ident)
        {
            accept(LexicalAnalyzer.ident);
        }
        else if (symbol == LexicalAnalyzer.charc || symbol == LexicalAnalyzer.stringc)
        {
            if (symbol == LexicalAnalyzer.charc)
            {
                accept(LexicalAnalyzer.charc);
            }
            else
            {
                accept(LexicalAnalyzer.stringc);
            }
        }
    }
    
    //<раздел типов> ::= <пусто> | type <определение типа> ;{ <определение типа>;}
    //<определение типа> ::= <имя> = <тип>
    void TypePart()
    {
        if (symbol == LexicalAnalyzer.typesy)
        {
            accept(LexicalAnalyzer.typesy);
            tempTypePart();
            while (symbol == LexicalAnalyzer.ident)
            {
                tempTypePart();
            }
        }
    }
    void tempTypePart()
    {
        accept(LexicalAnalyzer.ident);
        accept(LexicalAnalyzer.equal);
        if (symbol == LexicalAnalyzer.integersy || symbol == LexicalAnalyzer.realsy ||
            symbol == LexicalAnalyzer.boolsy || symbol == LexicalAnalyzer.charsy ||
            symbol == LexicalAnalyzer.stringsy) 
        {
            if (symbol == LexicalAnalyzer.integersy)
            {
                accept(LexicalAnalyzer.integersy);
            }
            if (symbol == LexicalAnalyzer.realsy)
            {
                accept(LexicalAnalyzer.realsy);
            }
            if (symbol == LexicalAnalyzer.boolsy)
            {
                accept(LexicalAnalyzer.boolsy);
            }
            if (symbol == LexicalAnalyzer.charsy)
            {
                accept(LexicalAnalyzer.charsy);
            }
            if (symbol == LexicalAnalyzer.stringsy)
            {
                accept(LexicalAnalyzer.stringsy);
            }
            accept(LexicalAnalyzer.semicolon);
        }
        else
        {
            io.Error(209, io.CurrentPosition);
        }
    }

    //<раздел переменных> ::= var <описание однотипных переменных> ; {<описание однотипных переменных>;} | <пусто>
    void VarPart()
    {
        if (symbol == LexicalAnalyzer.varsy)
        {
            accept(LexicalAnalyzer.varsy);
            while (symbol == LexicalAnalyzer.ident)
            {
                TempVarPart();
            }
        }
    }
    //<описание однотипных переменных> ::= <имя> {, <имя>} : <тип>
    void OnePer()
    {
        accept(LexicalAnalyzer.ident);
        while (symbol == LexicalAnalyzer.comma) // '{, <имя>}
        {
            accept(LexicalAnalyzer.comma);
            accept(LexicalAnalyzer.ident);
        }
        accept(LexicalAnalyzer.colon); // ':'
        if (symbol == LexicalAnalyzer.intc || symbol == LexicalAnalyzer.floatc ||
            symbol == LexicalAnalyzer.boolsy || symbol == LexicalAnalyzer.charc ||
            symbol == LexicalAnalyzer.stringc) 
        {
            if (symbol == LexicalAnalyzer.integersy)
            {
                accept(LexicalAnalyzer.integersy);
            }
            if (symbol == LexicalAnalyzer.realsy)
            {
                accept(LexicalAnalyzer.realsy);
            }
            if (symbol == LexicalAnalyzer.boolsy)
            {
                accept(LexicalAnalyzer.boolsy);
            }
            if (symbol == LexicalAnalyzer.charsy)
            {
                accept(LexicalAnalyzer.charsy);
            }
            if (symbol == LexicalAnalyzer.stringsy)
            {
                accept(LexicalAnalyzer.stringsy);
            }
            accept(LexicalAnalyzer.semicolon);
        }
        else
        {
            io.Error(209, io.CurrentPosition); // 'Ожидался тип данных'
        }
    }
    void TempVarPart()
    {
        OnePer();
        accept(LexicalAnalyzer.semicolon);
    }


    //<раздел процедур и функций> ::= {<описание процедуры или функции> ;}
    //<описание процедуры или функции> ::= <описание процедуры> | <описание функции>    
    void ProcFuncPart()
    {
        if (symbol == LexicalAnalyzer.procedurensy)
        {
            accept(LexicalAnalyzer.procedurensy);
            ProcedurePart();
        }
        else if (symbol == LexicalAnalyzer.functionsy)
        {
            accept(LexicalAnalyzer.functionsy);
        }
        
    }
    //<описание процедуры> ::= <заголовок процедуры> <блок>
    void ProcedurePart()
    {
        HeadProcedurePart();
        Block();
    }
    // <заголовок процедуры> ::= procedure <имя> ; | procedure <имя> (<раздел формальных параметров> {; <раздел формальных параметров>}) ;
    void HeadProcedurePart()
    {
        accept(LexicalAnalyzer.procedurensy);
        accept(LexicalAnalyzer.ident);
        if (symbol == LexicalAnalyzer.leftpar)
        {
            accept(LexicalAnalyzer.leftpar);
            FormalParametrs();
            accept(LexicalAnalyzer.semicolon);
            while (symbol != LexicalAnalyzer.rightpar)
            {
                FormalParametrs();
                accept(LexicalAnalyzer.semicolon);
            }
            accept(LexicalAnalyzer.rightpar);
            accept(LexicalAnalyzer.semicolon);
        }
        
        
        
        
        accept(LexicalAnalyzer.semicolon);
    }
    // <раздел формальных параметров> ::= <группа параметров> | var <группа параметров> | function <группа параметров> | procedure <имя> {, <имя>}
    void FormalParametrs()
    {
        if (symbol == LexicalAnalyzer.ident)
        {
            groupParametrs(); 
        }
        else if (symbol == LexicalAnalyzer.varsy)
        {
            accept(LexicalAnalyzer.varsy);
            groupParametrs(); 
        }
        else if (symbol == LexicalAnalyzer.functionsy)
        {
            accept(LexicalAnalyzer.functionsy);
            groupParametrs(); 
        }
        else if (symbol == LexicalAnalyzer.procedurensy)
        {
            accept(LexicalAnalyzer.procedurensy);
            accept(LexicalAnalyzer.ident);
            accept(LexicalAnalyzer.comma);
            while (symbol == LexicalAnalyzer.ident)
            {
                accept(LexicalAnalyzer.ident);
                accept(LexicalAnalyzer.comma);
            }
        }
    }
    // <группа параметров> ::= <имя> {, <имя>} : <имя типа>
    void groupParametrs()
    {
        accept(LexicalAnalyzer.ident);
        accept(LexicalAnalyzer.comma);
        while (symbol == LexicalAnalyzer.ident)
        {
            accept(LexicalAnalyzer.ident);
            accept(LexicalAnalyzer.comma);
        }
        accept(LexicalAnalyzer.colon);
        if (symbol == LexicalAnalyzer.integersy || symbol == LexicalAnalyzer.realsy ||
            symbol == LexicalAnalyzer.boolsy || symbol == LexicalAnalyzer.charsy || symbol == LexicalAnalyzer.stringsy)
        {
            if (symbol == LexicalAnalyzer.integersy)
            {
                accept(LexicalAnalyzer.integersy);
            }

            if (symbol == LexicalAnalyzer.realsy)
            {
                accept(LexicalAnalyzer.realsy);
            }

            if (symbol == LexicalAnalyzer.boolsy)
            {
                accept(LexicalAnalyzer.boolsy);
            }

            if (symbol == LexicalAnalyzer.charsy)
            {
                accept(LexicalAnalyzer.charsy);
            }

            if (symbol == LexicalAnalyzer.stringsy)
            {
                accept(LexicalAnalyzer.stringsy);
            }
        }
        else
        {
            io.Error(209, io.CurrentPosition);
        }
    }
    // <описание функции> ::= <заголовок функции> <блок>
    void FunctionPart()
    {
        HeadFuncionPart();
        Block();
    }
    // <заголовок функции> ::= function <имя> : <тип результата> ; | function <имя> (<раздел формальных параметров> {; <раздел формальных параметров>}) : <тип результата> ;
    void HeadFuncionPart()
    {
        accept(LexicalAnalyzer.functionsy);
        accept(LexicalAnalyzer.ident);
        if (symbol == LexicalAnalyzer.colon)
        {
            accept(LexicalAnalyzer.colon);
            accept(LexicalAnalyzer.ident);
            accept(LexicalAnalyzer.semicolon);
        }
        else if (symbol == LexicalAnalyzer.leftpar)
        {
            accept(LexicalAnalyzer.leftpar);
            FormalParametrs();
            accept(LexicalAnalyzer.semicolon);
            while (symbol != LexicalAnalyzer.rightpar)
            {
                FormalParametrs();
                accept(LexicalAnalyzer.semicolon);
            }
            accept(LexicalAnalyzer.rightpar);
            accept(LexicalAnalyzer.colon);
            accept(LexicalAnalyzer.ident);
            accept(LexicalAnalyzer.semicolon);
        }
        
    }
    
}
