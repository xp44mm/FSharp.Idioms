namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open System.Text.RegularExpressions

type StringOpsTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``tryStart empty``() =
        let x = "...rest"
        let y = StringOps.tryStartsWith ".." x |> Option.get
        Should.equal y ".."

    [<Fact>]
    member this.``tryLongestPrefix``() =
        let punctuators =
            set ["~"; "}"; "||="; "||"; "|="; "|"; "{"; "^="; "^"; "]"; "["; "??="; "??";
            "?"; ">>>="; ">>>"; ">>="; ">>"; ">="; ">"; "=>"; "==="; "=="; "="; "<=";
            "<<="; "<<"; "<"; ";"; ":"; "/="; "/"; "..."; "."; "-="; "--"; "-"; ",";
            "+="; "++"; "+"; "*="; "**="; "**"; "*"; ")"; "("; "&="; "&&="; "&&"; "&";
            "%="; "%"; "!=="; "!="; "!"]
        let tryLongest = StringOps.tryLongestPrefix punctuators
        let x = "!= x"
        let y = tryLongest x |> Option.get

        Should.equal y "!="

        let x1 = "="
        let y1 = tryLongest x1 |> Option.get
        Should.equal y1 x1

        let x1 = "===[]"
        let y1 = tryLongest x1 |> Option.get
        Should.equal y1 "==="




