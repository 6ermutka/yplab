(*Получить список из минимальных цифр натуральных чисел, содержащихся в
исходной последовательности*)
open System
let rec check() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a > 0 -> a
    | _ ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
let p = lazy (
    printf "Введите кол-во элементов в исходной последовательности: ";
    let n = check();
    n
)
let rec fmin n cmin =
    if n = 0 then cmin
    else
        let d = n % 10
        if d < cmin then fmin (n / 10) d
        else fmin (n / 10) cmin
let minDigit n = fmin n 9
let x = seq { 
    for i in 1 .. p.Value do
        printf "Введите элемент %d: " i
        yield check() 
}
let c = Seq.map minDigit x
let v = Seq.toList c
printf "Последовательность минимальных цифр %A" v


        
        
        
        
            
            
            
    
