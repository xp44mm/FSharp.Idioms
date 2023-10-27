namespace FSharp.Idioms.Literals

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/strings
type FSharpEscapeSequenceTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``Alert or Bell 7``() =
        let x = "\a"
        let y = int (x.Chars 0)
        Should.equal y 7

    [<Fact>]
    member this.``Backspace 8``() =
        let x = "\b"
        let y = int (x.Chars 0)
        Should.equal y 8


    [<Fact>]
    member this.``Form feed 12``() =
        let x = "\f"
        let y = int (x.Chars 0)
        Should.equal y 12

    [<Fact>]
    member this.``Newline 10``() =
        let x = "\n"
        let y = int (x.Chars 0)
        Should.equal y 10

    [<Fact>]
    member this.``Carriage return 13``() =
        let x = "\r"
        let y = int (x.Chars 0)
        Should.equal y 13

    [<Fact>]
    member this.``Horizontal Tab 9``() =
        let x = "\t"
        let y = int (x.Chars 0)
        Should.equal y 9

    [<Fact>]
    member this.``Vertical tab 11``() =
        let x = "\v"
        let y = int (x.Chars 0)
        Should.equal y 11

    [<Fact>]
    member this.``Quotation mark``() =
        let x = "\""
        let y = x.Chars 0
        Should.equal y '"'

    [<Fact>]
    member this.``Apostrophe``() =
        let x = "'"
        let y = x.Chars 0
        Should.equal y '\''

    [<Fact>]
    member this.``Unicode character dec``() =
        let x = "\231"
        let y = "ç"
        Should.equal y x

    [<Fact>]
    member this.``Unicode character hex``() =
        let x = "\xE7"
        let y = "ç"
        Should.equal y x

    [<Fact>]
    member this.``Unicode character UTF-16``() =
        let x = "\u00E7"
        let y = "ç"
        Should.equal y x

    [<Fact>]
    member this.``Unicode character UTF-32``() =
        let x = "\U0001F47D"
        let y = "👽"
        Should.equal y x

