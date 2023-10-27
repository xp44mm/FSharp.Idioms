module FSharp.Idioms.DefaultValues.TupleDefaultValue

open System
open Microsoft.FSharp.Reflection
let getDefault(ty:Type) =
    if FSharpType.IsTuple ty then
        Some(fun (loop: Type -> obj) ->
            let elementTypes = FSharpType.GetTupleElements(ty)
            let values =
                elementTypes
                |> Array.map(fun ety -> loop ety)
            FSharpValue.MakeTuple(values,ty)
        )
    else None

//type TupleDefaultValueProvider() =
//    static member Singleton = TupleDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = FSharpType.IsTuple ty
//        member this.defaultValue(loop,ty) =
//            let elementTypes = FSharpType.GetTupleElements(ty)
//            let values =
//                elementTypes
//                |> Array.map(fun ety -> loop ety)
//            FSharpValue.MakeTuple(values,ty)

