module FSharp.Idioms.DefaultValues.GuidDefaultValue

open System
open System.Reflection

let getDefault(ty:Type) =
    if ty = typeof<Guid> then
        Some(fun (loop: Type -> obj) ->
            box(Guid())
        )
    else None

//type GuidDefaultValueProvider() =
//    static member Singleton = GuidDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty = typeof<Guid>
//        member this.defaultValue(loop,ty) = new Guid() |> box

