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

let (|Search|_|) (re: Regex) = trySearch re

/// represents the first pattern match in a string.
let (|RegExp|_|) (pattern:string, options:RegexOptions) = 
    Regex(pattern,options)
    |> trySearch

/// input:string -> Match
let (|Rgx|_|) (pattern: string) = 
    Regex pattern
    |> trySearch

/// Same as Rgx but RegexOptions.IgnoreCase
let (|Rgi|_|) (pattern:string) = 
    Regex(pattern,RegexOptions.IgnoreCase)
    |> trySearch

/// the portion of the source string that follows the match.
let follows (m:Match) = m.Result("$'")

/// Substitutes the entire source string.
let entire (m:Match) = m.Result("$_")
   
//[<Obsolete("RegExp or Rgi")>]

[<Obsolete("trySearch")>]
let tryMatch (re: Regex) (input:string) =
    trySearch re input
    |> Option.map(fun m ->
        m.Value,input.[m.Index+m.Value.Length..]
    )


