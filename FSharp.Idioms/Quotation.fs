module FSharp.Idioms.Quotation

open System
open System.Globalization

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
    let rec loop inp =
        seq {
            match inp with
            | "" -> ()
            | PrefixChar '\\' rest ->
                match rest with
                | PrefixChar '"' rest ->
                    yield '"'
                    yield! loop rest
                | PrefixChar '\\' rest ->
                    yield '\\'
                    yield! loop rest
                | PrefixChar 'b' rest ->
                    yield '\b'
                    yield! loop rest
                | PrefixChar 'f' rest ->
                    yield '\f'
                    yield! loop rest
                | PrefixChar 'n' rest ->
                    yield '\n'
                    yield! loop rest
                | PrefixChar 'r' rest ->
                    yield '\r'
                    yield! loop rest
                | PrefixChar 't' rest ->
                    yield '\t'
                    yield! loop rest
                | Prefix "u[0-9A-Fa-f]{4}" (x,rest) ->
                    let ffff = x.[1..]
                    let value = Convert.ToInt32(ffff,16)
                    yield Convert.ToChar value
                    yield! loop rest
                | rest ->
                    yield '\\' // 落单的反斜杠容错
                    yield! loop rest
            | inp ->
                yield inp.[0]
                yield! loop inp.[1..]
        }

    System.String(
        loop literal.[1..literal.Length-2]
        |> Array.ofSeq
    )

