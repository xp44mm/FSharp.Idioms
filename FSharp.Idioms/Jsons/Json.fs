namespace FSharp.Idioms.Jsons
open System

[<RequireQualifiedAccess>]
type Json =
    | Object of list<string*Json>
    | Array  of Json list
    | Null
    | False
    | True
    | String of string
    | Number of float // 15 ~ 17 位有效数字 e308
    //| Decimal of decimal // int64 uint64 nativeint unativeint

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

    member json.floatValue with get() =
        match json with
        | Json.Number x -> x
        | _ -> failwith "only for Json.Number"

    member json.stringText with get() =
        match json with
        | Json.String x -> x
        | _ -> failwith "only for Json.String"

    member json.fields with get() =
        match json with
        | Json.Object fields -> fields
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    member json.elements with get() =
        match json with
        | Json.Array elems -> elems
        | _ -> ArgumentOutOfRangeException "only for Json.Array" |> raise



