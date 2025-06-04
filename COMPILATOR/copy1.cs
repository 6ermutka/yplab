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
            io.ListErrors();
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
        }
        else
        {
            accept(symbol);
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
            accept(LexicalAnalyzer.semicolon);
        }
        else if (symbol == LexicalAnalyzer.functionsy)
        {
            accept(LexicalAnalyzer.functionsy);
            FunctionPart();
            accept(LexicalAnalyzer.semicolon);
        }
        
    }
    //<описание процедуры> ::= <заголовок процедуры> <блок>
    void ProcedurePart()
    {
        HeadProcedurePart();
        Block();
    }
    // <заголовок процедуры> ::= procedure <имя> ; | procedure <имя> (<раздел формальных параметров> {; <раздел формальных параметров>}) ;
    void HeadProcedurePart() {
        accept(LexicalAnalyzer.ident);
        if (symbol != LexicalAnalyzer.leftpar)
        {
            accept(LexicalAnalyzer.semicolon);
        }
        else if (symbol == LexicalAnalyzer.leftpar) {
            accept(LexicalAnalyzer.leftpar);
            FormalParametrs();
            while (symbol == LexicalAnalyzer.semicolon) {
                accept(LexicalAnalyzer.semicolon);
                FormalParametrs();
            }
            accept(LexicalAnalyzer.rightpar);
            accept(LexicalAnalyzer.semicolon);
        }
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
        while (symbol == LexicalAnalyzer.comma)
        {
            accept(LexicalAnalyzer.comma);
            accept(LexicalAnalyzer.ident);
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
        accept(LexicalAnalyzer.ident);
        if (symbol == LexicalAnalyzer.colon)
        {
            accept(LexicalAnalyzer.colon);
            accept(symbol);
            accept(LexicalAnalyzer.semicolon);
        }
        else if (symbol == LexicalAnalyzer.leftpar)
        {
            accept(LexicalAnalyzer.leftpar);
            FormalParametrs(); 
            while (symbol == LexicalAnalyzer.semicolon)
            {
                accept(LexicalAnalyzer.semicolon);
                FormalParametrs();
            }
            accept(LexicalAnalyzer.rightpar);
            accept(LexicalAnalyzer.colon);
            accept(symbol);
            accept(LexicalAnalyzer.semicolon);
        }
        
    }
    
    // <раздел операторов> ::= <составной оператор>
    void StatementPart()
    {
        CompoundStatement();
    }

    // <оператор> ::= <непомеченный оператор> | <метка> <непомеченный оператор>
    void Statement()
    {
        if (symbol == LexicalAnalyzer.intc) // <метка>
        {
            accept(LexicalAnalyzer.intc);
        }
        UnlabeledStatement();
    }

    // <непомеченный оператор> ::= <простой оператор> | <сложный оператор>
    void UnlabeledStatement()
    {
        if (symbol == LexicalAnalyzer.beginsy || symbol == LexicalAnalyzer.ifsy || 
            symbol == LexicalAnalyzer.casesy || symbol == LexicalAnalyzer.whilesy || 
            symbol == LexicalAnalyzer.repeatsy || symbol == LexicalAnalyzer.forsy || 
            symbol == LexicalAnalyzer.withsy)
        {
            StructuredStatement(); // <сложный оператор>
        }
        else
        {
            SimpleStatement(); // <простой оператор>
        }
    }

    // <простой оператор> ::= <оператор присваивания> | <оператор процедуры> | <оператор перехода> | <пустой оператор>
    void SimpleStatement()
    {
        if (symbol == LexicalAnalyzer.ident)
        {
            accept(LexicalAnalyzer.ident);
            if (symbol == LexicalAnalyzer.assign) // Assignment
            {
                accept(LexicalAnalyzer.assign);
                Expression();
            }
            else if (symbol == LexicalAnalyzer.leftpar) // Procedure/function call with params
            {
                accept(LexicalAnalyzer.leftpar);
                ActualParameters();
                accept(LexicalAnalyzer.rightpar);
            }
            // Else it's just a procedure/function call without params
        }
        else if (symbol == LexicalAnalyzer.gotosy) // Goto statement
        {
            accept(LexicalAnalyzer.gotosy);
            accept(LexicalAnalyzer.intc); // <метка>
        }
        // Else it's an empty statement (do nothing)
    }

    // <оператор присваивания> ::= <переменная> := <выражение> | <имя функции> := <выражение>
    void AssignmentStatement()
    {
        Variable();
        accept(LexicalAnalyzer.assign);
        Expression();
    }

    // <переменная> ::= <полная переменная> | <компонента переменной> | <указанная переменная>
    void Variable()
    {
        accept(LexicalAnalyzer.ident); // <имя переменной>
        
        while (true)
        {
            if (symbol == LexicalAnalyzer.lbracket) // Indexed variable
            {
                accept(LexicalAnalyzer.lbracket);
                Expression();
                while (symbol == LexicalAnalyzer.comma)
                {
                    accept(LexicalAnalyzer.comma);
                    Expression();
                }
                accept(LexicalAnalyzer.rbracket);
            }
            else if (symbol == LexicalAnalyzer.point) // Field designation
            {
                accept(LexicalAnalyzer.point);
                accept(LexicalAnalyzer.ident); // <имя поля>
            }
            else if (symbol == LexicalAnalyzer.arrow) // File buffer or pointer dereference
            {
                accept(LexicalAnalyzer.arrow);
            }
            else
            {
                break;
            }
        }
    }

    // <выражение> ::= <простое выражение> | <простое выражение> <операция отношения> <простое выражение>
    void Expression()
    {
        SimpleExpression();
        
        if (symbol == LexicalAnalyzer.equal || symbol == LexicalAnalyzer.latergreater || 
            symbol == LexicalAnalyzer.later || symbol == LexicalAnalyzer.laterequal || 
            symbol == LexicalAnalyzer.greater || symbol == LexicalAnalyzer.greaterequal || 
            symbol == LexicalAnalyzer.insy)
        {
            // <операция отношения>
            accept(symbol);
            SimpleExpression();
        }
    }

    // <простое выражение> ::= <знак> <слагаемое> { <аддитивная операция> <слагаемое>}
    void SimpleExpression()
    {
        if (symbol == LexicalAnalyzer.plus || symbol == LexicalAnalyzer.minus)
        {
            accept(symbol); // <знак>
        }
        
        Term(); // <слагаемое>
        
        while (symbol == LexicalAnalyzer.plus || symbol == LexicalAnalyzer.minus || symbol == LexicalAnalyzer.orsy)
        {
            accept(symbol); // <аддитивная операция>
            Term(); // <слагаемое>
        }
    }

    // <слагаемое> ::= <множитель> { <мультипликативная операция> <множитель>}
    void Term()
    {
        Factor(); // <множитель>
        
        while (symbol == LexicalAnalyzer.star || symbol == LexicalAnalyzer.slash || 
               symbol == LexicalAnalyzer.divsy || symbol == LexicalAnalyzer.modsy || 
               symbol == LexicalAnalyzer.andsy)
        {
            accept(symbol); // <мультипликативная операция>
            Factor(); // <множитель>
        }
    }

    // <множитель> ::= <переменная> | <константа без знака> | (<выражение>) | <обозначение функции> | <множество> | not <множитель>
    void Factor()
    {
        if (symbol == LexicalAnalyzer.ident)
        {
            accept(LexicalAnalyzer.ident);
            
            if (symbol == LexicalAnalyzer.leftpar) // Function call
            {
                accept(LexicalAnalyzer.leftpar);
                ActualParameters();
                accept(LexicalAnalyzer.rightpar);
            }
            // Else it's a variable
        }
        else if (symbol == LexicalAnalyzer.intc || symbol == LexicalAnalyzer.floatc || 
                 symbol == LexicalAnalyzer.charc || symbol == LexicalAnalyzer.stringc || 
                 symbol == LexicalAnalyzer.ident) // Constant
        {
            accept(symbol);
        }
        else if (symbol == LexicalAnalyzer.leftpar) // Parenthesized expression
        {
            accept(LexicalAnalyzer.leftpar);
            Expression();
            accept(LexicalAnalyzer.rightpar);
        }
        else if (symbol == LexicalAnalyzer.lbracket) // Set
        {
            Set();
        }
        else if (symbol == LexicalAnalyzer.notsy) // Not
        {
            accept(LexicalAnalyzer.notsy);
            Factor();
        }
        else
        {
            io.Error(symbol, io.CurrentPosition);
        }
    }

    // <множество> ::= [<список элементов>]
    void Set()
    {
        accept(LexicalAnalyzer.lbracket);
        ElementList();
        accept(LexicalAnalyzer.rbracket);
    }

    // <список элементов> ::= <элемент> {, <элемент>} | <пусто>
    void ElementList()
    {
        if (symbol != LexicalAnalyzer.rbracket) // Not empty
        {
            Element();
            while (symbol == LexicalAnalyzer.comma)
            {
                accept(LexicalAnalyzer.comma);
                Element();
            }
        }
    }

    // <элемент> ::= <выражение> | <выражение> .. <выражение>
    void Element()
    {
        Expression();
        if (symbol == LexicalAnalyzer.twopoints)
        {
            accept(LexicalAnalyzer.twopoints);
            Expression();
        }
    }

    // <фактический параметр> ::= <выражение> | <переменная> | <имя процедуры> | <имя функции>
    void ActualParameter()
    {
        if (symbol == LexicalAnalyzer.ident)
        {
            accept(LexicalAnalyzer.ident);
            
            // We can't always distinguish these cases syntactically
        }
        else
        {
            Expression();
        }
    }

    // <фактический параметр> {, <фактический параметр>}
    void ActualParameters()
    {
        ActualParameter();
        while (symbol == LexicalAnalyzer.comma)
        {
            accept(LexicalAnalyzer.comma);
            ActualParameter();
        }
    }

    // <сложный оператор> ::= <составной оператор> | <выбирающий оператор> | <оператор цикла> | <оператор присоединения>
    void StructuredStatement()
    {
        switch (symbol)
        {
            case LexicalAnalyzer.beginsy:
                CompoundStatement();
                break;
            case LexicalAnalyzer.ifsy:
                IfStatement();
                break;
            case LexicalAnalyzer.casesy:
                CaseStatement();
                break;
            case LexicalAnalyzer.whilesy:
                WhileStatement();
                break;
            case LexicalAnalyzer.repeatsy:
                RepeatStatement();
                break;
            case LexicalAnalyzer.forsy:
                ForStatement();
                break;
            case LexicalAnalyzer.withsy:
                WithStatement();
                break;
            default:
                io.Error(symbol, io.CurrentPosition);
                break;
        }
    }

    // <составной оператор> ::= begin <оператор> {; <оператор>} end
    void CompoundStatement()
    {
        accept(LexicalAnalyzer.beginsy);
        Statement();
        while (symbol == LexicalAnalyzer.semicolon)
        {
            accept(LexicalAnalyzer.semicolon);
            if (symbol != LexicalAnalyzer.endsy) // Allow empty statements before end
            {
                Statement();
            }
        }
        accept(LexicalAnalyzer.endsy);
    }

    // <условный оператор> ::= if <выражение> then <оператор> | if <выражение> then <оператор> else <оператор>
    void IfStatement()
    {
        accept(LexicalAnalyzer.ifsy);
        Expression();
        accept(LexicalAnalyzer.thensy);
        Statement();
        if (symbol == LexicalAnalyzer.elsesy)
        {
            accept(LexicalAnalyzer.elsesy);
            Statement();
        }
    }

    // <оператор варианта> ::= case <выражение> of <элемент списка вариантов> {; <элемент списка вариантов>} end
    void CaseStatement()
    {
        accept(LexicalAnalyzer.casesy);
        Expression();
        accept(LexicalAnalyzer.ofsy);
        CaseElement();
        while (symbol == LexicalAnalyzer.semicolon)
        {
            accept(LexicalAnalyzer.semicolon);
            CaseElement();
        }
        accept(LexicalAnalyzer.endsy);
    }

    // <элемент списка вариантов> ::= <список меток варианта> : <оператор> | <пусто>
    void CaseElement()
    {
        if (symbol != LexicalAnalyzer.semicolon && symbol != LexicalAnalyzer.endsy)
        {
            CaseLabelList();
            accept(LexicalAnalyzer.colon);
            Statement();
        }
    }

    // <список меток варианта> ::= <метка варианта> {, <метка варианта>}
    void CaseLabelList()
    {
        CaseLabel();
        while (symbol == LexicalAnalyzer.comma)
        {
            accept(LexicalAnalyzer.comma);
            CaseLabel();
        }
    }

    // <метка варианта> ::= <константа>
    void CaseLabel()
    {
        Const();
    }

    // <цикл с предусловием> ::= while <выражение> do <оператор>
    void WhileStatement()
    {
        accept(LexicalAnalyzer.whilesy);
        Expression();
        accept(LexicalAnalyzer.dosy);
        Statement();
    }

    // <цикл с постусловием> ::= repeat <оператор> {; <оператор>} until <выражение>
    void RepeatStatement()
    {
        accept(LexicalAnalyzer.repeatsy);
        Statement();
        while (symbol == LexicalAnalyzer.semicolon)
        {
            accept(LexicalAnalyzer.semicolon);
            Statement();
        }
        accept(LexicalAnalyzer.untilsy);
        Expression();
    }

    // <цикл с параметром> ::= for <параметр цикла> := <выражение> <направление> <выражение> do <оператор>
    void ForStatement()
    {
        accept(LexicalAnalyzer.forsy);
        accept(LexicalAnalyzer.ident); // <параметр цикла>
        accept(LexicalAnalyzer.assign);
        Expression();
        if (symbol == LexicalAnalyzer.tosy || symbol == LexicalAnalyzer.downtosy)
        {
            accept(symbol); // <направление>
        }
        else
        {
            io.Error(symbol, io.CurrentPosition);
        }
        Expression();
        accept(LexicalAnalyzer.dosy);
        Statement();
    }

    // <оператор присоединения> ::= with <список переменных-записей> do <оператор>
    void WithStatement()
    {
        accept(LexicalAnalyzer.withsy);
        RecordVariableList();
        accept(LexicalAnalyzer.dosy);
        Statement();
    }

    // <список переменных-записей> ::= <переменная-запись> {, <переменная-запись>}
    void RecordVariableList()
    {
        Variable(); // <переменная-запись>
        while (symbol == LexicalAnalyzer.comma)
        {
            accept(LexicalAnalyzer.comma);
            Variable();
        }
    }
    
    
    
}
