//Найти первую цифру натурального числа.
open System

let rec check() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a > 0 -> a
    | (false, _) ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
    | (_, _) ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
printf "Введите натуральное число: "
let y = check()
let rec ff y=
    if y < 10 then (y%10)
    else ff (y/10) 
printf "Первая цифра %i = %i " (y) (ff y)
    
