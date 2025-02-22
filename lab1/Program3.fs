//Найти первую цифру натурального числа.
open System

let rec check() =
    let input = Console.ReadLine()
    match Double.TryParse(input) with
    | (true, a) -> a
    | (false, _) ->
        printfn "Вы ввели не действительно число. Попробуйте еще раз:"
        check()

printfn "Комплексное число вида: Z = X + Yi "
printf "Введите первое действительное число(Первого комплексного числа) X1: "
let X1 = check()
printf "Введите второе действительное число(Первого комплексного числа) Y1: "
let Y1 = check()
printf "Введите первое действительное число(Первого комплексного числа) X2: "
let X2 = check()
printf "Введите второе действительное число(Первого комплексного числа) Y2: "
let Y2 = check()
printf "Введите степень n: "
let n = int(Console.ReadLine())

let sum(x1:float) (y1:float) (x2:float) (y2:float) =
    printfn "Сумма = %f + %fi" (x1+x2) (y1+y2)

let raz (x1:float) (y1:float) (x2:float) (y2:float) =
    printfn "Разность = %f + %fi" (x1-x2) (y1-y2)

let umn (x1:float) (y1:float) (x2:float) (y2:float)=
    printfn "Умножение = %f + %fi" (x1*x2 - y1*y2) (x1*y2 + y1*x2)

let del (x1:float) (y1:float) (x2:float) (y2:float)=
    printfn "Деление = %f + %fi" ( (x1*x2 + y1*y2)/(x2*x2 + y2*y2) ) ( (x2*y1 - x1*y2)/(x2*x2 + y2*y2) ) 

let power (x: float) (y: float) (n: int) =
    let r = sqrt(x*x + y*y)
    let theta = Math.Atan2(y, x)
    let rn = Math.Pow(r, float n)
    let nTheta = float n * theta
    (rn * cos nTheta, rn * sin nTheta)

sum X1 Y1 X2 Y2
raz X1 Y1 X2 Y2
umn X1 Y1 X2 Y2
if X2 <> 0 && Y2 <> 0 then del X1 Y1 X2 Y2
else printfn "Деление на 0 невозможно"
let (resultX1, resultY1) = power X1 Y1 n
printfn "Первое комплексное число %f + %fi в степени %i = %f + %fi" X1 Y1 n resultX1 resultY1
let (resultX2, resultY2) = power X2 Y2 n
printfn "Первое комплексное число %f + %fi в степени %i = %f + %fi" X2 Y2 n resultX2 resultY2






    