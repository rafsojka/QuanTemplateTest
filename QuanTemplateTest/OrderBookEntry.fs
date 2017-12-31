module QuanTemplateTest.OrderBookEntry

type OrderBookEntry =
    {   Price       :   decimal
        Quantity    :   int }   
    static member Default =
        {   Price = 0.0M
            Quantity = 0    }
    static member Update price quantity =
        {   Price = price
            Quantity = quantity    }