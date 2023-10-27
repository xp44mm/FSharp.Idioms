module FSharp.Idioms.Literals.Render

open System

/// print dynamic value
let stringifyObj (tp:Type) (value:obj) = ParenRender.instanceToString 0 tp value

/// print generic value
let stringify<'t> (value:'t) = 
    let tp = typeof<'t>
    stringifyObj tp value

let stringifyTypeDynamic (ty:Type) = TypeRender.stringifyParen TypeRender.stringifies 0 ty

let stringifyType<'t> = stringifyTypeDynamic typeof<'t>
