module FSharp.Idioms.JsonElement

open System.Text.Json
open FSharp.Idioms.Jsons

/// 将 System.Text.Json.JsonElement 转换为 F# 的 Json DU
let rec toDU (elem: JsonElement) : Json =
    match elem.ValueKind with
    | JsonValueKind.Null -> Json.Null
    | JsonValueKind.False -> Json.False
    | JsonValueKind.True -> Json.True
    | JsonValueKind.String -> Json.String(elem.GetString())
    | JsonValueKind.Number -> Json.Number(elem.GetDouble())
    | JsonValueKind.Object ->
        let entries =
            [
                for prop in elem.EnumerateObject() do
                    yield (prop.Name, toDU prop.Value)
            ]
        Json.Object entries
    | JsonValueKind.Array ->
        let elements =
            [
                for item in elem.EnumerateArray() do
                    yield toDU item
            ]
        Json.Array elements
    | _ -> failwithf "不支持的 JSON 类型: %A" elem.ValueKind

