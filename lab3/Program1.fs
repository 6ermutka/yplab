(*Найти количество элементов последовательности, в которых встречается заданная цифра.*)
open System
let rec check() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a > 0 -> a
    | _ ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
let rec checkD() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a >= 0 && a <= 9 -> a
    | _ ->
        printfn "Вы ввели не цифру. Попробуйте еще раз:"
        checkD()
let p = lazy (
    printf "Введите цифру: ";
    let x = checkD();
    x;
)
let p1 = lazy (
    printf "Введите кол-во элементов: ";
    let n = check();
    n;
)
let x = p.Value
let n = p1.Value
let rec containsDigit d num =
    if num = 0 then false
    else
        if num % 10 = d then true
        else containsDigit d (num / 10)
let sequence = seq {
    for i in 1 .. n do
        printf "Введите элемент %d: " i
        yield check()
}
let count = Seq.fold (fun acc elem -> if containsDigit x elem then acc + 1 else acc) 0 sequence
printfn "Количество элементов, содержащих цифру %d: %d" x count
        
        
        
        
            
            
            
    
