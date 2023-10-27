module FSharp.Idioms.DefaultValues.OptionDefaultValue

open System
open Microsoft.FSharp.Reflection
let getDefault(ty:Type) =
    if FSharpType.IsUnion ty && ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Option<_>> then
        Some(fun (loop: Type -> obj) ->
            null
        )
    else None

//type OptionDefaultValueProvider() =
//    static member Singleton = OptionDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = FSharpType.IsUnion ty && ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Option<_>>
//        member this.defaultValue(loop,ty) = null

