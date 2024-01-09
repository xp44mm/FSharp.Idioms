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

/// obj -> Json 
let readDynamic: Type -> obj -> Json = JsonReaderApp.readDynamic

/// 't -> Json
let read<'t> (value:'t) = readDynamic typeof<'t> value

let writeDynamic: Type -> Json -> obj = JsonWriterApp.writeDynamic

/// Json -> 't
let write<'t> (json:Json) =
    writeDynamic typeof<'t> json 
    :?> 't

