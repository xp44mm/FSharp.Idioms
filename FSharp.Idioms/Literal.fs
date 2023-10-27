module FSharp.Idioms.Literal
open FSharp.Idioms.Literals

open System

let stringifyTypeDynamic (ty:Type) = TypeRender.stringifyParen TypeRender.stringifies 0 ty

let stringifyType<'t> = stringifyTypeDynamic typeof<'t>

/// print dynamic value
let stringifyDynamic (ty:Type) (value:obj) = ParenRender.instanceToString 0 ty value

/// print generic value
let stringify<'t> (value:'t) = stringifyDynamic typeof<'t> value

