module FSharp.Idioms.OrdinalIgnoreCase

open System
open System.Text.RegularExpressions
open System.Collections.Generic

///忽略大小写的方法比较字符串
let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
let (!=) a b = not(a == b)

let hashSet (sq:#seq<string>) =
    let hs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    for e in sq do
        hs.Add e |> ignore
    hs

let dictionary (pairs:#seq<string*'v>) =
    let dict = new Dictionary<string,_>(StringComparer.OrdinalIgnoreCase)
    for (k,v) in pairs do
        dict.Add (k,v) |> ignore
    dict

let (|IgnoreCase|_|) a b =
    if StringComparer.OrdinalIgnoreCase.Equals(a,b) then
        Some()
    else None
