module FSharp.Idioms.JsonNode

open System
open System.Text.Json
open System.Text.Json.Nodes

open FSharp.Idioms.Jsons

// F# DU -> System.Text.Json.JsonNode
let rec fromDU (json: Json) : JsonNode =
    match json with
    | Json.Null -> JsonValue.Create<obj>(null) // 必须指定类型才能表示为 null
    | Json.False -> JsonValue.Create(false)
    | Json.True -> JsonValue.Create(true)
    | Json.String s -> JsonValue.Create(s)
    | Json.Number n -> JsonValue.Create(n)
    | Json.Object entries ->
        let obj = JsonObject()
        for (key, value) in entries do
            obj.Add(key, fromDU value)
        obj :> JsonNode
    | Json.Array elements ->
        let arr = JsonArray()
        for el in elements do
            arr.Add(fromDU el)
        arr :> JsonNode

// System.Text.Json.JsonNode -> F# DU
let rec toDU (node: JsonNode) : Json =
    match node with
    | null -> Json.Null
    | _ ->
        match node.GetValueKind() with
        | JsonValueKind.Null -> Json.Null
        | JsonValueKind.False -> Json.False
        | JsonValueKind.True -> Json.True
        | JsonValueKind.String -> Json.String(node.GetValue<string>())
        | JsonValueKind.Number -> Json.Number(node.GetValue<double>()) // 与你的 float 匹配
        | JsonValueKind.Object ->
            let obj = node.AsObject()
            let entries =
                [
                    for prop in obj do
                        // 注意：prop.Value 可能为 null，但 fromJsonNode 会正确处理
                        yield (prop.Key, toDU prop.Value)
                ]
            Json.Object entries
        | JsonValueKind.Array ->
            let arr = node.AsArray()
            let elements =
                [
                    for item in arr do
                        yield toDU item
                ]
            Json.Array elements
        | kind -> failwithf "不支持的 JSON 类型: %A" kind
