[<AutoOpen>]
module FSharp.Idioms.StringOps

open System
open System.Text.RegularExpressions

///忽略大小写的方法比较字符串
let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
let (!=) a b = not(a == b)

let ( ** ) str i = String.replicate i str
let space i = " " ** i
let indent i = space (4*i)
let space4 i = space (4*i)

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

