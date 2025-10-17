module FSharp.Idioms.ActivePatterns
open FSharp.Idioms.RegularExpressions
open System.Text.RegularExpressions
open System

let (|On|_|) f x = f x
let (|Wild|) x = failwith $"Wild:{x}"

let (|StartsWith|_|) = String.tryStartsWith
let (|First|_|) = String.tryFirst
let (|LongestPrefix|_|) = String.tryLongestPrefix

///// represents the first pattern match in a string.
//[<Obsolete("RegularExpressions.(|Search|_|)")>]
//let (|Search|_|) = trySearch

///// input:string -> Match
//[<Obsolete("RegularExpressions.(|Rgx|_|)")>]
//let (|Rgx|_|) (pattern: string) = Regex(pattern) |> trySearch


let (|Search|_|) (re: Regex) = trySearch re

/// represents the first pattern match in a string.
let (|RegExp|_|) (pattern:string, options:RegexOptions) = 
    Regex(pattern,options)
    |> trySearch

/// input:string -> Match
let (|Rgx|_|) (pattern: string) = 
    Regex pattern
    |> trySearch

/// Same as Rgx and use RegexOptions.IgnoreCase
let (|Rgi|_|) (pattern:string) = 
    Regex(pattern,RegexOptions.IgnoreCase)
    |> trySearch
