module FSharp.Idioms.DefaultValues.DateTimeOffsetDefaultValue

open System
open System.Reflection

let getDefault(ty:Type) =
    if ty = typeof<DateTimeOffset> then
        Some(fun (loop: Type -> obj) ->
            let now = DateTimeOffset.Now
            box <| DateTimeOffset (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Offset)
        )
    else None


//type DateTimeOffsetDefaultValueProvider() =
//    static member Singleton = DateTimeOffsetDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty = typeof<DateTimeOffset>
//        member this.defaultValue(loop,ty) = 
//            let now = DateTimeOffset.Now
//            box <| DateTimeOffset (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Offset)

