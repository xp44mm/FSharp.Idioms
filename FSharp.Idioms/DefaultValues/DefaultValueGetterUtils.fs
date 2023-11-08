module FSharp.Idioms.DefaultValues.DefaultValueGetterUtils

open System
open System.Reflection
open FSharp.Idioms.DefaultValues.DefaultValueGetters

let DefaultValueGetters = [
    fun (ty:Type) -> {
    finder=ty=typeof<bool>
    getDefault=fun loop -> box false
    }
    fun (ty:Type) -> {
    finder=ty=typeof<sbyte>
    getDefault=fun loop -> box 0y
    }
    fun (ty:Type) -> {
    finder=ty=typeof<int16>
    getDefault=fun loop -> box 0s
    }
    fun (ty:Type) -> {
    finder=ty=typeof<int32>
    getDefault=fun loop -> box 0
    }
    fun (ty:Type) -> {
    finder=ty=typeof<int64>
    getDefault=fun loop -> box 0L
    }
    fun (ty:Type) -> {
    finder=ty=typeof<nativeint>
    getDefault=fun loop -> box 0n
    }
    fun (ty:Type) -> {
    finder=ty=typeof<byte>
    getDefault=fun loop -> box 0uy
    }
    fun (ty:Type) -> {
    finder=ty=typeof<uint16>
    getDefault=fun loop -> box 0us
    }
    fun (ty:Type) -> {
    finder=ty=typeof<char>
    getDefault=fun loop -> box '\u0000'
    }
    fun (ty:Type) -> {
    finder=ty=typeof<uint32>
    getDefault=fun loop -> box 0u
    }
    fun (ty:Type) -> {
    finder=ty=typeof<uint64>
    getDefault=fun loop -> box 0UL
    }
    fun (ty:Type) -> {
    finder=ty=typeof<unativeint>
    getDefault=fun loop -> box 0un
    }
    fun (ty:Type) -> {
    finder=ty=typeof<decimal>
    getDefault=fun loop -> box 0M
    }
    fun (ty:Type) -> {
    finder=ty=typeof<float>
    getDefault=fun loop -> box 0.0
    }
    fun (ty:Type) -> {
    finder=ty=typeof<float32>
    getDefault=fun loop -> box 0.0f
    }
    fun (ty:Type) -> {
    finder=ty=typeof<bigint>
    getDefault=fun loop -> box 0I
    }
    fun (ty:Type) -> {
    finder=ty=typeof<string>
    getDefault=fun loop -> box ""
    }
    fun (ty:Type) -> {
    finder = ty = typeof<Guid>
    getDefault = fun (loop: Type -> obj) ->
        Guid() |> box
    }
    fun (ty:Type) -> {
    finder = ty = typeof<DBNull>
    getDefault = fun (loop: Type -> obj) ->
        DBNull.Value
        |> box
    }
    fun (ty:Type) -> {
    finder = ty = typeof<DateTimeOffset>
    getDefault = fun (loop: Type -> obj) ->
        DateTimeOffset.MinValue |> box
    }
    fun (ty:Type) -> {
    finder = ty = typeof<TimeSpan>
    getDefault = fun (loop: Type -> obj) -> 
        TimeSpan.Zero |> box
    }
    fun (ty:Type) -> {
    finder = ty = typeof<Uri>
    getDefault = fun (loop: Type -> obj) -> 
        box (Uri("http://www.contoso.com/"))
    }
    EnumDefaultValueGetter
    NullableDefaultValueGetter
    OptionDefaultValueGetter
    ArrayDefaultValueGetter
    ListDefaultValueGetter
    SetDefaultValueGetter
    MapDefaultValueGetter
    TupleDefaultValueGetter
    RecordDefaultValueGetter
    UnionDefaultValueGetter
    fallbackDefaultValueGetter
    ]

/// 主函数
let rec getDefaultValue (getters: list<Type->DefaultValueGetter>) (ty: Type) =
    let action =
        getters
        |> Seq.tryPick (fun g -> 
            let getter = g ty
            if getter.finder then
                Some getter.getDefault
            else None
            )
        |> Option.defaultValue (fun (loop: Type -> obj) -> failwith "action")
    action (getDefaultValue getters)
