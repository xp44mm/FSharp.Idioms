module FSharp.Idioms.Literal

open System
open FSharp.Idioms.Literals

/// print dynamic value
let stringifyTypeDynamic (tp:Type) = 
    tp
    |> TypePrinterUtils.typeStringify TypePrinterUtils.printers 0

/// print generic value
let stringifyType<'t> = stringifyTypeDynamic typeof<'t>

/// print dynamic value
let stringifyDynamic (tp:Type) (value:obj) = 
    (tp,value)
    ||> ValuePrinterUtils.valueStringify ValuePrinterUtils.printers 0

/// print generic value
let stringify<'t> (value:'t) = 
    stringifyDynamic typeof<'t> value

open FSharp.Idioms.DefaultValues

let defaultValueDynamic (ty:Type) = 
    ty
    |> DefaultValueGetterUtils.getDefaultValue DefaultValueGetterUtils.DefaultValueGetters 

let defaultValue<'t> = 
    defaultValueDynamic typeof<'t> 
    :?> 't
