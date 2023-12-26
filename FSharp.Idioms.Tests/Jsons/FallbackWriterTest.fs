namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System

type FallbackWriterTest(output: ITestOutputHelper) =
    do()
    [<Fact>]
    member _.``try unit test``() =
        let x = Json.Array []
        let y = JsonWriterApp.writeDynamic typeof<unit> x
        Assert.Equal(y, ())

    [<Fact>]
    member _.``null``() =
        let json = Json.Null
        let y = JsonWriterApp.writeDynamic typeof<_> json
        Should.equal y null

    [<Fact>]
    member _.``false``() =
        let json = Json.False
        let y = JsonWriterApp.writeDynamic typeof<bool> json
        Should.equal y false

    [<Fact>]
    member _.``true``() =
        let json = Json.True
        let y = JsonWriterApp.writeDynamic typeof<bool> json
        Should.equal y true

    [<Fact>]
    member _.``string``() =
        let json = Json.String ""
        let y = JsonWriterApp.writeDynamic typeof<string> json
        Should.equal y ""

    [<Fact>]
    member _.``char``() =
        let json = Json.String "0"
        let y = JsonWriterApp.writeDynamic typeof<char> json
        Should.equal y '0'

    [<Fact>]
    member _.``number sbyte``() =
        let json = Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<sbyte> json
        Should.equal y 0y

    [<Fact>]
    member _.``number byte``() =
        let x =Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<byte> x
        let e = 0uy
        Assert.Equal(y, e)

    [<Fact>]
    member _.``number int16``() =
        let x =Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<int16> x
        let e = 0s
        Assert.Equal(y, e)

    [<Fact>]
    member _.``number uint16``() =
        let x =Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<uint16> x
        let e = 0us
        Assert.Equal(y, e)

    [<Fact>]
    member _.``number int``() =
        let x =Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<int> x
        Assert.Equal(y, 0)

    [<Fact>]
    member _.``number uint32``() =
        let x =Json.Number 0.0
        let y = JsonWriterApp.writeDynamic typeof<uint32> x
        Assert.Equal(y, 0u)

    [<Fact>]
    member _.``number int64``() =
        let x = Json.Decimal 0M
        let y = JsonWriterApp.writeDynamic typeof<int64> x
        Assert.Equal(y, 0L)

    [<Fact>]
    member _.``number uint64``() =
        let x = Json.Decimal 0M
        let y = JsonWriterApp.writeDynamic typeof<uint64> x
        Assert.Equal(y,0UL)

    [<Fact>]
    member _.``number single``() =
        let x =Json.Number 0.1
        let y = JsonWriterApp.writeDynamic typeof<single> x 
        Assert.Equal(y, 0.1f)

    [<Fact>]
    member _.``number decimal``() =
        let x = Json.Decimal 0M
        let y = JsonWriterApp.writeDynamic typeof<decimal> x
        Assert.Equal(y, 0M)

    [<Fact>]
    member _.``number nativeint``() =
        let x = Json.Decimal 0M
        let y = JsonWriterApp.writeDynamic typeof<nativeint> x
        Assert.Equal(y, 0n)

    [<Fact>]
    member _.``number unativeint``() =
        let x = Json.Decimal 0M
        let y = JsonWriterApp.writeDynamic typeof<unativeint> x
        Assert.Equal(y, 0un)

