module FSharp.Idioms.Literals.TypePrinterUtils

open System
open FSharp.Idioms
open FSharp.Idioms.Literals.TypePrinterImpls

let printers = [
    enumTypePrinter
    unitTypePrinter
    boolTypePrinter
    stringTypePrinter
    charTypePrinter
    sbyteTypePrinter
    byteTypePrinter
    int16TypePrinter
    uint16TypePrinter
    intTypePrinter
    uint32TypePrinter
    int64TypePrinter
    uint64TypePrinter
    singleTypePrinter
    floatTypePrinter
    decimalTypePrinter
    nativeintTypePrinter
    unativeintTypePrinter
    bigintTypePrinter
    arrayTypePrinter
    tupleTypePrinter
    AnonymousRecordTypePrinter
    FunctionTypePrinter
    GenericTypeDefinitionTypePrinter
    GenericTypePrinter
    ]

/// 根据优先级确定表达式是否带括号
let rec typeStringify (printers:list<int->Type->TypePrinter>) (prec:int) (ty:Type) =
    let pickedStringify =
        printers
        |> Seq.tryPick(fun getPrinter -> 
            let printer = getPrinter prec ty
            if printer.finder then
                Some printer.print
            else None
            )
        |> Option.defaultValue(fun loop -> ty.Name)

    pickedStringify (typeStringify printers)

