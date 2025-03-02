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
let rec fmin n cmin =
    if n = 0 then cmin
    else
        let d = n%10
        if d < cmin then fmin (n/10) d
        else fmin (n/10) cmin
let minDigit n = fmin n 9
printf "Введите кол-во элементов в исходном списке: "
let n= check()
let x = [ for i in 1 .. n do
              printf "Введите элемент %d: " i
              yield check() ]
let с = List.map minDigit x
printfn "Список минимальных цифр: %A" с
    


        
        
        
            
            
            
    
