// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module QuanTemplateTest.Main

open System

open QuanTemplateTest.ArrayBasedOrderBook

[<EntryPoint>]
let main argv = 
    try
        let fileName = argv.[0]
        let tickSize = Convert.ToDecimal(argv.[1])
        let bookDepth = Convert.ToInt32(argv.[2])

        let orderBookChanges = IO.File.ReadLines(fileName)

        let orderBook = new ArrayBasedOrderBook(tickSize, bookDepth)
        orderBook.ProcessChanges (orderBookChanges)
        orderBook.Print ()
    with
        | :? System.IO.FileNotFoundException as fnfex -> printfn "Filename \"%s\" was not found." argv.[0]
        | :? System.Exception as ex -> printfn "%s" ex.Message

    0 // return an integer exit code
