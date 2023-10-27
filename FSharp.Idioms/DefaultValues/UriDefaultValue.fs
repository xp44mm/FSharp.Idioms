module FSharp.Idioms.DefaultValues.UriDefaultValue

open System
open System.Reflection
let getDefault(ty:Type) =
    if ty = typeof<Uri> then
        Some(fun (loop: Type -> obj) ->
            box (Uri("http://www.contoso.com/"))
        )
    else None

//type UriDefaultValueProvider() =
//    static member Singleton = UriDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty = typeof<Uri>
//        member this.defaultValue(loop,ty) = new Uri("http://www.contoso.com/") |> box

