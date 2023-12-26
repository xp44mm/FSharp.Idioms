module FSharp.Idioms.Literal

open System
open FSharp.Idioms.Literals
open FSharp.Idioms.Zeros

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

let defaultofDynamic : Type->obj = ZeroUtils.getZero ZeroUtils.tries

let defaultof<'t> = defaultofDynamic typeof<'t> :?> 't
