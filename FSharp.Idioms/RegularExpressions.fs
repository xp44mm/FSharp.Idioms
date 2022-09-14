module FSharp.Idioms.RegularExpressions
open System
open System.Text.RegularExpressions

/// represents the first pattern match in a string.
let trySearch (re: Regex) (input:string) =
    let m = re.Match(input)
    if m.Success then
        Some(m)
    else
        None

/// represents the first pattern match in a string.
let (|Search|_|) (re: Regex) = trySearch re

/// input:string -> Match
let (|Rgx|_|) (pattern: string) = 
    Regex pattern
    |> trySearch
