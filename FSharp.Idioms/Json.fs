module FSharp.Idioms.Json

open FSharp.Idioms.Jsons

open System

/// 
let stringifyUnquotedJson (json:Json) = JsonRender.stringifyUnquotedJson json

let stringify (json:Json) = JsonRender.stringifyNormalJson json

/// obj -> Json 
let readDynamic (ty:Type) (value:obj) =
    JsonReaderApp.readDynamic ty value

/// 't -> Json
let read<'t> (value:'t) = readDynamic typeof<'t> value

let writeDynamic (ty:Type) (json:Json) =
    JsonWriterApp.writeDynamic ty json

/// Json -> 't
let write<'t> (json:Json) =
    writeDynamic typeof<'t> json 
    :?> 't

