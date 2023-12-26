module FSharp.Idioms.Jsons.JsonReaders // object -> Json
open FSharp.Idioms

open FSharp.Reflection

open System
open System.Collections.Generic

type Loop = Type -> obj -> Json

let tryBool = fun (ty:Type) ->
    if ty = typeof<bool> then
        Some(fun (loop:Loop) (value: obj) ->
        if unbox<bool> value then 
            Json.True
        else Json.False
        )
    else None

let tryString = fun (ty:Type) ->
    if ty = typeof<string> then
        Some(fun (loop:Loop) (value:obj) ->
            unbox<string> value
            |> Json.String
        )
    else None

let tryChar = fun (ty:Type) ->
    if ty = typeof<char> then
        Some(fun (loop:Loop) (value:obj) ->
            unbox<char> value
            |> Char.ToString
            |> Json.String
        )
    else None

let trySbyte = fun (ty:Type) ->
    if ty = typeof<sbyte> then
        Some(fun (loop:Loop) (value:obj) ->
            let value = unbox<sbyte> value
            Json.Number <|Convert.ToDouble value
        )
    else None

let tryByte = fun (ty:Type) ->
    if ty = typeof<byte> then
        Some(fun (loop:Loop) (value:obj) ->
            let value = unbox<byte> value
            Json.Number <|Convert.ToDouble value
        )
    else None

let tryInt16 = fun (ty:Type) ->
    if ty = typeof<int16> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<int16> value
        Json.Number <|Convert.ToDouble value
        )
    else None

let tryUint16 = fun (ty:Type) ->
    if ty = typeof<uint16> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<uint16> value
        Json.Number <|Convert.ToDouble value
        )
    else None

let tryInt = fun (ty:Type) ->
    if ty = typeof<int> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<int> value
        Json.Number <| Convert.ToDouble value
        )
    else None

let tryUint32 = fun (ty:Type) ->
    if ty = typeof<uint32> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<uint32> value
        Json.Number <| Convert.ToDouble value
        )
    else None

let tryInt64 = fun (ty:Type) ->
    if ty = typeof<int64> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<int64> value
        Json.Number <|Convert.ToDouble value
        )
    else None

let tryUint64 = fun (ty:Type) ->
    if ty = typeof<uint64> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<uint64> value
        Json.Number <|Convert.ToDouble value
        )
    else None

let trySingle = fun (ty:Type) ->
    if ty = typeof<single> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<single> value
        let s = Math.Round(float value,8)
        Json.Number s
        )
    else None

let tryFloat = fun (ty:Type) ->
    if ty = typeof<float> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<float> value
        Json.Number <|Convert.ToDouble value
        )
    else None

let tryDecimal = fun (ty:Type) ->
    if ty = typeof<decimal> then
        Some(fun (loop:Loop) (value:obj) ->
        Json.Number <|Convert.ToDouble value
        )
    else None

let tryNativeint = fun (ty:Type) ->
    if ty = typeof<nativeint> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<nativeint> value
        Json.Number <| float(value.ToInt64())
        )
    else None

let tryUnativeint = fun (ty:Type) ->
    if ty = typeof<unativeint> then
        Some(fun (loop:Loop) (value:obj) ->
        let value = unbox<unativeint> value
        Json.Number <| float(value.ToUInt64())
        )
    else None

let tryUnit = fun (ty:Type) ->
    if ty = typeof<unit> then
        Some(fun (loop:Loop) (value:obj) ->
        Json.Array []
        )
    else None

let tryDBNull = fun (ty:Type) ->
    if ty = typeof<DBNull> then
        Some(fun (loop:Loop) (value:obj) ->
        if value = DBNull.Value then
            Json.Null
        else failwith $"{value}"
        )
    else None

let tryOption = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Option<_>>.GetGenericTypeDefinition() then
        let vty = ty.GenericTypeArguments.[0]
        let get_Value = OptionType.get_Value ty
        Some(fun (loop:Loop) (value: obj) -> 
            if value = null then
                Json.Array []
            else
                let x = loop vty (get_Value value)
                Json.Array [x]
        )
    else None

let tryNullable = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
        let vty = ty.GenericTypeArguments.[0]
        Some(fun (loop:Loop) (value: obj) -> 
            if value = null then
                Json.Null
            else
                loop vty value
        )
    else None

let tryEnum = fun (ty:Type) ->
    if ty.IsEnum then
        Some(fun (loop:Loop) (value:obj) ->
        if ty.IsDefined(typeof<FlagsAttribute>,false) then
            let reader = EnumType.readFlags ty
            reader value
            |> Array.map(Json.String)
            |> Array.toList
            |> Json.Array
        else
            Json.String <| Enum.GetName(ty,value)
        )
    else None
            
let tryArray = fun (ty:Type) ->
    if ty.IsArray && ty.GetArrayRank() = 1 then
        Some(fun (loop:Loop) (value:obj) ->
        let elemType = ty.GetElementType()
        let reader = ArrayType.toArray // ty
        let elements = reader value

        elements
        |> Array.map(loop elemType)
        |> List.ofArray
        |> Json.Array
        )
    else None

let tryIEnumerable = fun (ty:Type) ->
    match ty.GetInterface("IEnumerable`1") with
    | null -> None
    | ity ->
    let elemType = ity.GenericTypeArguments.[0]
    if elemType.IsGenericType && elemType.GetGenericTypeDefinition() = typeof<KeyValuePair<_,_>>.GetGenericTypeDefinition() then
        let kty = elemType.GenericTypeArguments.[0]
        let vty = elemType.GenericTypeArguments.[1]

        let get_Key = KeyValuePairType.get_Key elemType
        let get_Value = KeyValuePairType.get_Value elemType

        fun (loop:Loop) (value:obj) ->
        value
        |> IEnumerableType.toList
        |> List.map(fun kvp -> 
            Json.Array [
                (loop:Loop) kty (get_Key kvp)
                (loop:Loop) vty (get_Value kvp)
                ])
        |> Json.Array
    else
        fun (loop:Loop) (value:obj) ->
        value
        |> IEnumerableType.toList
        |> List.map(loop elemType)
        |> Json.Array
    |> Some

let tryTuple = fun (ty:Type) ->
    if FSharpType.IsTuple ty then
        let elementTypes = FSharpType.GetTupleElements ty
        let reader = FSharpValue.PreComputeTupleReader ty

        Some(fun (loop:Loop) (value:obj) ->
        let fields = reader value

        fields
        |> Array.zip elementTypes
        |> List.ofArray
        |> List.map(fun(ftype,field)-> (loop:Loop) ftype field)
        |> Json.Array
        )
    else None

let tryUnion = fun (ty:Type) ->
    if FSharpType.IsUnion ty then
        let reader = UnionType.readUnion ty
        let qa = UnionType.getQualifiedAccess ty
        Some(fun (loop:Loop) (value:obj) ->
        let name,fields = reader value
        let tag = Json.String(qa + name)
        match
            fields
            |> Array.map(fun (ftype,field) -> loop ftype field)
            |> Array.toList
        with
        | [] -> tag
        | unionFields -> Json.Array (tag :: unionFields)
        )
    else None

let tryRecord = fun (ty:Type) ->
    if FSharpType.IsRecord ty then
        let pis = FSharpType.GetRecordFields ty
        let reader = FSharpValue.PreComputeRecordReader ty
        Some(fun (loop:Loop) (value:obj) ->
        reader value
        |> Array.zip pis
        |> Array.map(fun(pi,value) -> pi.Name, (loop:Loop) pi.PropertyType value)
        |> List.ofArray
        |> Json.Object
        )
    else None
