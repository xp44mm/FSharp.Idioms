module FSharp.Idioms.Parsers.PrimitiveParser

open FSharp.Idioms
open FSharp.Idioms.ActivePatterns

open System.Globalization
open System
open System.Text.RegularExpressions

type NumberInfo =
    {
        known: string list
        flags: Set<char>
    }

let realNumberFlags = set [ '.'; 'e'; 'E' ]

let isDouble flags =
    (realNumberFlags - flags).IsProperSubsetOf realNumberFlags

let realStyle (flags:Set<_>) =
    let p =
        if flags.Contains '.' then
            NumberStyles.AllowDecimalPoint
        else
            NumberStyles.None
    let e =
        if flags.Contains 'e' || flags.Contains 'E' then
            NumberStyles.AllowExponent
        else
            NumberStyles.None

    p ||| e

let getBase (flags:Set<_>) =
    if flags.Contains 'x' then
        16
    elif flags.Contains 'o' then
        8
    elif flags.Contains 'b' then
        2
    else
        10

/// 当确定输入是数字后，试探着解析剩余的部分
let rec private numberLoop (info:NumberInfo) inp =
    match inp with
    | Search(Regex @"^\.\d+") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]
        let info = {
            known = [info.known.Head + x]
            flags = info.flags.Add '.'
        }
        numberLoop info rest
    | Search(Regex @"^[eE][-+]?\d+") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]

        let info = {
            known = [info.known.Head + x]
            flags = info.flags.Add 'e'
        }
        numberLoop info rest
    | Search(Regex @"^[xX][0-9a-fA-F]+") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]
        let info = {
            known = [x.[1..]]
            flags = info.flags.Add 'x'
        }
        numberLoop info rest
    | Search(Regex @"^[oO][0-7]+") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]

        let info = {
            known = [x.[1..]]
            flags = info.flags.Add 'o'
        }
        numberLoop info rest
    | Search(Regex @"^[bB][01]+") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]

        let info = {
            known = [x.[1..]]
            flags = info.flags.Add 'b'
        }
        numberLoop info rest
    | Search(Regex @"^([fFmMIu]|u?[ysln]|U?L)\b") m ->
        let x = m.Value
        let rest = inp.[m.Index+m.Value.Length..]

        let info = {
            known = x :: info.known
            flags = info.flags.Add '$'
        }
        numberLoop info rest
    | _  ->
        let inline sign x =
            if info.flags.Contains '-' then -x else x
        if info.flags.Contains '$' then
            match info.known.Head with
            |  "f" | "F" ->
                let ii =
                    System.Single.Parse(info.known.Tail.Head,realStyle info.flags)
                    |> sign
                SINGLE ii
            |  "m" | "M" ->
                let ii =
                    System.Decimal.Parse(info.known.Tail.Head,realStyle info.flags)
                    |> sign
                DECIMAL ii
            |  "y" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parseInt32
                    |> Convert.ToSByte
                    |> sign
                SBYTE ii
            | "uy" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parseInt32
                    |> Convert.ToByte
                BYTE ii
            |  "s" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parseInt32
                    |> Convert.ToInt16
                    |> sign
                INT16 ii
            | "us" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parseInt32
                    |> Convert.ToUInt16
                UINT16 ii
            |  "l" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parseInt32
                    |> sign
                INT32 ii
            | "u"|"ul" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse Convert.ToUInt32
                UINT32 ii
            |  "L" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse Convert.ToInt64
                    |> sign
                INT64 ii
            | "UL" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse Convert.ToUInt64
                UINT64 ii
            |  "n" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse Convert.ToInt64
                    |> System.IntPtr.op_Explicit
                    |> sign
                INTPTR ii
            | "un" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse Convert.ToUInt64
                    |> System.UIntPtr.op_Explicit

                UINTPTR ii
            | "I" ->
                let ii =
                    (getBase info.flags,info.known.Tail.Head)
                    ||> ParseInteger.parse (System.Numerics.BigInteger.op_Implicit)
                    |> sign
                BIGINTEGER ii
            | never -> failwith never
        elif isDouble info.flags then
            let ii =
                System.Double.Parse (info.known.Head, realStyle info.flags)
                |> sign
            DOUBLE ii
        else
            let ii =
                (getBase info.flags, info.known.Head)
                ||> ParseInteger.parseInt32
                |> sign
            INT32 ii

