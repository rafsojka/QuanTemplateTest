module QuanTemplateTest.OrderBook

[<AbstractClass>]
type OrderBook(tickSize, bookDepth) =
    member this.TickSize = tickSize
    member this.BookDepth = bookDepth

    abstract member ProcessChanges  :   string seq -> unit

    abstract member Print   :   unit -> unit