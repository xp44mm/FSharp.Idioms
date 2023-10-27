module FSharp.Idioms.DefaultValues.DBNullDefaultValue

open System

let getDefault(ty:Type) =
    if ty = typeof<DBNull> then
        Some(fun (loop: Type -> obj) ->
            box DBNull.Value
        )
    else None


//type DBNullDefaultValueProvider() =
//    static member Singleton = DBNullDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty = typeof<DBNull>
//        member this.defaultValue(loop,ty) = box DBNull.Value
