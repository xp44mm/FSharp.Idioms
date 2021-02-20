namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit

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
