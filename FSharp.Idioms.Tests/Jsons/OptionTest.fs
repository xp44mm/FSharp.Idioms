namespace FSharp.Idioms.Jsons

open Xunit

open FSharp.xUnit
open System
open FSharp.Idioms
open FSharp.Idioms.Reflection

type OptionTest(output: ITestOutputHelper) =
    do()

    [<Fact>]
    member _.``box unbox some``() =
        let v = Some 1
        let x = box v
        let ty = x.GetType()
        let e = OptionType.get_Value ty x
        Should.equal e (box v.Value)

    [<Fact>]
    member _.``typeof test``() =
        let x = typeof<_>
        let y = typeof<obj>
        Should.equal x y

    //[<Fact>]
    //member _.``serialize none``() =
    //    let x = None
    //    let y = serialize x
    //    Should.equal y "null"

    //[<Fact>]
    //member _.``deserialize none``() =
    //    let x = "null"
    //    let y = deserialize<_ option> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y None

    //[<Fact>]
    //member _.``serialize some``() =
    //    let x = Some 1
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "1"

    //[<Fact>]
    //member _.``deserialize some``() =
    //    let x = "1"
    //    let y = deserialize<int option> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y <| Some 1

    [<Fact>]
    member _.``read none``() =
        let x = None
        let y = JsonReaderApp.readDynamic typeof<option<_>> x
        Should.equal y (Json.Array [])

    [<Fact>]
    member _.``read some``() =
        let x = Some 1
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y (Json.Array [Json.Number 1.0])

    [<Fact>]
    member _.``write none``() =
        let x = Json.Array []
        let y = JsonWriterApp.writeDynamic typeof<int option> x
        //output.WriteLine(Render.stringify y)
        Should.equal y None

    [<Fact>]
    member _.``write some``() =
        let x = Json.Array [Json.Number 1.0]
        let y = 
            JsonWriterApp.writeDynamic typeof<int option> x
            |> unbox<int option>
        //output.WriteLine(Render.stringify y)
        Should.equal y (Some 1)




