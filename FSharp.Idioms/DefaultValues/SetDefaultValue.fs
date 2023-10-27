module FSharp.Idioms.DefaultValues.SetDefaultValue

open System
open FSharp.Idioms

let getDefault(ty:Type) =
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>> then
        Some(fun (loop: Type -> obj) ->
            let emptyGeneric = SetType.setModuleType.GetMethod "Empty"
            let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
            empty.Invoke(null, Array.empty) |> box
        )
    else None

//type SetDefaultValueProvider() =
//    static member Singleton = SetDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
//        member this.defaultValue(loop,ty) =
//            let emptyGeneric = SetType.setModuleType.GetMethod "Empty"
//            let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
//            empty.Invoke(null, Array.empty) |> box

