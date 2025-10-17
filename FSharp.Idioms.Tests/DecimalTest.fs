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
