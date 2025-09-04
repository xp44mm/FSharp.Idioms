module FSharp.Idioms.Json

open FSharp.Idioms.Jsons

open System

let boolean (b:bool) = if b then Json.True else Json.False

/// string -> "string"
let quote: string->string = JsonString.quote

/// "string" -> string
let unquote: string->string = JsonString.unquote

let print: Json->string = JsonRender.stringifyNormalJson

/// 
let printUnquotedJson: Json->string = JsonRender.stringifyUnquotedJson

/// fromObj: obj -> Json 
let fromObj: Type -> obj -> Json = JsonReaderApp.readDynamic

/// from: 't -> Json
let from<'t>(value:'t) = fromObj typeof<'t> value

// toObj
let toObj: Type -> Json -> obj = JsonWriterApp.writeDynamic

/// deserialize to: Json -> 't
let deserialize<'t>(json:Json) =
    toObj typeof<'t> json 
    :?> 't

[<Obsolete("rename deserialize")>]
let cast<'t>(json:Json) = deserialize<'t>(json:Json)

/// 找到记录中第一个属性
let tryCapture (field:string) (records:Json list) =
    records
    |> List.tryPick(fun h ->
        if h.hasProperty field then
            Some h.[field]
        else None
    )
