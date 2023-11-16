module FSharp.Idioms.Literals.ValuePrinters
open FSharp.Idioms.Literals.Paren

open System
open System.Globalization
open FSharp.Reflection
open FSharp.Idioms

type Loop = Type -> obj -> int -> string

let tryBool =
    fun (ty:Type) ->
        if ty = typeof<bool> then
            Some(fun (loop:Loop) (value:obj) (precContext:int) ->
            if unbox<bool> value then "true" else "false"
            )
        else None

//let boolValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = 
//    }
let tryString =
    fun (ty:Type) ->
        if ty = typeof<string> then
            Some(fun loop (value:obj) (precContext:int) ->
                unbox<string> value
                |> FSharpCodeUtils.toStringLiteral
            )
        else None

//let stringValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let tryChar =
    fun (ty:Type) ->
        if ty = typeof<char> then
            Some(fun loop (value:obj) (precContext:int) ->
                unbox<char> value
                |> FSharpCodeUtils.toCharLiteral
            )
        else None

//let charValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let trySbyte =
    fun (ty:Type) ->
        if ty = typeof<sbyte> then
            Some(fun loop (value:obj) (precContext:int) ->
                let value = unbox<sbyte> value
                Convert.ToString value + "y"
            )
        else None

//let sbyteValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryByte =
    fun (ty:Type) ->
        if ty = typeof<byte> then
            Some(fun loop (value:obj) (precContext:int) ->
                let value = unbox<byte> value
                Convert.ToString value + "uy"
            )
        else None

//let byteValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let tryInt16 =
    fun (ty:Type) ->
        if ty = typeof<int16> then
            Some(fun loop (value:obj) (precContext:int) ->
            let value = unbox<int16> value
            Convert.ToString value + "s"
            )
        else None

//let ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let tryUint16 =
    fun (ty:Type) ->
        if ty = typeof<uint16> then
            Some(fun loop (value:obj) (precContext:int) ->
            let value = unbox<uint16> value
            Convert.ToString value + "us"
            )
        else None

//let uint16ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryInt =
    fun (ty:Type) ->
        if ty = typeof<int> then
            Some(fun loop (value:obj) (precContext:int) ->
            let value = unbox<int> value
            Convert.ToString value
            )
        else None

//let intValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryUint32 =
    fun (ty:Type) ->
        if ty = typeof<uint32> then
            Some(fun loop (value:obj) (precContext:int) ->
            let value = unbox<uint32> value
            Convert.ToString value  + "u"
            )
        else None

//let uint32ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryInt64 =
    fun (ty:Type) ->
        if ty = typeof<int64> then
            Some(fun loop (value:obj) (precContext:int) ->
            let value = unbox<int64> value
            Convert.ToString value  + "L"
            )
        else None

//let int64ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryUint64 =
    fun (ty:Type) ->
    if ty = typeof<uint64> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<uint64> value
        Convert.ToString value + "UL"
        )
    else None

//let uint64ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let trySingle =
    fun (ty:Type) ->
    if ty = typeof<single> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<single> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G9"
        s + "f"
        )
    else None

//let singleValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryFloat =
    fun (ty:Type) ->
    if ty = typeof<float> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<float> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G17"
        FSharpCodeUtils.decimalPoint s
        )
    else None

//let floatValuePrinter (ty:Type) =
//    {
//    finder = ty = typeof<>
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryDecimal =
    fun (ty:Type) ->
    if ty = typeof<decimal> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<decimal> value
        Convert.ToString value + "M"
        )
    else None

//let decimalValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryNativeint =
    fun (ty:Type) ->
    if ty = typeof<nativeint> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<nativeint> value
        Convert.ToString value + "n"
        )
    else None

//let nativeintValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryUnativeint =
    fun (ty:Type) ->
    if ty = typeof<unativeint> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<unativeint> value
        Convert.ToString value + "un"
        )
    else None

//let unativeintValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let tryBigint =
    fun (ty:Type) ->
    if ty = typeof<bigint> then
        Some(fun loop (value:obj) (precContext:int) ->
        let value = unbox<bigint> value
        Convert.ToString value + "I"
        )
    else None

//let bigintValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }

let tryUnit =
    fun (ty:Type) ->
    if ty = typeof<unit> then
        Some(fun loop (value:obj) (precContext:int) ->
        "()"
        )
    else None

//let unitValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) -> 
//    }
let tryDBNull =
    fun (ty:Type) ->
    if ty = typeof<DBNull> then
        Some(fun loop (value:obj) (precContext:int) ->
        "DBNull.Value"
        )
    else None

//let ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
        
//    }
let tryGuid =
    fun (ty:Type) ->
    if ty = typeof<Guid> then
        Some(fun loop (value:obj) (precContext:int) ->
        let id = unbox<Guid> value
        sprintf "Guid(\"%s\")" <| id.ToString()
        |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let ValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryEnum =
    fun (ty:Type) ->
    if ty.IsEnum then
        Some(fun loop (value:obj) (precContext:int) ->
        if ty.IsDefined(typeof<FlagsAttribute>,false) then
            let reader = EnumType.readFlags ty
            reader value
            |> Array.map(fun enm -> sprintf "%s.%s" ty.Name enm )
            |> String.concat "|||"
            |> putParen precContext valuePrecedences.["|||"]
        else
            Enum.GetName(ty,value)
            |> sprintf "%s.%s" ty.Name
            |> putParen precContext valuePrecedences.["."]
        )
    else None

//let enumValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryTimeSpan =
    fun (ty:Type) ->
    if ty = typeof<TimeSpan> then
        Some(fun (loop:Loop) (value:obj) (precContext:int) ->
        let v = unbox<TimeSpan> value
        [
            v.Days
            v.Hours
            v.Minutes
            v.Seconds
            v.Milliseconds
        ]
        |> List.map(fun i -> Convert.ToString i)
        |> String.concat ","
        |> sprintf "TimeSpan(%s)"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let timeSpanValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryDateTimeOffset =
    fun (ty:Type) ->
    if ty = typeof<DateTimeOffset> then
        Some(fun (loop:Loop) (value:obj) (precContext:int) ->
        let thisDate = unbox<DateTimeOffset> value
        [
            thisDate.Year       .ToString()
            thisDate.Month      .ToString()
            thisDate.Day        .ToString()
            thisDate.Hour       .ToString()
            thisDate.Minute     .ToString()
            thisDate.Second     .ToString()
            thisDate.Millisecond.ToString()
            thisDate.Offset |> loop typeof<TimeSpan> <| valuePrecedences.[","]
        ]
        |> String.concat ","
        |> sprintf "DateTimeOffset(%s)"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let dateTimeOffsetValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryDateTime =
    fun (ty:Type) ->
    if ty = typeof<DateTime> then
        Some(fun (loop:Loop) (value:obj) (precContext:int) ->
        let dt = unbox<DateTime> value
        loop typeof<DateTimeOffset> (DateTimeOffset dt) precContext
        )
    else None

//let dateTimeValuePrinter (ty:Type) =
//    {
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryNullable =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Nullable<_>>.GetGenericTypeDefinition() then
        Some(fun loop (value:obj) (precContext:int) ->
        match value with
        | null -> "Nullable()"
        | _ ->
            let underlyingType = ty.GenericTypeArguments.[0]
            loop underlyingType value valuePrecedences.[" "]
            |> sprintf "Nullable %s"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None
    
//let nullableValuePrinter (ty:Type) =
//    {
//    finder = 
        
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryArray =
    fun (ty:Type) ->
    if ty.IsArray && ty.GetArrayRank() = 1 then
        Some(fun loop (value:obj) (precContext:int) ->
        let elemType = ty.GetElementType()
        let reader = ArrayType.toArray // ty
        let elements = reader value

        elements
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "[|%s|]"  //一定無需加括號
        )
    else None

//let arrayValuePrinter (ty:Type) =
//    {
//    finder =
//    print = fun loop value prec ->
//    }

let tryList =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition() then
        Some(fun loop (value:obj) (precContext:int) ->
        let elemType = ty.GenericTypeArguments.[0]

        value
        |> IEnumerableType.toArray
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "[%s]" //一定無需加括號
        )
    else None

//let listValuePrinter (ty:Type) =
//    { 
//    finder = 
//    print = fun loop (value:obj) (precContext:int) ->
//        //let reader = ListType.readList ty
//    }
let trySet =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Set<_>>.GetGenericTypeDefinition() then
        Some(fun loop (value:obj) (precContext:int) ->
        let elemType = ty.GenericTypeArguments.[0]

        IEnumerableType.toArray value
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "set [%s]"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None


//let setValuePrinter (ty:Type) =
//    {
//    finder =
//    print = fun loop value precContext ->
//        //let reader = SetType.readSet ty
//    }
let tryHashSet =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<System.Collections.Generic.HashSet<_>>.GetGenericTypeDefinition() then
        Some(fun loop (value:obj) (precContext:int) ->
        let elemType = ty.GenericTypeArguments.[0]

        value
        |> IEnumerableType.toArray
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "HashSet [%s]"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let hashsetValuePrinter (ty:Type) =
//    {
//    finder =
        
//    print = fun loop (value:obj) (precContext:int) ->


//    }
let tryMap =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Map<_,_>>.GetGenericTypeDefinition() then
        let reader = MapType.toArray ty
        Some(fun (loop:Loop) (value:obj) (precContext:int) ->
        let elements = reader value
        let kvty = elements.GetType().GetElementType() //(k,v) tuple type

        elements
        |> Array.map(fun kvp -> loop kvty kvp valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "Map [%s]"
        |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let mapValuePrinter (ty:Type) =
//    {
//    finder =
//        //ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<>
//    print = fun loop (value:obj) (precContext:int) ->
//    }
let tryTuple =
    fun (ty:Type) ->
    if FSharpType.IsTuple ty then
        let elementTypes = FSharpType.GetTupleElements ty
        let reader = FSharpValue.PreComputeTupleReader ty

        Some(fun loop (value:obj) (precContext:int) ->
            let fields = reader value

            fields
            |> Array.zip elementTypes
            |> Array.map(fun(ftype,field)-> loop ftype field valuePrecedences.[","])
            |> String.concat ","
            |> putParen precContext valuePrecedences.[","]
        )
    else None

//let tupleValuePrinter (ty:Type) =
//    {
//        finder = 
//        print = fun loop (value:obj) (precContext:int) ->
//    }
let tryUnion =
    fun (ty:Type) ->
    if FSharpType.IsUnion ty then
        let reader = UnionType.readUnion ty
        let qa = UnionType.getQualifiedAccess ty
        Some(fun loop (value:obj) (precContext:int) ->
            let name,fields = reader value
            let name = qa + name

            match fields with
            | [||] -> name
            | [|ftype,field|] ->
                let payload:string = loop ftype field valuePrecedences.[" "]
                if payload.Chars 0 = '(' then name + payload else name + " " + payload
                |> putParen precContext valuePrecedences.[" "]
            | _ ->
                fields
                |> Array.map(fun(ftype,field)-> loop ftype field valuePrecedences.[","])
                |> String.concat ","
                |> sprintf "%s(%s)" name
                |> putParen precContext valuePrecedences.[" "]
        )
    else None

//let unionValuePrinter (ty:Type) =
//    {
//        finder = 
//        print = fun loop (value:obj) (precContext:int) ->
//    }
let tryRecord =
    fun (ty:Type) ->
    if FSharpType.IsRecord ty then
        let pis = FSharpType.GetRecordFields ty
        let reader = FSharpValue.PreComputeRecordReader ty
        Some(fun loop (value:obj) (precContext:int) ->
            let fields = reader value

            fields
            |> Array.zip pis
            |> Array.map(fun(pi,value)->
                let nm = 
                    if FSharpCodeUtils.isIdentifier pi.Name then
                        pi.Name 
                    else $"``{pi.Name}``"

                let value = loop pi.PropertyType value 0
                $"{nm}= {value}"
            )
            |> String.concat ";"
            |> sprintf "{%s}"
        )
    else None

//let recordValuePrinter (ty:Type) =
//    {
//        finder = 
//        print = fun loop (value:obj) (precContext:int) ->
//    }
let tryType =
    fun (ty:Type) ->
    if ty = typeof<Type> then
        Some(fun loop (value:obj) (precContext:int) ->
        value
        |> unbox<Type>
        |> TypePrinterApp.typeStringify TypePrinterApp.typePrinters <| 0
        |> sprintf "typeof<%s>"
        )
    else None

//let TypeValuePrinter ty = {
//    finder = 
//    print = fun loop value prec ->
//    }
