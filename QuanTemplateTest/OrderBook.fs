module QuanTemplateTest.OrderBook

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

[<AbstractClass>]
type OrderBook(tickSize, bookDepth) =
    member this.TickSize = tickSize
    member this.BookDepth = bookDepth

    abstract member ProcessChanges  :   string seq -> unit

    abstract member Print   :   unit -> unit

type ArrayBasedOrderBook(tickSize, bookDepth) =
    inherit OrderBook(tickSize, bookDepth)

    let mutable bidOrderBook = Array.create bookDepth OrderBookEntry.Default
    let mutable askOrderBook = Array.create bookDepth OrderBookEntry.Default

    // O(1)
    let updateOrderBook2 orderBookUpdate tickSize (orderBook:OrderBookEntry [])  =
        Array.set orderBook (orderBookUpdate.PriceLevelIndex - 1) (OrderBookEntry.Update ((decimal)orderBookUpdate.Price * tickSize) orderBookUpdate.Quantity)
        orderBook

    // O(n)
    // to improve look at 
    // https://hamberg.no/erlend/posts/2012-08-29-purely-functional-random-access-list.html
    // https://gist.github.com/kos59125/3721051
    let insertIntoOrderBook2 orderBookUpdate tickSize (orderBook:OrderBookEntry [])  =
        orderBook 
        |> Array.rev
        |> Array.mapi (
            fun idx elem -> 
                match idx with
                | idx when idx < (orderBook.Length - orderBookUpdate.PriceLevelIndex) -> orderBook.[idx + 1]
                | _ -> elem )
        |> Array.rev
        |> updateOrderBook2 orderBookUpdate tickSize

    // O(n)
    let deleteFromOrderBook2 orderBookUpdate (orderBook:OrderBookEntry []) =

        orderBook 
        |> Array.mapi (
            fun idx elem -> 
                            match idx with
                            | idx when idx = (orderBook.Length - 1) -> OrderBookEntry.Default
                            | idx when idx >= (orderBookUpdate.PriceLevelIndex - 1) -> orderBook.[idx + 1]
                            | _ -> elem )

    let updateOrderBookFromOrderBookUpdate orderBookUpdate =
        match orderBookUpdate.Side with
        | Bid ->
            let updatedBidOrderBook = 
                match orderBookUpdate.Instruction with
                | Update -> updateOrderBook2 orderBookUpdate tickSize bidOrderBook 
                | Delete -> deleteFromOrderBook2 orderBookUpdate bidOrderBook 
                | New -> insertIntoOrderBook2 orderBookUpdate tickSize bidOrderBook 
            bidOrderBook <- updatedBidOrderBook
        | Ask ->
            let updatedAskOrderBook =
                match orderBookUpdate.Instruction with
                | Update -> updateOrderBook2 orderBookUpdate tickSize askOrderBook 
                | Delete -> deleteFromOrderBook2 orderBookUpdate askOrderBook
                | New -> insertIntoOrderBook2 orderBookUpdate tickSize askOrderBook 
            askOrderBook <- updatedAskOrderBook

    override this.ProcessChanges (orderBookChanges) = 
        orderBookChanges |> Seq.iter (OrderBookUpdate.FromOrderBookUpdateFileLine >> updateOrderBookFromOrderBookUpdate)

    override this.Print () =
        Array.iter2 (fun x y -> printfn "%.1f,%d,%.1f,%d" x.Price x.Quantity y.Price y.Quantity) bidOrderBook askOrderBook
        
        
