namespace FSharp.Idioms.Parsers

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit

//type PrimitiveParserTest(output: ITestOutputHelper) =
//    [<Fact>]
//    member this.``char test``() =
//        let x = "'0'"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (CHAR '0')

//    [<Fact>]
//    member this.``sbyte test``() =
//        let x = "0y"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (SBYTE 0y)

//    [<Fact>]
//    member this.``byte test``() =
//        let x = "0uy"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (BYTE 0uy)

//    [<Fact>]
//    member this.``int16 test``() =
//        let x = "0s"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INT16 0s)

//    [<Fact>]
//    member this.``uint16 test``() =
//        let x = "0us"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (UINT16 0us)

//    [<Fact>]
//    member this.``int32 test``() =
//        let x = "230"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INT32 230)

//    [<Fact>]
//    member this.``uint32 test``() =
//        let x = "0u"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (UINT32 0u)

//    [<Fact>]
//    member this.``int64 test``() =
//        let x = "0L"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INT64 0L)

//    [<Fact>]
//    member this.``uint64 test``() =
//        let x = "0UL"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (UINT64 0UL)

//    [<Fact>]
//    member this.``nativeint test``() =
//        let x = "0n"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INTPTR 0n)

//    [<Fact>]
//    member this.``unativeint test``() =
//        let x = "0un"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (UINTPTR 0un)

//    [<Fact>]
//    member this.``single test``() =
//        let x = "0.033333335f"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (SINGLE 0.033333335f)

//    [<Fact>]
//    member this.``float test``() =
//        let x = "0.03333333333333333"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (DOUBLE 0.03333333333333333)

//    [<Fact>]
//    member this.``decimal test``() =
//        let x = "0M"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (DECIMAL 0M)

//    [<Fact>]
//    member this.``bigint test``() =
//        let x = "0I"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (BIGINTEGER 0I)

//    [<Fact>]
//    member this.``0xFFuy test``() =
//        let x = "0xFFuy"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (BYTE 0xFFuy)

//    [<Fact>]
//    member this.``0x0800s test``() =
//        let x = "0x0800s"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INT16 0x0800s)

//    [<Fact>]
//    member this.``0b0001 test``() =
//        let x = "0b0001"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INT32 0b0001)

//    [<Fact>]
//    member this.``-19n test``() =
//        let x = "-19n"
//        let y = PrimitiveParser.tokenize x
//        should.equal y (INTPTR -19n)
