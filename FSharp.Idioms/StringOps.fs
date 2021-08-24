namespace FSharp.Idioms

open System
open System.Text.RegularExpressions
open System.Globalization

[<AutoOpen>]
module StringOps =
    let ( ** ) str i = String.replicate i str
    ///忽略大小写的方法比较字符串
    let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
    let (!=) a b = not(a == b)

    let tryRegexMatch (re: Regex) (input:string) =
        let m = re.Match(input)
        if m.Success then
            Some(m.Value,input.[m.Value.Length..])
        else
            None

    let tryStartWith (prefix:string) (inp:string) =
        if inp.StartsWith(prefix, StringComparison.Ordinal) then
            Some(inp.[prefix.Length..])
        else None
    
    let tryPrefix (pattern:string) =
        let re = Regex (String.Format("^(?:{0})", pattern))
        tryRegexMatch re

    let tryFirstChar (c:char) (inp:string) =
        if inp.Length > 0 && inp.[0] = c then
            Some inp.[1..]
        else
            None

    ///输入字符串的前缀子字符串符合给定的模式
    let (|Prefix|_|) = tryPrefix


    ///匹配输入字符串的第一个字符，返回剩余字符串
    let (|PrefixChar|_|) = tryFirstChar

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

