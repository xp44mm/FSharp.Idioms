module FSharp.Idioms.RegularExpressions
open System
open System.Text.RegularExpressions

/// represents the first pattern match in a string.
let trySearch (re: Regex) (input:string) =
    let m = re.Match(input)
    if m.Success then
        Some m
    else
        None

/// the portion of the source string that follows the match.
let follows (m:Match) = m.Result("$'")

/// Substitutes the entire source string.
let entire (m:Match) = m.Result("$_")
   
