% предикат для запуска программы
start :-
    write('Введите список целых чисел в формате [1, 2, -3, 4, 5]: '),
    read(List),
    max_positive_sequence(List, 0, 0, Max), % Вычисление максимального количества положительных чисел
    write('Максимальное количество подряд идущих положительных чисел: '),
    write(Max), nl.
% предикат на проверку неотрицательности
is_positive(X) :- X > 0.
% предикат если положительный элемент
max_positive_sequence([H|T], CurrentCount, CurrentMax, Max) :-
    is_positive(H),
    NewCount is CurrentCount + 1,
    (NewCount > CurrentMax -> NewMax = NewCount; NewMax = CurrentMax),
    max_positive_sequence(T, NewCount, NewMax, Max).
% предикат если отрицательный элемент
max_positive_sequence([H|T], _, CurrentMax, Max) :-
    \+ is_positive(H),
    max_positive_sequence(T, 0, CurrentMax, Max).
% базис рекурсии
max_positive_sequence([], _, CurrentMax, CurrentMax).
