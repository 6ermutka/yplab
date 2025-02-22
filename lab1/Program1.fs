//Сформировать список из степеней числа 2.
open System

printf "Введите максимальную степень двойки): "
let y = int(Console.ReadLine())
let rec db x i =
    if i = 0 then 1
    else 2 * db (2) (i-1)
let x =
    [ for i in 0 .. y do
        yield db 2 i ]
printfn "Список из степеней числа 2: %A" x