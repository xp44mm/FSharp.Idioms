namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit

type StringOpsTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``parseLiteral empty``() =
        let x = "\"\""
        let y = unquote x
        Should.equal y ""

    [<Fact>]
    member this.``toLiteral empty``() =
        let x = ""
        let y = quote x
        Should.equal y "\"\""


    [<Fact>]
    member this.``parseLiteral quote``() =
        let x = "\"\\\"\""
        let y = unquote x
        Should.equal y "\""

    [<Fact>]
    member this.``toLiteral quote``() =
        let x = "\""
        let y = quote x
        Should.equal y "\"\\\"\""

    [<Fact>]
    member this.``parseLiteral Escape Characters``() =
        let x = """ "\\\b\f\n\r\t\w" """.Trim()
        let y = unquote x
        Should.equal y "\\\b\f\n\r\t\w"

    [<Fact>]
    member this.``toLiteral Escape Characters``() =
        let x = String [|'"';'\\';'\b';'\f';'\n';'\r';'\t';'\\';'w';'\\'|]
        let y = quote x
        Should.equal y <| """ "\"\\\b\f\n\r\t\\w\\" """.Trim()

    [<Fact>]
    member this.``parseLiteral Unicode character``() =
        let x = """ "\u00a9" """.Trim()
        let y = unquote x
        Should.equal y "©"

    [<Fact>]
    member this.``toLiteral Unicode character``() =
        let x = "©" 
        let y = quote x
        Should.equal y "\"©\""

    [<Fact>]
    member this.``toLiteral control character``() =
        let x = "\u0002"
        let y = quote x
        Should.equal y "\"\\u0002\""
