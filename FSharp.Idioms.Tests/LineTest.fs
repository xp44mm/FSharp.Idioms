namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open System.Text.RegularExpressions

type LineTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``split lines``() =
        let x = "xyz\r\n\nabc"
        let y = Line.splitToLines x |> Seq.toList
        let e = [
            ("xyz"); 
            (""); 
            ("abc")]
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
        let y = Line.identAll 2 [x]
        Should.equal y "  xyz\r\n  abc"

    [<Fact>]
    member _.``getColumnAndRest``() =
        let start = 19
        let inp = "0123456789\nabc"
        let pos = 20
        let col,nextStart = 
            Line.getColumnAndLpos (start, inp) pos
        //show (col,nextStart,rest)
        Should.equal col 1 //20-19,col base on 0
        Should.equal nextStart 30 //19+11




