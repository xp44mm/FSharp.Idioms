namespace FSharp.Idioms.Jsons
open System

[<RequireQualifiedAccess>]
type Json =
    | Object of entries:list<string*Json>
    | Array  of elements:list<Json>
    | Null
    | False
    | True
    | String of text: string
    | Number of value: float // 15~17 位有效数字 max 10^308

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
        | _ -> 
            ArgumentOutOfRangeException "only for Json.Array" 
            |> raise

    member json.elements with get() = json.getElements()

    member json.hasProperty(key:string) =
        match json with
        | Json.Object pairs ->
            pairs
            |> List.exists(fst >> (=) key)
        | _ -> false

    member json.getEntries() =
        match json with
        | Json.Object pairs -> pairs
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    member json.entries with get() = json.getEntries()

    /// Object.assign：保持先来者顺序，取后来者数值
    member object.assign (entries:seq<string*Json>) =
        [
            yield! object.entries
            yield! entries
        ]
        |> List.groupBy fst
        |> List.map(snd>>List.last) //后来者赢
        |> Json.Object

    /// Object.assign
    member obj1.assign (obj2:Json) = obj1.assign(obj2.entries)

    /// 保持先来者顺序，先来者赢，后来者补充coalesce
    member object.coalesce (entries:seq<string*Json>) =
        [
            yield! object.entries
            yield! entries
        ]
        |> List.distinctBy fst //先来者赢
        |> Json.Object

    member obj1.coalesce (obj2:Json) = obj1.coalesce(obj2.entries)

    [<Obsolete("Json.assign")>]
    member json.addProperty(key,value) =
        match json with
        | Json.Object entries ->
            (key,value) :: entries
            |> Json.Object
        | _ -> ArgumentOutOfRangeException "only for Json.Object" |> raise

    //保留键位置，新增附后
    [<Obsolete("Json.assign")>]
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

    member json.getBooleanValue() =
        match json with
        | Json.True -> true
        | Json.False -> false
        | _ -> failwith "only for Json.Boolean"

    member json.boolValue with get() = json.getBooleanValue()

    member object.setProperty(propertyName:string, value:Json) =
        match object with
        | Json.Object pairs ->
            let pairs =
                pairs
                |> List.map(fun (k,v)->
                    let v =
                        if k = propertyName then
                            value
                        else
                            v
                    k,v
                )
            Json.Object pairs
        | _ -> failwith "string index is only for object."

    member array.setElement(index:int, value:Json) =
        match array with
        | Json.Array ls -> 
            let ls =
                ls
                |> List.mapi(fun i e ->
                    if i = index then
                        value
                    else
                        e
                )
            Json.Array ls
        | _ -> failwith "int index is only for array."

    static member just() = Json.Null
    static member just(value:bool) = if value then Json.True else Json.False
    static member just(value:string) = Json.String value

    static member just(value:float) = Json.Number value
    static member just(value:int) = Json.just(float value)

    static member just(entries:list<string*Json>) = Json.Object(entries)
    static member just([<ParamArray>] entries: (string*Json)[]) = Json.just(List.ofArray entries)

    static member just(elements:list<Json>) = Json.Array(elements)
    static member just([<ParamArray>] elements: Json[]) = Json.just(List.ofArray elements)
