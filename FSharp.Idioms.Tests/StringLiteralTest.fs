namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open FSharp.Literals
type StringLiteralTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``ToStringEnsureEscapedArrayLength test``() =
        let nonAsciiChar = char 257
        let escapableNonQuoteAsciiChar = '\u0000'

        let value = sprintf "%c%s%c" nonAsciiChar @"\" escapableNonQuoteAsciiChar

        let convertedValue = StringLiteral.toStringLiteral(value)
        output.WriteLine(convertedValue)
        let expect = "\"" + String[|nonAsciiChar|] + @"\\\u0000"""

        Should.equal expect convertedValue

    [<Fact>]
    member this.``StringEscaping test``() =
        let v = "It's a good day\r\n\"sunshine\""
        let json = StringLiteral.toStringLiteral(v)
        Should.equal @"""It's a good day\r\n\""sunshine\""""" json

    [<Fact>]
    member this.``parseLiteral empty``() =
        let x = "\"\""
        let y = StringLiteral.parseStringLiteral x
        Should.equal y ""

    [<Fact>]
    member this.``toLiteral empty``() =
        let x = ""
        let y = StringLiteral.toStringLiteral x
        Should.equal y "\"\""


    [<Fact>]
    member this.``parseLiteral quote``() =
        let x = """ "\"" """.Trim()
        let y = StringLiteral.parseStringLiteral x
        Should.equal y "\""

    [<Fact>]
    member this.``toLiteral quote``() =
        let x = "\""
        let y = StringLiteral.toStringLiteral x
        Should.equal y "\"\\\"\""

    [<Fact>]
    member this.``parseLiteral Escape Characters``() =
        let x = """ "\\\b\f\n\r\t\w" """.Trim()
        let y = StringLiteral.parseStringLiteral x
        Should.equal y "\\\b\f\n\r\t\w"

    [<Fact>]
    member this.``toLiteral Escape Characters``() =
        let x = "\\\b\f\n\r\t\w"
        output.WriteLine(Render.stringify(x.ToCharArray()))
        let ee = [|'\\';'\b';'\f';'\n';'\r';'\t';'\\';'w'|]
        let y = StringLiteral.toStringLiteral x
        //to do: \后面跟着不在列表中的字符，将不会重复。
        Should.equal y <| """ "\\\b\f\n\r\t\\w" """.Trim()

    [<Fact>]
    member this.``parseLiteral Unicode character``() =
        let x = "\"\u00a9\""
        let y = StringLiteral.parseStringLiteral x
        Should.equal y "©"

    [<Fact>]
    member this.``toLiteral Unicode character``() =
        let x = "©" 
        let y = StringLiteral.toStringLiteral x
        Should.equal y "\"©\""
