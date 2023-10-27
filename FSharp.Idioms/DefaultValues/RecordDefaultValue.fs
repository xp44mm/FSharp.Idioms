module FSharp.Idioms.DefaultValues.RecordDefaultValue

open System
open Microsoft.FSharp.Reflection

let getDefault (ty: Type) =
    if FSharpType.IsRecord ty then
        Some
            (fun (loop: Type -> obj) ->
                let fields = FSharpType.GetRecordFields(ty)
                let values =
                    fields
                    |> Array.map (fun pi -> loop pi.PropertyType)
                FSharpValue.MakeRecord(ty, values))
    else
        None

//type RecordDefaultValueProvider() =
//    static member Singleton = RecordDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = FSharpType.IsRecord ty
//        member this.defaultValue(loop,ty) =
//            let fields = FSharpType.GetRecordFields(ty)
//            let values =
//                fields
//                |> Array.map(fun pi -> loop pi.PropertyType)
//            FSharpValue.MakeRecord(ty,values)
