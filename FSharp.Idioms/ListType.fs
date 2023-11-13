module FSharp.Idioms.ListType

open System
open System.Collections.Concurrent
open System.Reflection

let listModuleType = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.ListModule")

let Method_OfArray = listModuleType.GetMethod("OfArray")
let ofArray =
    let memo = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
    fun (ty:Type) ->
        memo.GetOrAdd(ty.GenericTypeArguments.[0],
            Method_OfArray.MakeGenericMethod(ty.GenericTypeArguments)
            )

let listTypeDef = typeof<list<_>>.GetGenericTypeDefinition()

let get_IsEmpty =
    let memo = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(
        ty.GenericTypeArguments.[0], ty.GetMethod("get_IsEmpty")
    )

let isEmpty (ty:Type) =
    let mi = get_IsEmpty ty
    fun (value:obj) -> mi.Invoke(value,[||]) :?> bool

let get_Head =
    let memo = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(
        ty.GenericTypeArguments.[0], ty.GetMethod("get_Head")
    )

let head (ty:Type) =
    let mi = get_Head ty
    fun (value:obj) -> mi.Invoke(value,[||])

let get_Tail =
    let memo = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(
        ty.GenericTypeArguments.[0], ty.GetMethod("get_Tail")
    )

let tail (ty:Type) =
    let mi = get_Tail ty
    fun (value:obj) -> mi.Invoke(value,[||])

///列表分解一次为元素
[<Obsolete("IEnumerableType.toArray")>]
let readList  =
    let memo = ConcurrentDictionary<Type, obj -> Type*obj[]>(HashIdentity.Structural)

    let factory (ty:Type) =
        let isEmpty = isEmpty ty
        let head = head ty
        let tail = tail ty

        let rec objToRevList acc ls =
            if isEmpty ls then
                acc
            else
                let elem = head(ls)
                let tail = tail(ls)
                let acc = elem :: acc
                objToRevList acc tail

        fun (ls:obj) ->
            let values =
                ls
                |> objToRevList []
                |> List.rev
                |> List.toArray
            ty.GenericTypeArguments.[0], values
    fun (listType:Type) -> memo.GetOrAdd(
        listType.GenericTypeArguments.[0],factory listType)

//let toArray (ls:obj) = IEnumerableType.toArray ls
