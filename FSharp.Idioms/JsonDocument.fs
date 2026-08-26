module FSharp.Idioms.JsonDocument

open System.Text.Json

open FSharp.Idioms.Jsons

/// 从 JsonDocument 转换为 F# 的 Json DU
let toDU (doc: JsonDocument) : Json =
    JsonElement.toDU doc.RootElement
