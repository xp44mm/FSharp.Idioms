module FSharp.Idioms.Parsers.ParseInteger

let digits = [| '0' .. '9' |] 

///大写字母表
let uppercases = [| 'A'..'F' |]

///小写字母表
let lowercases = [| 'a'..'f' |]

///字母，位置，查詢表
let alphabet =
    [
        yield! digits |> Array.mapi(fun i c -> c,i)
        yield! uppercases |> Array.mapi(fun i c ->c,i+10)
        yield! lowercases |> Array.mapi(fun i c ->c,i+10)
    ]|> Map.ofList

let valueof (hex:char) = alphabet.[hex]

let getDigits (text:string) =
    text.ToCharArray()
    |> Array.map(valueof)
    |> Array.rev

let parseInt32 bs (text:string) =
    getDigits text
    |> Array.mapi (fun i d -> d * pown bs i)
    |> Array.sum

let inline parse (convert:int->'a) (bs:int) (text:string) =
    let bs = convert bs

    getDigits text
    |> Seq.map convert
    |> Seq.mapi(fun i d -> d * pown bs i)
    |> Seq.sum

