module FSharp.Idioms.ActivePatterns
open FSharp.Idioms.RegularExpressions
open System.Text.RegularExpressions
open System

//[<Obsolete>]
let (|On|_|) f x = f x
let (|Wild|) x = failwith $"Wild:{x}"

let (|StartsWith|_|) = StringOps.tryStartsWith
let (|First|_|) = StringOps.tryFirst
let (|LongestPrefix|_|) = StringOps.tryLongestPrefix

/// represents the first pattern match in a string.
let (|Search|_|) = trySearch

/// input:string -> Match
let (|Rgx|_|) (pattern: string) = Regex(pattern) |> trySearch
