open System
open System.IO

let rec getAllFiles directory =
    seq {
        yield! Directory.GetFiles(directory)
        for subDirectory in Directory.GetDirectories(directory) do
            yield! getAllFiles subDirectory
    }
printf "Введите путь к каталогу: "
let directoryPath = Console.ReadLine()
let files = lazy (getAllFiles directoryPath)
printfn "Получаю файлы из католога %s" directoryPath
if Seq.isEmpty files.Value then 
    printfn "В каталоге нет файлов."
else
    let shortestFileName = files.Value |> Seq.minBy (fun file -> Path.GetFileName(file).Length)
    printfn "Самое короткое название файла: %s" (Path.GetFileName(shortestFileName))
    printfn "Путь до этого файла: %s" (shortestFileName)
            
    
