module FSharp.Idioms.DefaultValues.TimeSpanDefaultValue

open System
open System.Reflection
let getDefault(ty:Type) =
    if ty = typeof<TimeSpan> then
        Some(fun (loop: Type -> obj) ->
            box TimeSpan.Zero
        )
    else None

//type TimeSpanDefaultValueProvider() =
//    static member Singleton = TimeSpanDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty = typeof<TimeSpan>
//        member this.defaultValue(loop,ty) = box <| TimeSpan.Zero

