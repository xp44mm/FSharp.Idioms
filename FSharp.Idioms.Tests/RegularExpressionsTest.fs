namespace FSharp.Idioms
open FSharp.Idioms.ActivePatterns
open Xunit

open System
open FSharp.xUnit
open System.Text.RegularExpressions

type RegularExpressionsTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``Active Pattern RegExp``() =
        let x = "123xYz456"
        match x with
        | RegExp(@"[a-z]+",RegexOptions.IgnoreCase) mat ->
            let y = mat.Value
            Should.equal y "xYz"
        | _ -> failwith $"{x}"

    [<Fact>]
    member this.``Active Pattern Rgi``() =
        let x = "123xYz456"
        match x with
        | Rgi @"[a-z]+" mat ->
            let y = mat.Value
            Should.equal y "xYz"
        | _ -> failwith $"{x}"

    [<Fact>]
    member this.``get follows using Match``() =
        let m = Regex.Match("123abc", @"\d+")
        //Substitutes the portion of the source string that follows the match.
        let rest = m.Result("$'")
        Should.equal rest "abc"

    [<Fact>]
    member this.``get entire source string using Match``() =
        let src = "123abc"
        let m = Regex.Match(src, @"\d+")
        //Substitutes the entire source string.
        let entire = m.Result("$_")
        Should.equal entire src

