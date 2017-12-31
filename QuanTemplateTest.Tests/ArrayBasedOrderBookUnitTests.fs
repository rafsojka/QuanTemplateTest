namespace QuanTemplateTest.Tests

open NUnit.Framework
open FsUnit

open QuanTemplateTest.OrderBookUpdate
open QuanTemplateTest.OrderBookEntry
open QuanTemplateTest.ArrayBasedOrderBook

[<TestFixture>]
type ArrayBasedOrderBookUnitTests() = 

    [<Test>]
    member test.``ArrayBasedOrderBook.updateOrderBook works`` () =
        let bookDepth = 5
        let tickSize = 10.0M
        let bidOrderBook = Array.create bookDepth OrderBookEntry.Default

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "U B 3 3 30"
        let updatedBidOrderBook = updateOrderBook orderBookUpdate tickSize bidOrderBook 

        updatedBidOrderBook.[2].Price          |> should equal 30.0
        updatedBidOrderBook.[2].Quantity       |> should equal 30

    [<Test>]
    member test.``ArrayBasedOrderBook.insertIntoOrderBook works`` () =
        let bookDepth = 5
        let tickSize = 10.0M
        let bidOrderBook = Array.create bookDepth OrderBookEntry.Default

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 3 5 50"
        let updatedBidOrderBook = insertIntoOrderBook orderBookUpdate tickSize bidOrderBook 

        updatedBidOrderBook.[2].Price          |> should equal 50.0
        updatedBidOrderBook.[2].Quantity       |> should equal 50

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 1 4 40"
        let updatedBidOrderBook = insertIntoOrderBook orderBookUpdate tickSize updatedBidOrderBook 

        updatedBidOrderBook.[0].Price          |> should equal 40.0
        updatedBidOrderBook.[0].Quantity       |> should equal 40

        updatedBidOrderBook.[2].Price          |> should equal 0.0M
        updatedBidOrderBook.[2].Quantity       |> should equal 0

        updatedBidOrderBook.[3].Price          |> should equal 50.0
        updatedBidOrderBook.[3].Quantity       |> should equal 50

    [<Test>]
    member test.``ArrayBasedOrderBook.deleteFromOrderBook works`` () =
        let bookDepth = 5
        let tickSize = 10.0M
        let bidOrderBook = Array.create bookDepth OrderBookEntry.Default

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 3 5 50"
        let updatedBidOrderBook = insertIntoOrderBook orderBookUpdate tickSize bidOrderBook 

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 1 4 40"
        let updatedBidOrderBook = insertIntoOrderBook orderBookUpdate tickSize updatedBidOrderBook 

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "D B 3 3 30"
        let updatedBidOrderBook = deleteFromOrderBook orderBookUpdate updatedBidOrderBook

        updatedBidOrderBook.[0].Price          |> should equal 40.0
        updatedBidOrderBook.[0].Quantity       |> should equal 40

        updatedBidOrderBook.[2].Price          |> should equal 50.0
        updatedBidOrderBook.[2].Quantity       |> should equal 50
