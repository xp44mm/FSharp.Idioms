module FSharp.Idioms.DefaultValues.ArrayDefaultValue

open System

let getDefault(ty:Type) =
    if ty.IsArray then
        Some(fun (loop: Type -> obj) ->
            let elementType = ty.GetElementType()
            Array.CreateInstance (elementType, 0) |> box
        )
    else None

//type ArrayDefaultValueProvider() =
//    static member Singleton = ArrayDefaultValueProvider() :> DefaultValueProvider
//    interface DefaultValueProvider with
//        member this.filter ty = ty.IsArray
//        member this.defaultValue(loop,ty) =
//            let elementType = ty.GetElementType()
//            Array.CreateInstance (elementType, 0) |> box

//动态创建泛型数组：
//需要使用`Array.CreateInstance(elementType,length)`方式创建一个新的数组。elementType为数组元素的类型，length为数组的长度。
//然后使用Array.SetValue(object,index)方法将元素插入
