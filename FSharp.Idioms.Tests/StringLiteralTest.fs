namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open FSharp.Idioms.Literals
open FSharp.Idioms

type StringLiteralTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``escape sequence in string``() =
        //https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
        let x = "\a\b\f\n\r\t\v\\\"\'"
        let y = x.ToCharArray()
        Should.equal y [|'\a';'\b';'\f';'\n';'\r';'\t';'\v';'\\';'\"';'\'';|]

    [<Fact>]
    member this.``Quotation mark``() =
        Should.equal "\"" <| '\"'.ToString()
        Should.equal "\"" <| '"'.ToString()
    
    [<Fact>]
    member this.``Apostrophe``() =
        Should.equal '\'' <| "\'".Chars 0
        Should.equal '\'' <| "'".Chars 0

    [<Fact>]
    member this.``toStringLiteral empty``() =
        let x = ""
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y "\"\""

    [<Fact>]
    member this.``toStringLiteral null char``() =
        let x = "\u0000"
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y @"""\u0000"""

    [<Fact>]
    member this.``toCharLiteral null char``() =
        let x = '\u0000'
        let y = FSharpCodeUtils.toCharLiteral x
        Should.equal y @"'\u0000'"

    [<Fact>]
    member this.``toStringLiteral Unit Separator``() =
        let x = "\u001F"
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y @"""\u001F"""

    [<Fact>]
    member this.``toCharLiteral Unit Separator``() =
        let x = '\u001F'
        let y = FSharpCodeUtils.toCharLiteral x
        Should.equal y @"'\u001F'"

    [<Fact>]
    member this.``toStringLiteral delete``() =
        let x = "\u007F"
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y @"""\u007F"""

    [<Fact>]
    member this.``toCharLiteral delete``() =
        let x = '\u007F'
        let y = FSharpCodeUtils.toCharLiteral x
        Should.equal y @"'\u007F'"

    [<Fact>]
    member this.``toStringLiteral quote``() =
        let x = "\""
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y "\"\\\"\""


    [<Fact>]
    member this.``toStringLiteral Escape Characters``() =
        let x = String [|'"';'\\';'\b';'\f';'\n';'\r';'\t';'\\';'w';'\\'|]
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y <| """ "\"\\\b\f\n\r\t\\w\\" """.Trim()

    [<Fact>]
    member this.``toStringLiteral Trigraph``() =
        let x = "\032" 
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y "\" \""

    [<Fact>]
    member this.``toStringLiteral Unicode character``() =
        let x = "\u00a9"
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y "\"©\""


    [<Fact>]
    member this.``toStringLiteral Long Unicode character``() =
        let x = "\U00002260"
        let y = FSharpCodeUtils.toStringLiteral x
        Should.equal y "\"≠\""

