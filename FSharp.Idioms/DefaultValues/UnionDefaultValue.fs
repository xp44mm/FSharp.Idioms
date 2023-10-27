module FSharp.Idioms.DefaultValues.UnionDefaultValue

open System
open FSharp.Idioms
open Microsoft.FSharp.Reflection
let getDefault(ty:Type) =
    if FSharpType.IsUnion ty then
        Some(fun (loop: Type -> obj) ->
            let unionCaseInfo =
                FSharpType.GetUnionCases(ty)
                |> Array.minBy(fun i -> i.Tag)

            let uionFieldTypes =
                unionCaseInfo.GetFields()
                |> Array.map(fun info -> info.PropertyType)

            let fields =
                uionFieldTypes
                |> Array.map loop

            FSharpValue.MakeUnion(unionCaseInfo, fields)
        )
    else None

//type UnionDefaultValueProvider() =
//    static member Singleton = UnionDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = FSharpType.IsUnion ty
//        member this.defaultValue(loop,ty) =
//            let unionCaseInfo =
//                FSharpType.GetUnionCases(ty)
//                |> Array.minBy(fun i -> i.Tag)

//            let uionFieldTypes =
//                unionCaseInfo.GetFields()
//                |> Array.map(fun info -> info.PropertyType)

//            let fields =
//                uionFieldTypes
//                |> Array.map loop

//            FSharpValue.MakeUnion(unionCaseInfo, fields)




