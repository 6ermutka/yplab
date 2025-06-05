public class SyntaxAnalyzer
{
    private IOHandler io;
    private LexicalAnalyzer lexer;
    private byte symbol;
    private TextPosition token;
    private readonly HashSet<byte> begPart; // label, const, type, var, function, procedure, begin
    private readonly HashSet<byte> stTypePart; // type, var, function, procedure, begin
    private readonly HashSet<byte> stVarPart; // var, function, procedure, begin
    private readonly HashSet<byte> stProcFuncPart; // function, procedure, begin
    private readonly HashSet<byte> idStarters; // ident
    private readonly HashSet<byte> afterVar; // semicolon
    private readonly HashSet<byte> stmtStarters; // begin, ident
    private readonly HashSet<byte> exprStarters; // ident, number, leftpar, not, plus, minus
    public SyntaxAnalyzer(IOHandler ioHandler, LexicalAnalyzer lexerHandler)
    {
        io = ioHandler;
        lexer = lexerHandler;
        symbol = lexer.NextSym();
        begPart = new HashSet<byte>
        {
            LexicalAnalyzer.labelsy, LexicalAnalyzer.constsy,
            LexicalAnalyzer.typesy, LexicalAnalyzer.varsy,
            LexicalAnalyzer.functionsy, LexicalAnalyzer.procedurensy,
            LexicalAnalyzer.beginsy
        };
        stTypePart = new HashSet<byte>
        {
            LexicalAnalyzer.typesy, LexicalAnalyzer.varsy,
            LexicalAnalyzer.functionsy, LexicalAnalyzer.procedurensy,
            LexicalAnalyzer.beginsy
        };
        stVarPart = new HashSet<byte>
        {
            LexicalAnalyzer.varsy,
            LexicalAnalyzer.functionsy, LexicalAnalyzer.procedurensy,
            LexicalAnalyzer.beginsy
        };
        stProcFuncPart = new HashSet<byte>
        {
            LexicalAnalyzer.functionsy, LexicalAnalyzer.procedurensy,
            LexicalAnalyzer.beginsy
        };
        idStarters = new HashSet<byte> { LexicalAnalyzer.ident };
        afterVar = new HashSet<byte> { LexicalAnalyzer.semicolon };
        stmtStarters = new HashSet<byte>
        {
            LexicalAnalyzer.beginsy, LexicalAnalyzer.ident
        };
        exprStarters = new HashSet<byte>
        {
            LexicalAnalyzer.ident, LexicalAnalyzer.intc, LexicalAnalyzer.floatc,
            LexicalAnalyzer.leftpar, LexicalAnalyzer.notsy,
            LexicalAnalyzer.plus, LexicalAnalyzer.minus
        };
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
    private void Accept(byte expectedSymbol, HashSet<byte> followers)
    {
        if (symbol == expectedSymbol)
        {
            symbol = lexer.NextSym();
        }
        else
        {
            io.Error(expectedSymbol, io.CurrentPosition);
            SkipTo2(new HashSet<byte> { expectedSymbol }, followers);
            if (symbol == expectedSymbol)
            {
                symbol = lexer.NextSym();
            }
        }
    }
    private void SkipTo(HashSet<byte> recoverySet)
    {
        while (!recoverySet.Contains(symbol))
        {
            symbol = lexer.NextSym();
        }
    }
    private void SkipTo2(HashSet<byte> starters, HashSet<byte> followers)
    {
        var combined = new HashSet<byte>(starters);
        combined.UnionWith(followers);
        SkipTo(combined);
    }
    private bool BelongsTo(byte sym, HashSet<byte> set)
    {
        return set.Contains(sym);
    }
    //<программа> ::= program <имя> (<имя файла> {, <имя файла>}); <блок>
    public void Program()
    {
        var followers = new HashSet<byte> { LexicalAnalyzer.point };
        accept(LexicalAnalyzer.programsy); // 'program'
        accept(LexicalAnalyzer.ident); // '<имя>'
        if (symbol == LexicalAnalyzer.leftpar)
        {
            accept(LexicalAnalyzer.leftpar); // '('
            while (symbol != LexicalAnalyzer.rightpar)
            {
                accept(LexicalAnalyzer.ident); // '<имя файла>'
                if (symbol == LexicalAnalyzer.rightpar)
                {
                    accept(LexicalAnalyzer.rightpar); // ')'
                    break;
                }

                accept(LexicalAnalyzer.comma); // ','
            }
        }

        accept(LexicalAnalyzer.semicolon); // ';'
        Block(followers); // '<блок>'
    }
    private void Block(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, begPart))
        {
            io.Error(18, io.CurrentPosition);
            SkipTo2(begPart, followers);
        }
    
        if (BelongsTo(symbol, stTypePart))
        {
            var varFollowers = new HashSet<byte>(stVarPart);
            varFollowers.UnionWith(followers);
            TypePart(varFollowers);
        }
    
        if (BelongsTo(symbol, stVarPart))
        {
            var procFuncFollowers = new HashSet<byte>(stProcFuncPart);
            procFuncFollowers.UnionWith(followers);
            VarPart(procFuncFollowers);
        }
    
        // Обрабатываем составной оператор
        if (symbol == LexicalAnalyzer.beginsy)
        {
            CompoundStatement(followers);
        }
    
        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition); 
            SkipTo(followers);
        }
    }
    // <раздел типов> ::= type <описание типа>; {<описание типа>;}
    private void TypePart(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, stTypePart))
        {
            io.Error(18, io.CurrentPosition);
            SkipTo2(stTypePart, followers);
        }

        if (symbol == LexicalAnalyzer.typesy)
        {
            Accept(LexicalAnalyzer.typesy, followers);

            var typeDeclFollowers = new HashSet<byte>(afterVar);
            typeDeclFollowers.UnionWith(followers);

            do
            {
                TypeDescription(typeDeclFollowers);
                Accept(LexicalAnalyzer.semicolon, followers);
            } while (symbol == LexicalAnalyzer.ident);
        }

        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <описание типа> ::= <имя> = <тип>
    private void TypeDescription(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, idStarters))
        {
            io.Error(2, io.CurrentPosition);
            SkipTo2(idStarters, followers);
        }

        if (symbol == LexicalAnalyzer.ident)
        {
            Accept(LexicalAnalyzer.ident, followers);
            Accept(LexicalAnalyzer.equal, followers);
            Type(followers);
        }

        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <тип> ::= <стандартный тип> | <тип записи>
    private void Type(HashSet<byte> followers)
    {
        if (symbol == LexicalAnalyzer.recordsy)
        {
            RecordType(followers);
        }
        else
        {
            StandardType(followers);
        }
    }
    // <тип записи> ::= record <список полей> end
    private void RecordType(HashSet<byte> followers)
    {
        Accept(LexicalAnalyzer.recordsy, followers);

        var fieldListFollowers = new HashSet<byte> { LexicalAnalyzer.endsy };
        fieldListFollowers.UnionWith(followers);

        FieldList(fieldListFollowers);
        Accept(LexicalAnalyzer.endsy, followers);
    }
    // <список полей> ::= <описание однотипных переменных>; {<описание однотипных переменных>;}
    private void FieldList(HashSet<byte> followers)
    {
        var fieldFollowers = new HashSet<byte>(afterVar);
        fieldFollowers.UnionWith(followers);

        while (true)
        {
            if (!BelongsTo(symbol, idStarters) && !followers.Contains(symbol))
            {
                io.Error(2, io.CurrentPosition);
                SkipTo2(idStarters, followers);
                if (followers.Contains(symbol)) break;
            }
            if (symbol == LexicalAnalyzer.ident)
            {
                OneTypeVarDescription(fieldFollowers);
                Accept(LexicalAnalyzer.semicolon, followers);
            }
            
            if (symbol == LexicalAnalyzer.endsy || followers.Contains(symbol))
                break;
        }

        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <стандартный тип> ::= integer | real | boolean | char | string
    private void StandardType(HashSet<byte> followers)
    {
        if (symbol == LexicalAnalyzer.integersy ||
            symbol == LexicalAnalyzer.realsy ||
            symbol == LexicalAnalyzer.boolsy ||
            symbol == LexicalAnalyzer.charsy ||
            symbol == LexicalAnalyzer.stringsy)
        {
            Accept(symbol, followers);
        }
        else
        {
            io.Error(19, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <раздел переменных> ::= var <описание однотипных переменных> ; {<описание однотипных переменных>;}
    private void VarPart(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, stVarPart))
        {
            io.Error(18, io.CurrentPosition);
            SkipTo2(stVarPart, followers);
        }
        if (symbol == LexicalAnalyzer.varsy)
        {
            Accept(LexicalAnalyzer.varsy, followers);

            var varDeclFollowers = new HashSet<byte>(afterVar);
            varDeclFollowers.UnionWith(followers);

            do
            {
                OneTypeVarDescription(varDeclFollowers);
                Accept(LexicalAnalyzer.semicolon, followers);
            } while (symbol == LexicalAnalyzer.ident);
        }

        if (!BelongsTo(symbol, followers))
        {
            io.Error(2, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <описание однотипных переменных> ::= <имя> {, <имя>} : <тип>
    private void OneTypeVarDescription(HashSet<byte> followers)
    {
        while (symbol != LexicalAnalyzer.colon && !followers.Contains(symbol))
        {
            if (symbol == LexicalAnalyzer.ident)
            {
                Accept(LexicalAnalyzer.ident, followers);

                while (symbol == LexicalAnalyzer.comma)
                {
                    Accept(LexicalAnalyzer.comma, followers);
                    Accept(LexicalAnalyzer.ident, followers);
                }
                break;
            }
            else
            {
                io.Error(2, io.CurrentPosition);
                symbol = lexer.NextSym();
            }
        }
        Accept(LexicalAnalyzer.colon, followers);
        Type(followers);
    }
    // <оператор> ::= <составной оператор> | <оператор присваивания>
    private void Statement(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, stmtStarters))
        {
            io.Error(22, io.CurrentPosition); // Ожидается оператор
            SkipTo2(stmtStarters, followers);
        }

        if (symbol == LexicalAnalyzer.beginsy)
        {
            CompoundStatement(followers);
        }
        else if (symbol == LexicalAnalyzer.ident)
        {
            AssignmentStatement(followers);
        }
        else
        {
            io.Error(22, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <составной оператор> ::= begin <оператор> {; <оператор>} end
    private void CompoundStatement(HashSet<byte> followers)
    {
        Accept(LexicalAnalyzer.beginsy, followers);

        var stmtFollowers = new HashSet<byte> { LexicalAnalyzer.semicolon, LexicalAnalyzer.endsy };
        stmtFollowers.UnionWith(followers);

        Statement(stmtFollowers);

        while (symbol == LexicalAnalyzer.semicolon)
        {
            Accept(LexicalAnalyzer.semicolon, followers);
            
            if (symbol != LexicalAnalyzer.endsy)
            {
                Statement(stmtFollowers);
            }
        }
        Accept(LexicalAnalyzer.endsy, followers);
    }
    // <оператор присваивания> ::= <переменная> := <выражение>
    private void AssignmentStatement(HashSet<byte> followers)
    {
        Accept(LexicalAnalyzer.ident, followers); // переменная
        Accept(LexicalAnalyzer.assign, followers); // :=

        Expression(followers);

        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition);
            SkipTo(followers);
        }
    }
    // <выражение> ::= <простое выражение> [<знак отношения> <простое выражение>]
    private void Expression(HashSet<byte> followers)
    {
        if (!BelongsTo(symbol, exprStarters))
        {
            io.Error(23, io.CurrentPosition); 
            SkipTo2(exprStarters, followers);
        }

        if (BelongsTo(symbol, exprStarters))
        {
            SimpleExpression(followers);
            if (symbol == LexicalAnalyzer.equal ||
                symbol == LexicalAnalyzer.latergreater ||
                symbol == LexicalAnalyzer.later ||
                symbol == LexicalAnalyzer.laterequal ||
                symbol == LexicalAnalyzer.greater ||
                symbol == LexicalAnalyzer.greaterequal)
            {
                Accept(symbol, followers);
                SimpleExpression(followers);
            }
        }

        if (!BelongsTo(symbol, followers))
        {
            io.Error(6, io.CurrentPosition);
            SkipTo(followers);
        }
    }

    // <простое выражение> ::= [<знак>] <слагаемое> {<знак сложения> <слагаемое>}
    private void SimpleExpression(HashSet<byte> followers)
    {
        if (symbol == LexicalAnalyzer.plus || symbol == LexicalAnalyzer.minus)
        {
            Accept(symbol, followers);
        }
        Term(followers);
        while (symbol == LexicalAnalyzer.plus ||
               symbol == LexicalAnalyzer.minus ||
               symbol == LexicalAnalyzer.orsy)
        {
            Accept(symbol, followers);
            Term(followers);
        }
    }

    // <слагаемое> ::= <множитель> {<знак умножения> <множитель>}
    private void Term(HashSet<byte> followers)
    {
        Factor(followers);
        while (symbol == LexicalAnalyzer.star ||
               symbol == LexicalAnalyzer.slash ||
               symbol == LexicalAnalyzer.divsy ||
               symbol == LexicalAnalyzer.modsy ||
               symbol == LexicalAnalyzer.andsy)
        {
            Accept(symbol, followers);
            Factor(followers);
        }
    }

    // <множитель> ::= <переменная> | <число> | (<выражение>) | not <множитель>
    private void Factor(HashSet<byte> followers)
    {
        if (symbol == LexicalAnalyzer.ident)
        {
            Accept(LexicalAnalyzer.ident, followers);
        }
        else if (symbol == LexicalAnalyzer.intc || symbol == LexicalAnalyzer.floatc)
        {
            Accept(symbol, followers);
        }
        else if (symbol == LexicalAnalyzer.leftpar)
        {
            Accept(LexicalAnalyzer.leftpar, followers);
            var exprFollowers = new HashSet<byte> { LexicalAnalyzer.rightpar };
            exprFollowers.UnionWith(followers);
            Expression(exprFollowers);
            Accept(LexicalAnalyzer.rightpar, followers);
        }
        else if (symbol == LexicalAnalyzer.notsy)
        {
            Accept(LexicalAnalyzer.notsy, followers);
            Factor(followers);
        }
        else
        {
            io.Error(24, io.CurrentPosition);
            SkipTo(followers);
        }
    }
}

