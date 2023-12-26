namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open System

type FallbackReaderTest(output: ITestOutputHelper) =
    do()

    [<Fact>]
    member _.``try unit test``() =
        let x = ()
        let y = JsonReaderApp.readDynamic typeof<unit> x
        Assert.Equal(y, Json.Array [])

    [<Fact>]
    member _.``covert from sbyte test``() =
        let x = 0y
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y, Json.Number 0.0)

    [<Fact>]
    member _.``covert from byte test``() =
        let x = 0uy
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from int16 test``() =
        let x = 0s
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from uint16 test``() =
        let x = 0us
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from int test``() =
        let x = 0
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from uint32 test``() =
        let x = 0u
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from int64 test``() =
        let x = 0L
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from uint64 test``() =
        let x = 0UL
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from single test``() =
        let x = 0.1f
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.1)

    [<Fact>]
    member _.``covert from decimal test``() =
        let x = 0M
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from nativeint test``() =
        let x = 0n
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y, Json.Number 0.0)

    [<Fact>]
    member _.``covert from unativeint test``() =
        let x = 0un
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 0.0)

    [<Fact>]
    member _.``covert from nullable test``() =
        let x0 = Nullable()
        let y0 = JsonReaderApp.readDynamic typeof<Nullable<int>> x0
        Assert.Equal(y0,Json.Null)

        let x = Nullable(3)
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y,Json.Number 3.0)

    [<Fact>]
    member _.``covert from null test``() =
        let ls = null
        let res = JsonReaderApp.readDynamic typeof<_> ls
        Should.equal res Json.Null

    [<Fact>]
    member _.``covert from char test``() =
        let x = '\t'
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y, Json.String "\t")

    [<Fact>]
    member _.``covert from string test``() =
        let x = ""
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Assert.Equal(y, Json.String "")
