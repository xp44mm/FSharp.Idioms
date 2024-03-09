namespace FSharp.Idioms.Jsons
open System

[<RequireQualifiedAccess>]
type Json =
    | Object of entries:list<string*Json>
    | Array  of elements:list<Json>
    | Null
    | False
    | True
    | String of text:string
    | Number of value:float // 15~17 位有效数字 e308

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

    member json.getValue() =
        match json with
        | Json.Number x -> x
        | _ -> failwith "only for Json.Number"

    member json.floatValue with get() = json.getValue()

    member json.getText() =
        match json with
        | Json.String x -> x
        | _ -> failwith "only for Json.String"

    member json.stringText with get() = json.getText()

    member json.getElements() =
        match json with
        | Json.Array elems -> elems
        | _ -> ArgumentOutOfRangeException "only for Json.Array" |> raise

    member json.elements with get() = json.getElements()

    /// hasProperty
    member json.hasProperty(key:string) =
        match json with
        | Json.Object pairs ->
            pairs
            |> List.exists(fst >> (=) key)
        | _ -> false

    /// entries
    member json.getEntries() =
        match json with
        | Json.Object pairs -> pairs
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    /// entries
    member json.entries with get() = json.getEntries()

    member json.addProperty(key,value) =
        match json with
        | Json.Object entries ->
            (key,value) :: entries
            |> Json.Object
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    //保留键位置，新增附后
    member json.replaceProperty(key, value) =
        if json.hasProperty key then
            json.getEntries()
            |> List.map(fun ((name,_) as entry) -> 
                if name = key then
                    name,value
                else entry
            )
            |> Json.Object
        else
            json.getEntries() @ [key,value]
            |> Json.Object

    /// Object.assign
    member json.assign (entries:seq<string*Json>) =
        [
            yield! json.entries |> List.map fst
            yield! entries |> Seq.map fst
        ]
        |> List.distinct
        |> List.map(fun name -> 
            match entries |> Seq.tryFind(fst >> (=) name) with
            | Some x -> x
            | _ -> name,json.[name]
        )
        |> Json.Object

    /// Object.assign
    member a.assign (b:Json) = a.assign(b.entries)

    /// 先来者赢，后来者补充
    member json.merge (entries:seq<string*Json>) = 
        let ls = 
            json.entries 
            |> List.map fst
            |> Set.ofList

        let entries = 
            entries 
            |> Seq.filter(fun(name,_) -> ls.Contains name |> not)
        json.assign(entries)

    member a.merge (b:Json) = a.merge(b.entries)
