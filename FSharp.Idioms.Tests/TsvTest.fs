namespace FSharp.Idioms

open System.IO
open System.Text

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open FSharp.Idioms.Literal

type TsvTest(output : ITestOutputHelper) =
    let path = Path.Combine(__SOURCE_DIRECTORY__,"flange.tsv")

    [<Fact>]
    member this.``01 - parseTsv test``() =
        let y =
            Tsv.parseTsv path
        output.WriteLine(stringify y)

    [<Fact>]
    member this.``02 - parseText test``() =
        let text = File.ReadAllText(path,Encoding.UTF8)
        let y =
            Tsv.parseText text
        output.WriteLine(stringify y)

    [<Fact>]
    member this.``03 - stringify test``() =
        let cells =
            [|
                [|"a";"b"|]
                [|"1";"2"|]
            |]
        let y = Tsv.stringify cells
        let e = "a\tb\r\n1\t2"
        //output.WriteLine(stringify y)
        Should.equal y e

    [<Fact>]
    member this.``04 - getFieldTitles test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        output.WriteLine(stringify titles)

    [<Fact>]
    member this.``05 - getValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getValue titles "PN" rows.[1]
        output.WriteLine(stringify y)

    [<Fact>]
    member this.``06 - getFloatValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getFloatValue titles "PN" rows.[1]
        output.WriteLine(stringify y)

    [<Fact>]
    member this.``07 - getIntValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getIntValue titles "N" rows.[1]
        output.WriteLine(stringify y)






