namespace QuanTemplateTest.Tests

open NUnit.Framework
open FsUnit

open QuanTemplateTest.Main

[<TestFixture>]
type QuanTemplateTestUnitTests() = 
    [<Test>]
    member test.``updateOrderBook works`` () =
        let bookDepth = 5
        let tickSize = 10.0M
        let bidOrderBook = Array.create bookDepth OrderBookEntry.Default

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "U B 3 3 30"
        updateOrderBook orderBookUpdate bidOrderBook tickSize

        bidOrderBook.[2].Price          |> should equal 30.0
        bidOrderBook.[2].Quantity       |> should equal 30

    [<Test>]
    member test.``insertIntoOrderBook works`` () =
        let bookDepth = 5
        let tickSize = 10.0M
        let bidOrderBook = Array.create bookDepth OrderBookEntry.Default

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 3 5 50"
        insertIntoOrderBook orderBookUpdate bidOrderBook tickSize

        bidOrderBook.[2].Price          |> should equal 50.0
        bidOrderBook.[2].Quantity       |> should equal 50

        let orderBookUpdate = OrderBookUpdate.FromOrderBookUpdateFileLine "N B 1 4 40"
        insertIntoOrderBook orderBookUpdate bidOrderBook tickSize

        bidOrderBook.[0].Price          |> should equal 40.0
        bidOrderBook.[0].Quantity       |> should equal 40

        bidOrderBook.[2].Price          |> should equal 0.0M
        bidOrderBook.[2].Quantity       |> should equal 0

        bidOrderBook.[3].Price          |> should equal 50.0
        bidOrderBook.[3].Quantity       |> should equal 50
