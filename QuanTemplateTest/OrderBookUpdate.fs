module QuanTemplateTest.OrderBookUpdate

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