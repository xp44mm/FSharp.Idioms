module FSharp.Idioms.DefaultValues.MapDefaultValue

open System
open FSharp.Idioms
let getDefault(ty:Type) =
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>> then
        Some(fun (loop: Type -> obj) ->
            let emptyGeneric = MapType.mapModuleType.GetMethod "Empty"
            let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
            empty.Invoke(null, Array.empty) |> box
        )
    else None

//type MapDefaultValueProvider() =
//    static member Singleton = MapDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>>
//        member this.defaultValue(loop,ty) =
//            let emptyGeneric = MapType.mapModuleType.GetMethod "Empty"
//            let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
//            empty.Invoke(null, Array.empty) |> box

