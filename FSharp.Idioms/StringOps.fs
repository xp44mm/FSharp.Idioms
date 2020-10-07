namespace FSharp.Idioms

open System

[<AutoOpen>]
module StringOps =
    let ( ** ) str i = String.replicate i str
    ///忽略大小写的方法比较字符串
    let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
    let (!=) a b = not(a == b)
