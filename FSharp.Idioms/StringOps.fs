[<AutoOpen>]
module FSharp.Idioms.StringOps

open System
open System.Text.RegularExpressions
open System.Globalization

///忽略大小写的方法比较字符串
let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
let (!=) a b = not(a == b)

let ( ** ) str i = String.replicate i str
let space i = " " ** i
let indent i = space (4*i)
let unindent(text) = []

let tryRegexMatch (re: Regex) (input:string) =
    let m = re.Match(input)
    if m.Success then
        Some(m.Value,input.[m.Value.Length..])
    else
        None

/// 匹配输入的开始字符串
let tryStartWith (prefix:string) (inp:string) =
    if inp.StartsWith(prefix, StringComparison.Ordinal) then
        Some(inp.[prefix.Length..])
    else None

/// 匹配前缀，用正则表达式的模式
let tryPrefix (pattern:string) =
    let re = Regex (String.Format("^(?:{0})", pattern))
    tryRegexMatch re

/// 匹配输入的首字符
let tryFirstChar (c:char) (inp:string) =
    if inp.Length > 0 && inp.[0] = c then
        Some inp.[1..]
    else
        None

/// 匹配输入的最长前缀，没有向前看的附加条件
let tryLongestPrefix (candidates:Set<string> ) (input:string) =
    let rec loop i (maybe:string option) (rest:Set<string> ) =
        if input.Length > i then 
            let fltr =
                rest
                |> Set.filter(fun x -> x.Length > i)
                |> Set.filter(fun x -> x.[i] = input.[i])
            if fltr.IsEmpty then
                maybe
            else
                let maybe = 
                    match input.[..i] with
                    | x when fltr.Contains x -> Some x
                    | _ -> maybe
                loop (i+1) maybe fltr
        else maybe
    
    loop 0 None candidates
    |> Option.map(fun elected -> elected, input.[elected.Length..])
    
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

/// 行位置，行的字符数量，数量包括结尾的\n，行的内容。
let splitLines(text:string) =
    let rec loop pos (inp:string) =
        seq{
            match inp with
            | "" -> ()
            | On (tryPrefix @"[^\n]*\n") (x, rest) ->
                yield pos, x
                yield! loop (pos+x.Length) rest

            | _ -> yield pos,inp

        }
    loop 0 text

///绝对位置计算行列索引值
let rowColumn lines (pos:int) =
    let row,col =
        lines
        |> Seq.mapi(fun r (p,l)-> r,p,l)
        |> Seq.pick(fun(row,lpos,line:string)-> 
            if lpos <= pos && pos < lpos + line.Length then
                let col = pos - lpos
                Some(row,col)
            else None
            )
    row,col

/// 每行开始的空格数
let startSpaces lines =
    lines
    |> Seq.map(fun line -> Regex.Match(line,"^ *").Length)
    |> Seq.min

///各行同时缩进
let indentCodeBlock (spaces:int) (codeBlock:string) =
    let spaces = space spaces
    codeBlock
    |> splitLines
    |> Seq.map(fun (_,line) -> spaces+line)
    |> String.concat ""