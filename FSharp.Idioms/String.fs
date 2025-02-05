module FSharp.Idioms.String

open System
open System.Text.RegularExpressions

let fromCharArray (chars) =
    chars
    |> Seq.toArray
    |> System.String
