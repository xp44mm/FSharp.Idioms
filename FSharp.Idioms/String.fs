module FSharp.Idioms.String

open System
open System.Reflection
open System.IO

let ( ** ) str i = String.replicate i str

let fromChars (chars) = chars |> Seq.toArray |> System.String

let fromCharlist (chars: char list) = chars |> List.toArray |> System.String

[<Obsolete("fromChars")>]
let fromCharArray (chars) = fromChars chars

/// 匹配输入的开始字符串
let tryStartsWith (value: string) (inp: string) =
    if inp.StartsWith(value, StringComparison.Ordinal) then
        Some value
    else
        None

/// 匹配输入的首字符
let tryFirst (c: char) (inp: string) =
    if inp.Length > 0 && inp.[0] = c then
        Some c
    else
        None

/// 匹配输入的最长前缀，没有向前看的附加条件
let tryLongestPrefix (candidates: #seq<string>) (input: string) =
    let rec loop i (maybe: string option) (rest: Set<string>) =
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
                loop (i + 1) maybe fltr
        else
            maybe
    candidates |> Set.ofSeq |> loop 0 None

let fromEmbedded (assy:Assembly) (name: string) =
    use stream = assy.GetManifestResourceStream(name)
    if isNull stream then
        failwith $"Resource '{name}' not found in assembly '{assy.GetName()}'."
    use sr = new StreamReader(stream)
    sr.ReadToEnd()
