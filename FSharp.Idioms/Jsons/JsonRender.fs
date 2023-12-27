module FSharp.Idioms.Jsons.JsonRender
open FSharp.Idioms

open System
open System.Text.RegularExpressions

let rec stringifyNormalJson (json:Json) = 
    match json with
    | Json.Object pairs ->
        pairs
        |> List.map(fun(k,v)->
           JsonString.quote k + ":" + stringifyNormalJson v
        )
        |> String.concat ","
        |> sprintf "{%s}"

    | Json.Array ls ->
        ls
        |> List.map(stringifyNormalJson)
        |> String.concat ","
        |> sprintf "[%s]"

    | Json.Null -> "null"
    | Json.False -> "false"
    | Json.True -> "true"
    | Json.String x -> JsonString.quote x
    | Json.Number c -> Convert.ToString c
    //| Json.Decimal c -> Convert.ToString c

///用于unquoted json 模式，如果需要加引号
let stringifyKey x =
    if String.IsNullOrWhiteSpace x || 
        Regex.IsMatch(x,@"[,:{}[\]""\u0000-\u001F\u007F]|(^\u0020)|(\u0020$)") then
        JsonString.quote x
    else
        x
///用于unquoted json 模式，如果需要加引号    
let stringifyStringValue x =
    if x = "true" || x = "false" || x = "null" then
        JsonString.quote x
    elif Regex.IsMatch(x, @"^[-+]?\d+(\.\d+)?([eE][-+]?\d+)?$") then
        JsonString.quote x
    else
        stringifyKey x

/// unquoted json 模式，如果没有歧义，省略引号
let rec stringifyUnquotedJson (json:Json)= 
    match json with
    | Json.Object pairs ->
        pairs
        |> List.map(fun(k,v)->
           stringifyKey k + ":" + stringifyUnquotedJson v
        )
        |> String.concat ","
        |> sprintf "{%s}"

    | Json.Array ls ->
        ls
        |> List.map(stringifyUnquotedJson)
        |> String.concat ","
        |> sprintf "[%s]"

    | Json.Null -> "null"
    | Json.False -> "false"
    | Json.True -> "true"
    | Json.String x -> stringifyStringValue x
    | Json.Number c -> Convert.ToString c
    //| Json.Decimal c -> Convert.ToString c

