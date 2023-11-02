namespace FSharp.Idioms

open System.IO

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
        output.WriteLine($"{stringify y}")

    [<Fact>]
    member this.``02 - getFieldTitles test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        output.WriteLine($"{stringify titles}")

    [<Fact>]
    member this.``03 - getValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getValue titles "PN" rows.[1]
        output.WriteLine($"{stringify y}")

    [<Fact>]
    member this.``04 - getFloatValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getFloatValue titles "PN" rows.[1]
        output.WriteLine($"{stringify y}")

    [<Fact>]
    member this.``05 - getIntValue test``() =
        let rows = Tsv.parseTsv path
        let titles = Tsv.getFieldTitles rows
        let y = Tsv.getIntValue titles "N" rows.[1]
        output.WriteLine($"{stringify y}")






