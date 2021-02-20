namespace FSharp.Idioms

open System
open System.Text.RegularExpressions

[<AutoOpen>]
module StringOps =
    let ( ** ) str i = String.replicate i str
    ///忽略大小写的方法比较字符串
    let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
    let (!=) a b = not(a == b)

    let private tryPrefix (pattern:string) inp =
        let re = Regex (String.Format("^(?:{0})", pattern))
        let m = re.Match(inp)
        if m.Success then
            Some(m.Value,inp.[m.Value.Length..])
        else
            None
    
    ///输入字符串的前缀子字符串符合给定的模式
    let (|Prefix|_|) = tryPrefix
    
    ///匹配输入字符串的第一个字符，返回剩余字符串
    let (|PrefixChar|_|) (c:char) (inp:string) =
        if inp.[0] = c then
            Some inp.[1..]
        else
            None
    
    