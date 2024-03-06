module FSharp.Idioms.Json

open FSharp.Idioms.Jsons

open System

/// string -> "string"
let quote:string->string = JsonString.quote

/// "string" -> string
let unquote:string->string = JsonString.unquote

let print:Json->string = JsonRender.stringifyNormalJson

/// 
let printUnquotedJson:Json->string = JsonRender.stringifyUnquotedJson

/// fromObj: obj -> Json 
let fromObj: Type -> obj -> Json = JsonReaderApp.readDynamic

/// from: 't -> Json
let from<'t> (value:'t) = fromObj typeof<'t> value

// toObj
let toObj: Type -> Json -> obj = JsonWriterApp.writeDynamic

/// to: Json -> 't 
let To<'t> (json:Json) =
    toObj typeof<'t> json 
    :?> 't

