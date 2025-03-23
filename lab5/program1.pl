% Составить программу нахождения числа, которое образуется из данного натурального числа при записи его цифр в обратном порядке. 
% Например, для числа 1234 получаем ответ 4321.

% предикат для запуска программы
start :-
    write('Введите натуральное число: '),
    read(N),
    reverse_number(N, 0, Reversed),
    write('Перевернутое число: '), 
    write(Reversed), 
    nl.
% базис рекурсии
reverse_number(0, Acc, Acc).
% рекурсия 
reverse_number(N, Acc, Reversed) :-
    N > 0,
    LastDigit is N mod 10,          
    NewAcc is Acc * 10 + LastDigit, 
    NewN is N // 10,                
    reverse_number(NewN, NewAcc, Reversed).
