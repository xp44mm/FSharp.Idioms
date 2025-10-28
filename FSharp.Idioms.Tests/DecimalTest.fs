namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit
open System.Text.RegularExpressions

type DecimalTest(output: ITestOutputHelper) =

    [<Theory>]
    [<Natural 5>]
    member this.``takeNumber``(i: int) =
        let cases =
            [
                [ '1'; '2'; '3' ], (123.0, [])
                [ '-'; '4'; '5'; '.'; '6' ], (-45.6, [])
                [ '7'; '8'; 'e'; '2' ], (7800.0, [])
                [ '0'; '.'; '9' ], (0.9, [])
                [ '1'; '.'; '2'; '3'; 'e'; '-'; '1' ], (0.123, [])
            ]
        let buff, e = cases.[i]
        let y = Decimal.takeNumber buff
        Should.equal e y

    [<Fact>]
    member this.``big number test``() =
        let x = "1.234567890123456789"

        let buff = x.ToCharArray() |> Array.toList

        let y = Decimal.takeNumber buff |> fst
        output.WriteLine($"{y}")

    [<Fact>]
    member this.``parse int test``() =
        let x = "-234567890"
        let y = Decimal.parseInt x
        let e = -234567890L
        output.WriteLine($"{y}")
        Should.equal e y
    [<Fact>]
    member this.``parse float test``() =
        let x = "-234567890.1234568"
        let y = Decimal.parseFloat x
        let e = -234567890.1234568
        output.WriteLine($"{y}")
        Should.equal e y



