module FSharp.Idioms.DefaultValues.ListDefaultValue

open System
open FSharp.Idioms
let getDefault(ty:Type) =
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>> then
        Some(fun (loop: Type -> obj) ->
            let listModuleType = ListType.listModuleType
            let emptyGeneric = listModuleType.GetMethod "Empty"
            let listempty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
            listempty.Invoke(null, Array.empty) |> box
        )
    else None

//type ListDefaultValueProvider() =
//    static member Singleton = ListDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>>
//        member this.defaultValue(loop,ty) =
//            let listModuleType = ListType.listModuleType
//            let emptyGeneric = listModuleType.GetMethod "Empty"
//            let listempty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
//            listempty.Invoke(null, Array.empty) |> box

