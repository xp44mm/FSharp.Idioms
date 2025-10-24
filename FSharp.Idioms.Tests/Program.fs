module FSharp.Idioms.Program

open System
open System.IO
open System.Text

open System
open System.Collections
open System.Collections.Generic

open FSharp.Idioms.Literal
open System
open System.Xml
open System.Reflection


open System
open System.IO
open System.Text

let [<EntryPoint>] main _ =
    let x = "1.234567890123456789"
    Console.WriteLine($"{x}")

    let buff = x.ToCharArray() |> Array.toList

    let y = Decimal.takeNumber buff |> fst
    Console.WriteLine($"{y}")


    0
