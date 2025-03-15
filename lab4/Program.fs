(*Дерево содержит строки. Заменить в каждой строке последний символ на
заданный.*)
open System
type Tree =
    | Leaf of string
    | Node of string * Tree list

let rec check() =
    let input = Console.ReadLine()
    match Int32.TryParse(input) with
    | (true, a) when a > 0 -> a
    | _ ->
        printfn "Вы ввели не натуральное число. Попробуйте еще раз:"
        check()
let rec checkString () =
    let input = Console.ReadLine()
    match Int32.TryParse(input), Double.TryParse(input) with
    | (true, _), _ -> 
        printfn "Пожалуйста, введите строку:"
        checkString()
    | (false, _), (true, _) ->
        printfn "Пожалуйста, введите строку:"
        checkString()
    | (false, _), (false, _) ->
        input
let rec inputTree () =
    printfn "Введите 'leaf' для создания листа или 'node' для создания узла:"
    let input = System.Console.ReadLine()
    match input.ToLower() with
    | "leaf" ->
        printfn "Введите значение для листа:"
        let value = checkString()
        Leaf value
    | "node" ->
        printfn "Введите название узла:"
        let name = checkString()
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
        printfn "%s- %s" indent value
    | Node (name, children) ->
        printfn "%s%s -|" indent name
        children |> List.iter (fun child -> visualizeTree child (indent + "  "))
let replaceLastChar (str: string) (newChar: char) =
    if str.Length > 0 then
        str.Substring(0, str.Length - 1) + newChar.ToString()
    else
        str
let rec mapTree f tree =
    match tree with
    | Leaf value -> Leaf (f value)
    | Node (name, children) -> Node (f name, List.map (mapTree f) children)
let tree = inputTree()
printfn "Исходное дерево:"
visualizeTree tree ""
printfn "Введите символ для замены последнего символа в каждой строке:"
let newChar = checkString().[0]
let newTree = mapTree (fun str -> replaceLastChar str newChar) tree
printfn "Новое дерево:"
visualizeTree newTree ""

