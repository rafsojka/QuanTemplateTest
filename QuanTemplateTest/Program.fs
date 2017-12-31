// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module QuanTemplateTest.Main

open System

type Instruction = |   Update   |   Delete  |   New

type Side = |   Bid |   Ask

type OrderBookUpdate =
    {   Instruction     :   Instruction
        Side            :   Side
        PriceLevelIndex :   int
        Price           :   int
        Quantity        :   int
    }
    static member FromOrderBookUpdateFileLine (line:string) =
        let tokens = line.Split(' ')
        let instruction =
            match tokens.[0] with
            | "U" -> Update
            | "D" -> Delete
            | "N" -> New
            | _ -> invalidArg line "Invalid Instruction string"
        let side = 
            match tokens.[1] with
            | "B" -> Bid
            | "A" -> Ask
            | _ -> invalidArg line "Invalid Side string"
        {   Instruction = instruction
            Side = side
            PriceLevelIndex = Convert.ToInt32(tokens.[2])
            Price = Convert.ToInt32(tokens.[3])
            Quantity = Convert.ToInt32(tokens.[4])  }

type OrderBookEntry =
    {   Price       :   decimal
        Quantity    :   int }   
    static member Default =
        {   Price = 0.0M
            Quantity = 0    }
    static member Update price quantity =
        {   Price = price
            Quantity = quantity    }

 // O(1)
let updateOrderBook orderBookUpdate (orderBook:OrderBookEntry []) tickSize =
    orderBook.[orderBookUpdate.PriceLevelIndex - 1] <- OrderBookEntry.Update ((decimal)orderBookUpdate.Price * tickSize) orderBookUpdate.Quantity

// O(n)
// to improve look at 
// https://hamberg.no/erlend/posts/2012-08-29-purely-functional-random-access-list.html
// https://gist.github.com/kos59125/3721051
let insertIntoOrderBook orderBookUpdate (orderBook:OrderBookEntry []) tickSize =
    //orderBook |> Array.iteri (fun idx elem -> if(idx > orderBookUpdate.PriceLevelIndex-1) then orderBook.[idx] <- orderBook.[idx - 1])
//    for i in orderBookUpdate.PriceLevelIndex .. orderBook.Length-1 do
//        orderBook.[i] <- orderBook.[i - 1]
    for i in orderBook.Length-1 .. -1 .. orderBookUpdate.PriceLevelIndex  do
        orderBook.[i] <- orderBook.[i - 1]

    orderBook.[orderBookUpdate.PriceLevelIndex - 1] <- OrderBookEntry.Update ((decimal)orderBookUpdate.Price * tickSize) orderBookUpdate.Quantity
    
// O(n)
let deleteFromOrderBook orderBookUpdate (orderBook:OrderBookEntry []) =
    orderBook |> Array.iteri (fun idx elem -> 
                                                if(idx < orderBook.Length - 1) then 
                                                    if(idx >= (orderBookUpdate.PriceLevelIndex - 1)) then 
                                                        orderBook.[idx] <- orderBook.[idx + 1]
                                                else
                                                    orderBook.[idx] <- OrderBookEntry.Default)

[<EntryPoint>]
let main argv = 
    
    let fileName = argv.[0]
    let tickSize = Convert.ToDecimal(argv.[1])
    let bookDepth = Convert.ToInt32(argv.[2])

    let bidOrderBook = Array.create bookDepth OrderBookEntry.Default
    let askOrderBook = Array.create bookDepth OrderBookEntry.Default

    let orderBookChanges = IO.File.ReadLines(fileName)

   

    let updateOrderBookFromOrderBookUpdate orderBookUpdate =
        match orderBookUpdate.Side with
        | Bid ->
            match orderBookUpdate.Instruction with
            | Update -> updateOrderBook orderBookUpdate bidOrderBook tickSize
            | Delete -> deleteFromOrderBook orderBookUpdate bidOrderBook 
            | New -> insertIntoOrderBook orderBookUpdate bidOrderBook tickSize
        | Ask ->
            match orderBookUpdate.Instruction with
            | Update -> updateOrderBook orderBookUpdate askOrderBook tickSize
            | Delete -> deleteFromOrderBook orderBookUpdate askOrderBook
            | New -> insertIntoOrderBook orderBookUpdate askOrderBook tickSize

    orderBookChanges |> Seq.iter (OrderBookUpdate.FromOrderBookUpdateFileLine >> updateOrderBookFromOrderBookUpdate)

    Array.iter2 (fun x y -> printfn "%.1f,%d,%.1f,%d" x.Price x.Quantity y.Price y.Quantity) bidOrderBook askOrderBook

    0 // return an integer exit code
