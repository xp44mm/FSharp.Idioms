namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open System.Text.RegularExpressions

type StringOpsTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``tryLongest``() =
        let punctuators =
            set ["~"; "}"; "||="; "||"; "|="; "|"; "{"; "^="; "^"; "]"; "["; "??="; "??";
            "?"; ">>>="; ">>>"; ">>="; ">>"; ">="; ">"; "=>"; "==="; "=="; "="; "<=";
            "<<="; "<<"; "<"; ";"; ":"; "/="; "/"; "..."; "."; "-="; "--"; "-"; ",";
            "+="; "++"; "+"; "*="; "**="; "**"; "*"; ")"; "("; "&="; "&&="; "&&"; "&";
            "%="; "%"; "!=="; "!="; "!"]
        let tryLongest = tryLongestPrefix punctuators
        let x = "!= x"
        let y = tryLongest x
        Should.equal y <| Some("!="," x")

        let x1 = "="
        let y1 = tryLongest x1
        Should.equal y1 <| Some("=","")

        let x1 = "===[]"
        let y1 = tryLongest x1
        Should.equal y1 <| Some("===","[]")

    [<Fact>]
    member this.``parseLiteral empty``() =
        let x = "\"\""
        let y = Quotation.unquote x
        Should.equal y ""

    [<Fact>]
    member this.``tryStartWith empty``() =
        let x = "...rest"
        let y = tryStartWith ".." x
        Should.equal y (Some".rest")

    [<Fact>]
    member this.``toLiteral empty``() =
        let x = ""
        let y = Quotation.quote x
        Should.equal y "\"\""

    [<Fact>]
    member this.``parseLiteral quote``() =
        let x = "\"\\\"\""
        let y = Quotation.unquote x
        Should.equal y "\""

    [<Fact>]
    member this.``toLiteral quote``() =
        let x = "\""
        let y = Quotation.quote x
        Should.equal y "\"\\\"\""

    [<Fact>]
    member this.``parseLiteral Escape Characters``() =
        let x = """ "\\\b\f\n\r\t\w" """.Trim()
        let y = Quotation.unquote x
        Should.equal y "\\\b\f\n\r\t\w"

    [<Fact>]
    member this.``toLiteral Escape Characters``() =
        let x = String [|'"';'\\';'\b';'\f';'\n';'\r';'\t';'\\';'w';'\\'|]
        let y = Quotation.quote x
        Should.equal y <| """ "\"\\\b\f\n\r\t\\w\\" """.Trim()

    [<Fact>]
    member this.``parseLiteral Unicode character``() =
        let x = """ "\u00a9" """.Trim()
        let y = Quotation.unquote x
        Should.equal y "©"

    [<Fact>]
    member this.``toLiteral Unicode character``() =
        let x = "©" 
        let y = Quotation.quote x
        Should.equal y "\"©\""

    [<Fact>]
    member this.``toLiteral control character``() =
        let x = "\u0002"
        let y = Quotation.quote x
        Should.equal y "\"\\u0002\""

    [<Fact>]
    member this.``split lines``() =
        let x = "xyz\r\n\nabc"
        let y = Line.splitLines x |> Seq.toList
        let e = [
            (0, "xyz\r\n"); 
            (5, "\n"); 
            (6, "abc")]
        Should.equal e y

    [<Fact>]
    member this.``row column``() =
        let x = "xyz\r\n\nabc"
        let lines = Line.splitLines x
        let pos = 7
        let row,col = Line.rowColumn lines pos
        Should.equal x.[pos] 'b'
        Should.equal row 2
        Should.equal col 1

    [<Fact>]
    member this.``indentCodeBlock test``() =
        let x = "xyz\r\nabc"
        let y = Line.indentCodeBlock 2 x
        Should.equal y "  xyz\r\n  abc"




