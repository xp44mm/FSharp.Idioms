namespace FSharp.Idioms
open FSharp.Idioms.RegularExpressions
open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open System.Text.RegularExpressions

type RegularExpressionsTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``Active Pattern Search``() =
        let x = "123xyz"
        match x with
        | Search(Regex(@"^\d+")) ma ->
            //https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.match?view=net-6.0
            let raw = ma.Value
            let rest = x.[ma.Length..]
            Should.equal "123" raw
            Should.equal "xyz" rest
        | _ -> failwith $"{x}"

