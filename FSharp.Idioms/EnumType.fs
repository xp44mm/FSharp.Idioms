module FSharp.Idioms.EnumType

open System
open System.Reflection
open System.Collections.Concurrent

/// enum to uint64
let toUInt64 (enumType:Type) =
    if enumType = typeof<sbyte> then
        unbox<sbyte> >> uint64
    elif enumType = typeof<byte> then
        unbox<byte> >> uint64
    elif enumType = typeof<int16> then
        unbox<int16> >> uint64
    elif enumType = typeof<uint16> then
        unbox<uint16> >> uint64
    elif enumType = typeof<int32> then
        unbox<int32> >> uint64
    elif enumType = typeof<uint32> then
        unbox<uint16> >> uint64
    elif enumType = typeof<int64> then
        unbox<int64> >> uint64
    elif enumType = typeof<uint64> then
        unbox<uint64>
    //elif ty = typeof<nativeint> then
    //    unbox<nativeint> >> uint64
    //elif ty = typeof<unativeint> then
    //    unbox<unativeint> >> uint64
    else
        failwith "Unknown Enum Underlying Type."

/// enum from uint64
let fromUInt64 (enumType:Type) (src:uint64) =
    src |>
    if enumType = typeof<sbyte> then
        sbyte >> box
    elif enumType = typeof<byte> then
        byte >> box
    elif enumType = typeof<int16> then
        int16 >> box
    elif enumType = typeof<uint16> then
        uint16 >> box
    elif enumType = typeof<int32> then
        int32 >> box
    elif enumType = typeof<uint32> then
        uint16 >> box
    elif enumType = typeof<int64> then
        int64 >> box
    elif enumType = typeof<uint64> then
        box
    //elif ty = typeof<nativeint> then
    //    nativeint >> box
    //elif ty = typeof<unativeint> then
    //    unativeint >> box
    else
        failwith "Unknown Enum Underlying Type."

let memoUnderlyingTypes = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
let getEnumUnderlyingType (enumType:Type) =
    let valueFactory (enumType:Type) = enumType.GetEnumUnderlyingType()
    memoUnderlyingTypes.GetOrAdd(enumType,valueFactory)

let memoNameValuePairs  = ConcurrentDictionary<Type, (string*uint64)[]>(HashIdentity.Structural)
let getNameValuePairs (enumType:Type) =
    let valueFactory (enumType:Type) =
        let underlyingType = getEnumUnderlyingType enumType
        Enum.GetNames(enumType)
        |> Array.map(fun name ->
            let value =
                enumType.GetField(
                    name, BindingFlags.Public ||| BindingFlags.Static
                    ).GetValue(null)
                |> toUInt64 underlyingType
            name, value
        )
    memoNameValuePairs.GetOrAdd(enumType,valueFactory)

let memoValues = ConcurrentDictionary<Type, Map<string,uint64>>(HashIdentity.Structural)
let getValues(enumType:Type) =
    let valueFactory (enumType:Type) =
        getNameValuePairs enumType
        |> Map.ofArray
    memoValues.GetOrAdd(enumType,valueFactory)

let memoReadFlags = ConcurrentDictionary<Type, obj -> string[]>(HashIdentity.Structural)

///[<Flags>] Enum 的值
let readFlags (enumType:Type) =
    let valueFactory (ty:Type) =
        let enumUnderlyingType = getEnumUnderlyingType ty
        let zeroPairs,positivePairs =
            getNameValuePairs ty
            |> Array.partition(fun (name,value) -> value = 0UL)

        let zeroNames = zeroPairs |> Array.map fst

        fun (enm:obj) ->
            let inpValue = toUInt64 enumUnderlyingType enm
            let flagNames =
                positivePairs
                |> Array.filter(fun(flag,flagValue) -> inpValue &&& flagValue = flagValue)
                |> Array.map fst

            if Array.isEmpty flagNames then zeroNames else flagNames
    memoReadFlags.GetOrAdd(enumType, Func<_,_> valueFactory)

