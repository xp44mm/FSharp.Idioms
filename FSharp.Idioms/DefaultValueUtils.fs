module FSharp.Idioms.DefaultValueUtils

open FSharp.Idioms.DefaultValues
open System

let defaultValueDynamic (ty:Type) = 
    DefaultValueDriver.defaultValue DefaultValues.getDefaults ty

let defaultValue<'t> = 
    defaultValueDynamic typeof<'t> :?> 't
