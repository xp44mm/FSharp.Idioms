namespace FSharp.Idioms.Reflection

open Xunit

open FSharp.Idioms
open FSharp.xUnit

type Person = { name : string; age : int }

type RecordTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    //[<Fact>]
    //member this.``getRecordFields``() =
    //    let x = { name = "cuisl"; age = 18 }
    //    let props = 
    //        RecordType.getRecordFields typeof<Person>
    //        |> Array.map(fun pi -> pi.Name, pi.PropertyType.Name)
    //    Should.equal props [|"name","String";"age","Int32"|]

    //[<Fact>]
    //member this.``readRecord``() =
    //    let x = { name = "cuisl"; age = 18 }
    //    let props = 
    //        RecordType.readRecord typeof<Person> x
    //        |> Array.map(fun (pi,value) -> pi.Name, pi.PropertyType.Name,value)

    //    Should.equal props [|"name","String",box "cuisl";"age","Int32", box 18|]




