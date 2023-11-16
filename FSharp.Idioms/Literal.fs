module FSharp.Idioms.Literal

open System
open FSharp.Idioms.Literals

/// print dynamic value
let stringifyTypeDynamic (tp:Type) = 
    (tp,0)
    ||> TypePrinterApp.typeStringify TypePrinterApp.typePrinters

/// print generic value
let stringifyType<'t> = stringifyTypeDynamic typeof<'t>

/// print dynamic value
let stringifyDynamic (tp:Type) (value:obj) = 
    (tp, value, 0)
    |||> ValuePrinterApp.valueStringify ValuePrinterApp.printers

/// print generic value
let stringify<'t> (value:'t) = stringifyDynamic typeof<'t> value

open FSharp.Idioms.Zeros

let zeroDynamic (ty:Type) = 
    ty
    |> ZeroUtils.getZero ZeroUtils.tries 

let zero<'t> = zeroDynamic typeof<'t> :?> 't
