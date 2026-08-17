module FSharp.Idioms.Int64

open System

let take (chars: char list) =
    let sign, rest = Char.tryPlusMinusRest chars
    let value, count, restAfterValue = UInt64.takeValueAndBits rest

    if count = 0 then
        None, rest
    else
        match sign with
        | Some -1 -> Some(-int64(value)), restAfterValue
        | _ -> Some(int64(value)), restAfterValue

/// Try to parse a string as a int64. Returns Some(int64) if successful, or None if the input is not a valid int64.
let tryParse (inp: string) =
    if String.IsNullOrWhiteSpace inp then
        None
    else
        let chars = List.ofSeq inp
        match take chars with
        | Some value, [] -> Some value
        | _ -> None
