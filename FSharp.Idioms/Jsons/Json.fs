namespace FSharp.Idioms.Jsons
//https://learn.microsoft.com/en-us/dotnet/standard/base-types/conversion-tables
[<RequireQualifiedAccess>]
type Json =
    | Object of list<string*Json>
    | Array  of Json list
    | Null
    | False
    | True
    | String of string
    | Number of float
    | Decimal of decimal // int64 uint64 nativeint unativeint

    member t.Item with get(idx:int) =
        match t with
        | Json.Array ls -> ls.[idx]
        | _ -> failwith "int index is only for array."

    member t.Item with get(key:string) =
        match t with
        | Json.Object pairs ->
            match pairs |> List.tryFind(fst>>(=)key) with
            | Some(key,json) -> json
            | _ -> failwith "no found key."
        | _ -> failwith "string index is only for object."

    member j.ContainsKey(key:string) =
        match j with
        | Json.Object pairs ->
            pairs
            |> List.exists(fst>>(=)key)
        | _ -> false
