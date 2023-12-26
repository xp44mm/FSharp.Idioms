namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open System
open System.Reflection
open System.Text.RegularExpressions

type EnumTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize enum``() =
    //    let x = DateTimeKind.Local
    //    let y = serialize x
        
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "\"Local\""

    //[<Fact>]
    //member _.``deserialize enum``() =
    //    let x = "\"Utc\""
    //    let y = deserialize<DateTimeKind> x

    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y DateTimeKind.Utc

    //[<Fact>]
    //member _.``serialize flags``() =
    //    let x = BindingFlags.Public ||| BindingFlags.NonPublic
    //    let y = serialize x

    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """["Public","NonPublic"]"""

    //[<Fact>]
    //member _.``deserialize flags``() =
    //    let x = """["Public","NonPublic"]"""
    //    let y = deserialize<BindingFlags> x

    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y (BindingFlags.Public ||| BindingFlags.NonPublic)

    //[<Fact>]
    //member _.``serialize zero flags``() =
    //    let x = RegexOptions.None
    //    let y = serialize x

    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """["None"]"""

    //[<Fact>]
    //member _.``deserialize zero flags``() =
    //    let x = """["None"]"""
    //    let y = deserialize<RegexOptions> x

    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y RegexOptions.None

    [<Fact>]
    member _.``read enum``() =
        let x = DateTimeKind.Local
        let y = JsonReaderApp.readDynamic (x.GetType()) x

        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.String "Local"
    [<Fact>]
    member _.``enum instantiate``() =
        let x = Json.String "Local"
        let y = JsonWriterApp.writeDynamic typeof<DateTimeKind> x

        //output.WriteLine(Render.stringify y)
        Should.equal y DateTimeKind.Local
         
    [<Fact>]
    member _.``read flags``() =
        let x = BindingFlags.Public ||| BindingFlags.NonPublic
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify res)
        Should.equal y <| Json.Array [ Json.String "Public"; Json.String "NonPublic" ]

    [<Fact>]
    member _.``flags instantiate``() =
        let x = Json.Array [Json.String "Public"; Json.String "NonPublic" ]
        let y = JsonWriterApp.writeDynamic typeof<BindingFlags> x

        //output.WriteLine(Render.stringify y)
        Should.equal y (BindingFlags.Public ||| BindingFlags.NonPublic)

    [<Fact>]
    member _.``read zero flags``() =
        let x = RegexOptions.None
        let y = JsonReaderApp.readDynamic typeof<RegexOptions> x
        //output.WriteLine(Render.stringify res)
        Should.equal y <| Json.Array [Json.String "None"]

    [<Fact>]
    member _.``zero flags instantiate``() =
        let x = Json.Array [Json.String "None"]
        let y = JsonWriterApp.writeDynamic typeof<RegexOptions> x

        //output.WriteLine(Render.stringify y)
        Should.equal y RegexOptions.None

