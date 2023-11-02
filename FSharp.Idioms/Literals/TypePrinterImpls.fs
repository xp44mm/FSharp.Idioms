module FSharp.Idioms.Literals.TypePrinterImpls

open System
open System.Text.RegularExpressions
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Literals.Paren

let enumTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty.IsEnum
    print = fun loop -> ty.Name
    }
let unitTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<unit>
    print = fun loop -> "unit"
    }
let boolTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<bool>
    print = fun loop -> "bool"
    }
let stringTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<string>
    print = fun loop -> "string"
    }
let charTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<char>
    print = fun loop -> "char"
    }
let sbyteTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<sbyte>
    print = fun loop -> "sbyte"
    }
let byteTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<byte>
    print = fun loop -> "byte"
    }
let int16TypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<int16>
    print = fun loop -> "int16"
    }
let uint16TypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<uint16>
    print = fun loop -> "uint16"
    }
let intTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<int>
    print = fun loop -> "int"
    }
let uint32TypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<uint32>
    print = fun loop -> "uint32"
    }
let int64TypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<int64>
    print = fun loop -> "int64"
    }
let uint64TypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<uint64>
    print = fun loop -> "uint64"
    }
let singleTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<single>
    print = fun loop -> "single"
    }
let floatTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<float>
    print = fun loop -> "float"
    }
let decimalTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<decimal>
    print = fun loop -> "decimal"
    }
let nativeintTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<nativeint>
    print = fun loop -> "nativeint"
    }
let unativeintTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<unativeint>
    print = fun loop -> "unativeint"
    }
let bigintTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty = typeof<bigint>
    print = fun loop -> "bigint"
    }
let arrayTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty.IsArray && ty.GetArrayRank() = 1
    print = fun loop ->
        let arrayPrec = typePrecedences.["[]"]
        let elemType = ty.GetElementType()
        loop arrayPrec elemType + "[]"
        |> putParen prec arrayPrec
    }

let tupleTypePrinter (prec:int) (ty:Type) = 
    {
    finder = FSharpType.IsTuple ty
    print = fun loop ->
        let tuplePrec = typePrecedences.["*"]

        FSharpType.GetTupleElements ty
        |> Array.map(loop (tuplePrec+1))
        |> String.concat "*"
        |> putParen prec tuplePrec
    }
let AnonymousRecordTypePrinter (prec:int) (ty:Type) = 
    {
    finder = 
        FSharpType.IsRecord ty && 
        Regex.IsMatch(ty.Name,"^<>f__AnonymousType\d+`\d+$")
    print = fun loop ->
        FSharpType.GetRecordFields ty
        |> Array.map(fun pi -> 
            loop typePrecedences.[":"] pi.PropertyType
            |> sprintf "%s:%s" pi.Name)
        |> String.concat ";"
        |> sprintf "{|%s|}"
    }

let FunctionTypePrinter (prec:int) (ty:Type) = 
    {
    finder = FSharpType.IsFunction ty
    print = fun loop ->
        let domainType,rangeType = FSharpType.GetFunctionElements ty
        let funPrec = typePrecedences.["->"]
        let domainType = loop (funPrec+1) domainType
        let rangeType  = loop (funPrec-1) rangeType
        sprintf "%s->%s" domainType rangeType
        |> putParen prec funPrec
    }


let GenericTypeDefinitionTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty.IsGenericType && ty.IsGenericTypeDefinition
    print = fun loop ->
        let name = FSharpCodeUtils.getGenericTypeName ty.Name

        ty.GetGenericArguments()
        |> Array.filter(fun p -> p.IsGenericParameter)
        |> Array.sortBy(fun p -> p.GenericParameterPosition)
        |> Array.map(fun t -> "'" + t.Name)
        |> String.concat ","
        |> sprintf "%s<%s>" name
        |> putParen prec typePrecedences.["<>"]
    }

let GenericTypePrinter (prec:int) (ty:Type) = 
    {
    finder = ty.IsGenericType
    print = fun loop ->
        let name = FSharpCodeUtils.getGenericTypeName ty.Name

        ty.GenericTypeArguments
        |> Array.map(loop typePrecedences.[","])
        |> String.concat ","
        |> sprintf "%s<%s>" name
        |> putParen prec typePrecedences.["<>"]
    }

