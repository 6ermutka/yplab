program LexerTest;

type
  TArray = array[1..10] of Integer;
  TRecord = record
    Field1: Integer;
    Field2: Char;
  end;
  TSet = set of 'A'..'Z';

var
  i, j: Integer;
  x, y: Real;
  ch: Char;
  str: String;

const
  MAX = 100;
  MESSAGE = 'Hello, Lexer!';

procedure TestProc(param1: Integer; param2: Char);
begin
  if param1 > MAX then
    writeln('Greater than max')
  else
    writeln('Less or equal');
end;

function TestFunc(a, b: Integer): Integer;
begin
  Result := a + b;
end;

begin
  { Все ключевые слова }
  program LexerTest;
  type TType = Integer;
  var k: Integer;
  const PI = 3.14;
  begin
    for i := 1 to 10 do
    begin
      while j < 20 do
      begin
        repeat
          if i mod 2 = 0 then
            j := j + 1
          else if i > 5 then
            j := j + 2
          else
            j := j - 1;
          case j of
            10: writeln('Ten');
            20: writeln('Twenty');
            else writeln('Other');
          end;
        until j >= 30;
      end;
    end;

    with rec do
    begin
      Field1 := 42;
      Field2 := 'X';
    end;

    goto label1;
    label1:
    
    arr[1] := 100;
    p := @i;
    s := ['A', 'B', 'C'];
    
    try
      x := 1.7e38;
      y := -2.9e-39;
    except
      on E: Exception do
        writeln('Error');
    end;
    
    { Все операторы и символы }
    i := 10 + 5;
    x := y * 2.5;
    ch := #65;
    str := 'Test string '' with escaped quote';
    flag := (i < 20) and (j > 0) or (x <> y);
    
    if i <= 10 then
      j := 20
    else if i >= 15 then
      j := 30;
      
    for k := 1 downto 0 do
      writeln(k);
      
    while not flag do
    begin
      flag := True;
    end;
    
    TestProc(i, 'A');
    j := TestFunc(1, 2);
    
    { Все специальные символы }
    i := 10 * (5 + 3) - 2 / 4;
    x := i mod 3;
    arr[1] := arr[2] + arr[3];
    p^ := 100;
    
    { Строки и символы }
    ch := '''';
    str := 'This is a ''test'' string';
    str := '';
    
    { Числовые литералы }
    i := 32767;
    x := 3.14159;
    x := 1.7e+38;
    x := 2.9e-39;
    
    { Комментарии }
    
    { Все разделители }
    [i, j, k];
    (x, y);
    i := 10;
    j := 20;
    k := 30;
    
    { Ниль }
    p := nil;
  end;
end.