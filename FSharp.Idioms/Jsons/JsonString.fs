module FSharp.Idioms.Jsons.JsonString

open System
open System.Globalization
open System.Text.RegularExpressions
open FSharp.Idioms.ActivePatterns
open FSharp.Idioms.RegularExpressions

/// JSON格式加引号：xyz -> "xyz"
let quote (value:string) =
    value.ToCharArray()
    |> Array.map(fun c ->
        match c with
        | '\\' -> @"\\"
        | '"' -> "\\\""
        | '\b' -> @"\b"
        | '\f' -> @"\f"
        | '\n' -> @"\n"
        | '\r' -> @"\r"
        | '\t' -> @"\t"
        | c when c < '\u0010' ->
            @"\u000" + Convert.ToString(Convert.ToInt16(c),16)
        | c when c < '\u0020' ->
            @"\u00" + Convert.ToString(Convert.ToInt16(c),16)
        | '\u007F' -> "\\u007F"
        | c ->
            c.ToString(CultureInfo.InvariantCulture)
    )
    |> String.concat ""
    |> sprintf "\"%s\""

/// JSON格式去引号: "xyz" -> xyz
let unquote (literal:string) =
    let rec loop i rest =
        seq {
            match rest with
            | "\"" -> () // ignore end "
            | First '\\' _ ->
                match rest.[1..] with
                | First '"' c ->
                    yield c
                    yield! loop (i+2) rest.[2..]
                | First '\\' c ->
                    yield c
                    yield! loop (i+2) rest.[2..]
                | First 'b' _ ->
                    yield '\b'
                    yield! loop (i+2) rest.[2..]
                | First 'f' _ ->
                    yield '\f'
                    yield! loop (i+2) rest.[2..]
                | First 'n' _ ->
                    yield '\n'
                    yield! loop (i+2) rest.[2..]
                | First 'r' _ ->
                    yield '\r'
                    yield! loop (i+2) rest.[2..]
                | First 't' _ ->
                    yield '\t'
                    yield! loop (i+2) rest.[2..]
                | Rgx @"^u[0-9A-Fa-f]{4}" m ->
                    let ch = 
                        let ffff = m.Value.[1..] // remove first u
                        Convert.ToInt32(ffff,16)
                        |> Convert.ToChar
                    yield ch
                    yield! loop (i+1+m.Length) rest.[1+m.Length..]
                | rest1 ->
                    yield '\\' // 落单的反斜杠，容错
                    yield! loop (i+1) rest1
            | _ ->
                yield rest.[0]
                yield! loop (i+1) rest.[1..]
        }

    String(
        loop 0 literal.[1..] // skip start "
        |> Array.ofSeq
    )

