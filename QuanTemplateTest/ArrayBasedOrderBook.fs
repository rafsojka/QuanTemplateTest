module QuanTemplateTest.ArrayBasedOrderBook

open QuanTemplateTest.OrderBookUpdate
open QuanTemplateTest.OrderBookEntry
open QuanTemplateTest.OrderBook

// O(1)
let updateOrderBook orderBookUpdate tickSize (orderBook:OrderBookEntry [])  =
    Array.set orderBook (orderBookUpdate.PriceLevelIndex - 1) (OrderBookEntry.Update ((decimal)orderBookUpdate.Price * tickSize) orderBookUpdate.Quantity)
    orderBook

// O(n)
// to improve look at 
// https://hamberg.no/erlend/posts/2012-08-29-purely-functional-random-access-list.html
// https://gist.github.com/kos59125/3721051
let insertIntoOrderBook orderBookUpdate tickSize (orderBook:OrderBookEntry [])  =
    orderBook 
    |> Array.rev
    |> Array.mapi (
        fun idx elem -> 
            match idx with
            | idx when idx < (orderBook.Length - orderBookUpdate.PriceLevelIndex) -> orderBook.[idx + 1]
            | _ -> elem )
    |> Array.rev
    |> updateOrderBook orderBookUpdate tickSize

// O(n)
let deleteFromOrderBook orderBookUpdate (orderBook:OrderBookEntry []) =

    orderBook 
    |> Array.mapi (
        fun idx elem -> 
                        match idx with
                        | idx when idx = (orderBook.Length - 1) -> OrderBookEntry.Default
                        | idx when idx >= (orderBookUpdate.PriceLevelIndex - 1) -> orderBook.[idx + 1]
                        | _ -> elem )

type ArrayBasedOrderBook(tickSize, bookDepth) =
    inherit OrderBook(tickSize, bookDepth)

    let mutable bidOrderBook = Array.create bookDepth OrderBookEntry.Default
    let mutable askOrderBook = Array.create bookDepth OrderBookEntry.Default

    let updateOrderBookFromOrderBookUpdate orderBookUpdate =
        match orderBookUpdate.Side with
        | Bid ->
            let updatedBidOrderBook = 
                match orderBookUpdate.Instruction with
                | Update -> updateOrderBook orderBookUpdate tickSize bidOrderBook 
                | Delete -> deleteFromOrderBook orderBookUpdate bidOrderBook 
                | New -> insertIntoOrderBook orderBookUpdate tickSize bidOrderBook 
            bidOrderBook <- updatedBidOrderBook
        | Ask ->
            let updatedAskOrderBook =
                match orderBookUpdate.Instruction with
                | Update -> updateOrderBook orderBookUpdate tickSize askOrderBook 
                | Delete -> deleteFromOrderBook orderBookUpdate askOrderBook
                | New -> insertIntoOrderBook orderBookUpdate tickSize askOrderBook 
            askOrderBook <- updatedAskOrderBook

    override this.ProcessChanges (orderBookChanges) = 
        orderBookChanges |> Seq.iter (OrderBookUpdate.FromOrderBookUpdateFileLine >> updateOrderBookFromOrderBookUpdate)

    override this.Print () =
        Array.iter2 (fun x y -> printfn "%.1f,%d,%.1f,%d" x.Price x.Quantity y.Price y.Quantity) bidOrderBook askOrderBook