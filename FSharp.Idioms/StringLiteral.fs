module FSharp.Idioms.StringLiteral

open System
open System.Globalization

let private nonprintablePairs =
    [
        '\\'    , "\\\\"
        '\t'    , "\\t"
        '\n'    , "\\n"
        '\r'    , "\\r"
        '\f'    , "\\f"
        '\b'    , "\\b"
        '\u0000', "\\u0000" // Null char
        '\u0085', "\\u0085" // Next Line
        '\u2028', "\\u2028" // Line Separator
        '\u2029', "\\u2029" // Paragraph Separator
    ]

let private charEscapings =
    ('\'' , @"\'") :: nonprintablePairs
    |> Map.ofList

/// ['s'] -> "s"
let private stringEscapings =
    ('"' , "\\\"") :: nonprintablePairs
    |> Map.ofList

let private charUnescapings =
    charEscapings
    |> Map.toSeq
    |> Seq.map(fun(x,y)->y,x)
    |> Map.ofSeq

/// ["s"] -> 's'
let private stringUnescapings =
    stringEscapings
    |> Map.toSeq
    |> Seq.map(fun(x,y)->y,x)
    |> Map.ofSeq

/// c -> 'c'
let toCharLiteral (c:char) =
    if charEscapings.ContainsKey c then
        charEscapings.[c]
    else
        c.ToString(CultureInfo.InvariantCulture)
    |> sprintf "'%s'"

/// xyz -> "xyz"
let toStringLiteral (value:string) =
    value.ToCharArray()
    |> Array.map(fun c ->
        if stringEscapings.ContainsKey c then
            stringEscapings.[c]
        else
            c.ToString(CultureInfo.InvariantCulture)
    )
    |> String.concat ""
    |> fun s -> "\"" + s + "\""

/// 解析4位16进制unicode表示的字符：\uffff
let parseCode (literal:string) =
    let hex = Convert.ToUInt32(literal.[2..], 16)
    Convert.ToChar hex

/// 'c' -> c
let parseCharLiteral (quotedString:string) =
    let content = quotedString.[1..quotedString.Length-2]//去掉首尾包围的引号
    if charUnescapings.ContainsKey content then
        charUnescapings.[content]
    elif content.StartsWith @"\u" then
        parseCode content
    else
        content.[0]

/// "xyz" -> xyz
let parseStringLiteral (quotedString:string) =
    let rec loop inp =
        seq {
            match inp with
            | "" -> ()

            | Prefix """\\["bfnrt\\]""" (x,rest) ->
                yield stringUnescapings.[x]
                yield! loop rest

            | Prefix """\\u[0-9a-fA-F]{4}""" (x,rest) ->
                yield parseCode x
                yield! loop rest

            | _ ->
                yield inp.[0]
                yield! loop inp.[1..]
        }
    //去掉首尾包围的引号
    let content = quotedString.[1..quotedString.Length-2]
    String(loop content |> Array.ofSeq)

