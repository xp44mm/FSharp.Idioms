module FSharp.Idioms.StringOps

open System
open System.Text.RegularExpressions

///忽略大小写的方法比较字符串
let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
let (!=) a b = not(a == b)

let ( ** ) str i = String.replicate i str

[<Obsolete("Line.space")>]
let space i = " " ** i

[<Obsolete("Line.space4")>]
let space4 i = " " ** (4*i)

[<Obsolete("Line.space4")>]
let indent i = " " ** (4*i)

/// 匹配输入的开始字符串
let tryStartsWith (value:string) (inp:string) =
    if inp.StartsWith(value, StringComparison.Ordinal) then
        Some value
    else None

[<Obsolete(nameof tryStartsWith)>]
let tryStart (prefix:string) (inp:string) =
    tryStartsWith prefix inp
    |> Option.map(fun _ ->
        inp.[prefix.Length..]
    )

/// 匹配输入的首字符
let tryFirst (c:char) (inp:string) =
    if inp.Length > 0 && inp.[0] = c then
        Some c
    else
        None

/// 匹配输入的最长前缀，没有向前看的附加条件
let tryLongestPrefix (candidates:#seq<string>) (input:string) =
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
    candidates
    |> Set.ofSeq
    |> loop 0 None
    
[<Obsolete(nameof RegularExpressions.trySearch)>]
let tryRegexMatch (re: Regex) (input:string) = 
    RegularExpressions.trySearch re input
    |> Option.map(fun m ->
        m.Value,input.[m.Index+m.Value.Length..]
    )

[<Obsolete(nameof RegularExpressions.trySearch)>]
let tryMatch (re: Regex) (input:string) = 
    RegularExpressions.trySearch re input
    |> Option.map(fun m ->
        m.Value,input.[m.Index+m.Value.Length..]
    )

[<Obsolete(nameof tryStartsWith)>]
let tryStartWith = tryStartsWith

/// 匹配前缀，用正则表达式的模式
[<Obsolete(nameof RegularExpressions.trySearch)>]
let tryPrefix (pattern:string) =
    let re = Regex (String.Format("^(?:{0})", pattern))
    fun inp ->
        RegularExpressions.trySearch re inp
        |> Option.map(fun m ->
            m.Value,inp.[m.Index+m.Value.Length..]
        )

///输入字符串的前缀子字符串符合给定的模式
[<Obsolete(nameof RegularExpressions.(|Rgx|_|))>]
let (|Prefix|_|) (pattern:string) =
    let re = Regex (String.Format("^(?:{0})", pattern))
    fun inp ->
        RegularExpressions.trySearch re inp
        |> Option.map(fun m ->
            m.Value,inp.[m.Index+m.Value.Length..]
        )

///匹配输入字符串的第一个字符，返回剩余字符串
[<Obsolete("(|First|_)")>]
let (|PrefixChar|_|) = tryFirst

