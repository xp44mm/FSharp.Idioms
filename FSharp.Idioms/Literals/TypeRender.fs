module FSharp.Idioms.Literals.TypeRender

open System
open Microsoft.FSharp.Reflection
open System.Text.RegularExpressions
open FSharp.Idioms
//
let precedences =
    [
        []
        [":"]
        [","]
        ["->"]
        ["*"]
        ["<>";"[]"]
        ["max"]
    ]
    |> List.mapi(fun i ls ->
        ls
        |> List.map(fun sym -> sym,i*10)
    )
    |> List.concat
    |> Map.ofList

///打印简单的类型
let printSimpleType (ty:Type) =
    if ty.IsEnum then ty.Name
    elif ty = typeof<unit>       then "unit"
    elif ty = typeof<bool>       then "bool"
    elif ty = typeof<string>     then "string"
    elif ty = typeof<char>       then "char"
    elif ty = typeof<sbyte>      then "sbyte"
    elif ty = typeof<byte>       then "byte"
    elif ty = typeof<int16>      then "int16"
    elif ty = typeof<uint16>     then "uint16"
    elif ty = typeof<int>        then "int"
    elif ty = typeof<uint32>     then "uint32"
    elif ty = typeof<int64>      then "int64"
    elif ty = typeof<uint64>     then "uint64"
    elif ty = typeof<single>     then "single"
    elif ty = typeof<float>      then "float"
    elif ty = typeof<decimal>    then "decimal"
    elif ty = typeof<nativeint>  then "nativeint"
    elif ty = typeof<unativeint> then "unativeint"
    elif ty = typeof<bigint>     then "bigint"
    else ty.Name

let ArrayTypeStringify (ty:Type) =
    if ty.IsArray && ty.GetArrayRank() = 1 then
        Some(fun loop prec ->
            let arrayPrec = precedences.["[]"]
            let elementType = ty.GetElementType()
            loop arrayPrec elementType + "[]"
            |> FSharpCodeUtils.putparen prec arrayPrec
        )
    else None

let TupleTypeStringify (ty:Type) =
    if FSharpType.IsTuple ty then
        Some(fun loop prec ->
            let tuplePrec = precedences.["*"]
            FSharpType.GetTupleElements ty
            |> Array.map(loop (tuplePrec+1))
            |> String.concat "*"
            |> FSharpCodeUtils.putparen prec tuplePrec
        )
    else None

let getGenericTypeName (type_name:string) =
    match type_name.Split('`').[0] with
    | "Void"         -> "unit"
    | "FSharpOption" -> "option"
    | "FSharpList"   -> "list"
    | "IEnumerable"  -> "seq"
    | "FSharpSet"    -> "Set"
    | "FSharpMap"    -> "Map"
    | "List"         -> "ResizeArray"
    | name -> name

let GenericTypeDefinitionStringify (ty:Type) =
    if ty.IsGenericType && ty.IsGenericTypeDefinition then
        Some(fun loop prec ->
            let name = getGenericTypeName ty.Name
            ty.GetGenericArguments()
            |> Array.filter(fun p -> p.IsGenericParameter)
            |> Array.sortBy(fun p -> p.GenericParameterPosition)
            |> Array.map(fun t -> "'" + t.Name)
            |> String.concat ","
            |> sprintf "%s<%s>" name
            |> FSharpCodeUtils.putparen prec precedences.["<>"]
        )
    else None

let GenericTypeStringify (ty:Type) =
    if ty.IsGenericType then
        Some(fun loop prec ->
            let name = getGenericTypeName ty.Name
            ty.GenericTypeArguments
            |> Array.map(loop precedences.[","])
            |> String.concat ","
            |> sprintf "%s<%s>" name
            |> FSharpCodeUtils.putparen prec precedences.["<>"]
        )
    else None

let AnonymousRecordTypeStringify (ty:Type) =
    if FSharpType.IsRecord ty && Regex.IsMatch(ty.Name,"^<>f__AnonymousType\d+`\d+$") then
        Some(fun loop prec ->
            FSharpType.GetRecordFields ty
            |> Array.map(fun pi -> sprintf "%s:%s" pi.Name <| loop precedences.[":"] pi.PropertyType)
            |> String.concat ";"
            |> sprintf "{|%s|}"
        )
    else None

let FunctionTypeStringify (ty:Type) =
    if FSharpType.IsFunction ty then
        Some(fun loop prec ->
            let domainType, rangeType = FSharpType.GetFunctionElements ty
            let funPrec = precedences.["->"]
            let domainType = loop (funPrec+1) domainType
            let rangeType  = loop (funPrec-1) rangeType
            sprintf "%s->%s" domainType rangeType
            |> FSharpCodeUtils.putparen prec funPrec
        )
    else None

let stringifies = [
    ArrayTypeStringify
    TupleTypeStringify
    AnonymousRecordTypeStringify
    FunctionTypeStringify
    GenericTypeDefinitionStringify
    GenericTypeStringify
]

/// 根据优先级确定表达式是否带括号
let rec stringifyParen (stringifies:#seq<Type->((int->Type->string)->int->string)option>) (prec:int) (ty:Type) =
    let stringify =
        stringifies
        |> Seq.tryPick(fun s -> s ty)
        |> Option.defaultValue(fun loop prec -> printSimpleType ty)

    stringify(stringifyParen stringifies) prec
