module FSharp.Idioms.DefaultValues.NullableDefaultValue

open System
let getDefault(ty:Type) =
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
        Some(fun (loop: Type -> obj) ->
            null
        )
    else None

//type NullableDefaultValueProvider() =
//    static member Singleton = NullableDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
//        member this.defaultValue(loop,ty) = null
