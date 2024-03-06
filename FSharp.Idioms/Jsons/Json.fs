namespace FSharp.Idioms.Jsons
open System

[<RequireQualifiedAccess>]
type Json =
    | Object of list<string*Json>
    | Array  of list<Json>
    | Null
    | False
    | True
    | String of string
    | Number of float // 15~17 位有效数字 e308

    member t.Item with get(idx:int) =
        match t with
        | Json.Array ls -> ls.[idx]
        | _ -> failwith "int index is only for array."

    member t.Item with get(key:string) =
        match t with
        | Json.Object pairs ->
            match pairs |> List.tryFind(fst >> (=) key) with
            | Some(key,json) -> json
            | _ -> failwith "no found key."
        | _ -> failwith "string index is only for object."

    member json.floatValue with get() =
        match json with
        | Json.Number x -> x
        | _ -> failwith "only for Json.Number"

    member json.stringText with get() =
        match json with
        | Json.String x -> x
        | _ -> failwith "only for Json.String"

    member json.elements with get() =
        match json with
        | Json.Array elems -> elems
        | _ -> ArgumentOutOfRangeException "only for Json.Array" |> raise

    /// hasProperty
    member json.hasProperty(key:string) =
        match json with
        | Json.Object pairs ->
            pairs
            |> List.exists(fst >> (=) key)
        | _ -> false

    /// entries
    member json.entries with get() =
        match json with
        | Json.Object pairs -> pairs
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    member json.addProperty(key,value) =
        match json with
        | Json.Object entries ->
            (key,value) :: entries
            //|> List.distinctBy fst
            |> Json.Object
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    member json.replaceProperty(key, value) =
        match json with
        | Json.Object entries ->
            let rest =
                entries
                |> List.filter(fst >> (<>) key)
            (key,value) :: rest
            |> Json.Object
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise


