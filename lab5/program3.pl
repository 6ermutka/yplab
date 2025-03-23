% Определим множество как список без повторяющихся элементов. Найти
% дополнение множества.

% предикат для запуска программы
start :-
    write('Введите универсальное множество U [элемент1, элемент2, ...]: '), nl,
    read(U),
    write('Введите множество A [элемент1, элемент2, ...]: '), nl,
    read(A),
    complement(U, A, Complement),
    write('Дополнение множества A: '), write(Complement), nl.
member(X, [X|_]).
member(X, [_|T]) :- member(X, T).
% предикат для нахождения дополнения множества
complement(U, A, Complement) :-
    findall(X, (member(X, U), \+ member(X, A)), Complement).
