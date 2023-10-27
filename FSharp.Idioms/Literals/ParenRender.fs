module FSharp.Idioms.Literals.ParenRender

open System
open System.Globalization
open FSharp.Reflection
open FSharp.Idioms

let decimalpoint (s:string) =
    if s.Contains "." || s.Contains "E" || s.Contains "e" then
        s
    else
        s + ".0"

//优先级高的先计算
let precedences =
    [
        []
        [";"]
        [","]
        ["|||"]
        [" "]
        ["."]
        ["max"]
    ]
    |> List.mapi(fun i ls ->
        ls
        |> List.map(fun sym -> sym,i*10)
    )
    |> List.concat
    |> Map.ofList

///可能会有Nullable<>時，顯式使用明確的typeof<>提供类型
let rec instanceToString (precContext:int) (ty:Type) (value:obj) =
    if ty = typeof<bool> then
        let b = unbox<bool> value
        if b then "true" else "false"

    elif ty = typeof<string> then
        unbox<string> value
        |> FSharpCodeUtils.toStringLiteral

    elif ty = typeof<char> then
        unbox<char> value
        |> FSharpCodeUtils.toCharLiteral

    elif ty = typeof<sbyte> then
        let value = unbox<sbyte> value
        Convert.ToString value + "y"

    elif ty = typeof<byte> then
        let value = unbox<byte> value
        Convert.ToString value + "uy"

    elif ty = typeof<int16> then
        let value = unbox<int16> value
        Convert.ToString value  + "s"

    elif ty = typeof<uint16> then
        let value = unbox<uint16> value
        Convert.ToString value  + "us"

    elif ty = typeof<int> then
        let value = unbox<int> value
        Convert.ToString value

    elif ty = typeof<uint32> then
        let value = unbox<uint32> value
        Convert.ToString value  + "u"

    elif ty = typeof<int64> then
        let value = unbox<int64> value
        Convert.ToString value  + "L"

    elif ty = typeof<uint64> then
        let value = unbox<uint64> value
        Convert.ToString value + "UL"

    elif ty = typeof<single> then
        let value = unbox<single> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G9"
        decimalpoint s + "f"

    elif ty = typeof<float> then
        let value = unbox<float> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G17"
        decimalpoint s

    elif ty = typeof<decimal> then
        let value = unbox<decimal> value
        Convert.ToString value + "M"

    elif ty = typeof<nativeint> then
        let value = unbox<nativeint> value
        Convert.ToString value + "n"

    elif ty = typeof<unativeint> then
        let value = unbox<unativeint> value
        Convert.ToString value + "un"

    elif ty = typeof<bigint> then
        let value = unbox<bigint> value
        Convert.ToString value + "I"

    elif ty = typeof<DBNull> || DBNull.Value.Equals value then
        "DBNull.Value"
    elif ty = typeof<TimeSpan> then
        let tspan = unbox<TimeSpan> value

        [
            tspan.Days
            tspan.Hours
            tspan.Minutes
            tspan.Seconds
            tspan.Milliseconds
        ]
        |> List.map(fun i -> i.ToString())
        |> String.concat ","
        |> sprintf "TimeSpan(%s)"
        |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty = typeof<DateTimeOffset> then
        let thisDate = unbox<DateTimeOffset> value
        [
            thisDate.Year       .ToString()
            thisDate.Month      .ToString()
            thisDate.Day        .ToString()
            thisDate.Hour       .ToString()
            thisDate.Minute     .ToString()
            thisDate.Second     .ToString()
            thisDate.Millisecond.ToString()
            thisDate.Offset |> instanceToString precedences.[","] typeof<TimeSpan>
        ]
        |> String.concat ","
        |> sprintf "DateTimeOffset(%s)"
        |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty = typeof<DateTime> then
        let dt = unbox<DateTime> value
        instanceToString precContext typeof<DateTimeOffset> (DateTimeOffset dt)

    elif ty = typeof<Guid> then
        let id = unbox<Guid> value
        sprintf "Guid(\"%s\")" <| id.ToString()
        |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty.IsEnum then
        if ty.IsDefined(typeof<FlagsAttribute>,false) then
            let reader = EnumType.readFlags ty
            reader value
            |> Array.map(fun enm -> sprintf "%s.%s" ty.Name enm )
            |> String.concat "|||"
            |> FSharpCodeUtils.putparen precContext precedences.["|||"]
        else
            Enum.GetName(ty,value)
            |> sprintf "%s.%s" ty.Name
            |> FSharpCodeUtils.putparen precContext precedences.["."]

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
        if value = null then
            "Nullable()"
        else
            let underlyingType = ty.GenericTypeArguments.[0]
            instanceToString precedences.[" "] underlyingType value
            |> sprintf "Nullable %s"
        |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty.IsArray && ty.GetArrayRank() = 1 then
        let reader = ArrayType.readArray ty
        let elemType, elements = reader value

        arrayToString elemType elements
        |> sprintf "[|%s|]"  //一定無需加括號

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>> then
        let reader = ListType.readList ty
        let elemType, elements = reader value

        arrayToString elemType elements
        |> sprintf "[%s]" //一定無需加括號

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>> then
        let reader = SetType.readSet ty
        let elementType, elements = reader value
        if  Array.isEmpty elements then
            "set []"
        else
            arrayToString elementType elements
            |> fun content ->
                String.Format("set [{0}]",content)
            |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>> then 
        let reader = MapType.readMap ty
        let tupleType, elements = reader value
        if Array.isEmpty elements then
            "Map []"
        else
            arrayToString tupleType elements
            |> fun content ->
                String.Format("Map [{0}]",content)
            |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<System.Collections.Generic.HashSet<_>> then
        let reader = SeqType.seqReader ty
        let elements = reader value
        let elementType = ty.GenericTypeArguments.[0]
        if  Array.isEmpty elements then
            "HashSet []"
        else
            arrayToString elementType elements
            |> fun content -> $"HashSet [{content}]"
            |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif FSharpType.IsTuple ty then
        let reader = TupleType.readTuple ty
        let fields = reader value
                
        tupleToString fields
        |> FSharpCodeUtils.putparen precContext precedences.[","]

    elif FSharpType.IsUnion ty then
        let reader = UnionType.readUnion ty
        let name,fields = reader value
        let qa = UnionType.getQualifiedAccess ty
        let name = qa + name

        match fields with
        | [||] -> name
        | [|ftype,field|] ->
            let payload = instanceToString precedences.[" "] ftype field
            if payload.StartsWith("(") then name + payload else name + " " + payload
            |> FSharpCodeUtils.putparen precContext precedences.[" "]
        | _ ->
            fields
            |> tupleToString
            |> sprintf "%s(%s)" name
            |> FSharpCodeUtils.putparen precContext precedences.[" "]

    elif FSharpType.IsRecord ty then
        let reader = RecordType.readRecord ty
        let fields = reader value

        fields
        |> Array.map(fun(pi,value)->
            let nm = 
                if FSharpCodeUtils.isIdentifier pi.Name then
                    pi.Name 
                else String.Format("``{0}``",pi.Name)

            let value = instanceToString 0 pi.PropertyType value
            String.Format("{0}= {1}",nm,value)
        )
        |> String.concat ";"
        |> sprintf "{%s}"

    elif ty = typeof<Type> then
        TypeRender.stringifyParen TypeRender.stringifies 0 (unbox<Type>value)
        |> sprintf "typeof<%s>"

    elif value = null then
        "null" //没有类型信息，null,nullable,None都打印成null
    elif ty = typeof<obj> && value.GetType() <> typeof<obj> then
        instanceToString precContext (value.GetType()) value
    else
        sprintf "%A" value

and arrayToString elemType (elements:obj[]) =
    elements
    |> Array.map(instanceToString precedences.[";"] elemType)
    |> String.concat ";"

and tupleToString fields =
    fields
    |> Array.map(fun(ftype,field)-> instanceToString precedences.[","] ftype field)
    |> String.concat ","

// null, nullable, None 后台都是null，需要显示提供类型
let stringifyNullableType ty value = instanceToString 0 ty value

/// value <> null
let stringify value = 
    instanceToString 0 (value.GetType()) value

