module FSharp.Idioms.Literal

open System
open FSharp.Idioms.Literals

/// print dynamic value
let stringifyTypeDynamic (tp:Type) = 
    (tp,0)
    ||> TypePrinterUtils.typeStringify TypePrinterUtils.printers

/// print generic value
let stringifyType<'t> = stringifyTypeDynamic typeof<'t>

/// print dynamic value
let stringifyDynamic (tp:Type) (value:obj) = 
    (tp, value, 0)
    |||> ValuePrinterUtils.valueStringify ValuePrinterUtils.printers

/// print generic value
let stringify<'t> (value:'t) = stringifyDynamic typeof<'t> value

open FSharp.Idioms.DefaultValues

let zeroDynamic (ty:Type) = 
    ty
    |> DefaultValueGetterUtils.getDefaultValue DefaultValueGetterUtils.DefaultValueGetters 

let zero<'t> = zeroDynamic typeof<'t> :?> 't
