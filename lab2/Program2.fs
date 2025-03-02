(*Найти количество элементов списка, в которых встречается заданная цифра.*)
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
let rec checkD() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a >= 0 && a <= 9 -> a
    | _ ->
        printfn "Вы ввели не цифру. Попробуйте еще раз:"
        checkD()
printf "Введите цифру: "
let x = checkD()
printf "Введите кол-во элементов: "
let n = check()
let rec kk d num =
    if num = 0 then false
    else
        if num%10 = d then true
        else kk d (num/10)
    
let k = [ for i in 1 .. n do
              printf "Введите элемент %d: " i
              yield check() ]
let count = List.fold (fun acc elem -> if kk x elem then acc + 1 else acc) 0 k
printfn "Количество элементов, содержащих цифру %d: %d" x count
    


        
        
        
            
            
            
    
