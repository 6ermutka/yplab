(*Найти сумму четных значений, находящихся в листьях (узел является листом,
если у него нет ни левого, ни правого поддерева).*)
open System
type Tree =
    | Leaf of int
    | Node of int * Tree list
let rec check() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a > 0 -> a
    | _ ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
let rec inputTree () =
    printfn "Введите 'leaf' для создания листа или 'node' для создания узла:"
    let input = System.Console.ReadLine()
    match input.ToLower() with
    | "leaf" ->
        printfn "Введите значение для листа:"
        let value = check()
        Leaf value
    | "node" ->
        printfn "Введите название узла:"
        let name = check()
        printfn "Введите количество дочерних элементов узла:"
        let count = check()
        let children = List.init count (fun _ -> inputTree())
        Node (name, children)
    | _ ->
        printfn "Неверный ввод. Попробуйте снова."
        inputTree()
let rec visualizeTree tree indent =
    match tree with
    | Leaf value ->
        printfn "%s- %i" indent value
    | Node (value, children) ->
        printfn "%s%i -|" indent value
        children |> List.iter (fun child -> visualizeTree child (indent + "  "))
let sumEvenLeaves tree =
    let rec foldTree acc = function
        | Leaf value -> if value % 2 = 0 then acc + value else acc
        | Node (_, children) -> List.fold foldTree acc children
    foldTree 0 tree
let tree = inputTree()
printfn "Визуализация дерева:"
visualizeTree tree ""
let result = sumEvenLeaves tree
printfn "Сумма четных значений в листьях: %d" result