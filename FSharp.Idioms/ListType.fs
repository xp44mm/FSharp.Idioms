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
    let memo = ConcurrentDictionary<Type, obj->bool>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0], 
        let mi = ty.GetMethod("get_IsEmpty")
        fun (value:obj) -> mi.Invoke(value,[||]) :?> bool
    )

// []
let empty =
    let memo = ConcurrentDictionary<Type, obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0],
        let get_Empty = ty.GetMethod("get_Empty")
        get_Empty.Invoke(null,[||])
    )

let get_Head =
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0], 
        let mi = ty.GetMethod("get_Head")
        fun (value:obj) -> mi.Invoke(value,[||])
    )

let get_Tail =
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0], 
        let mi = ty.GetMethod("get_Tail")
        fun (value:obj) -> mi.Invoke(value,[||])
    )
// ctor
let Cons =
    let memo = ConcurrentDictionary<Type, obj*obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0], 
        let m = ty.GetMethod("Cons")
        fun (e:obj,ls:obj) -> m.Invoke(null,[|e; ls|])
    )

///列表分解一次为元素
[<Obsolete("IEnumerableType.toArray")>]
let readList  =
    let memo = ConcurrentDictionary<Type, obj->obj[]>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0],
        let isEmpty = get_IsEmpty ty
        let head = get_Head ty
        let tail = get_Tail ty

        let rec objToRevList acc ls =
            if isEmpty ls then
                acc
            else
                let elem = head(ls)
                let tail = tail(ls)
                let acc = elem :: acc
                objToRevList acc tail

        fun (ls:obj) ->
            ls
            |> objToRevList []
            |> List.rev
            |> List.toArray
    )

let writelist (ty:Type) (revseq:seq<obj>) =
    let cons = Cons ty
    let empty = empty ty

    revseq
    |> Seq.fold(fun ls e -> cons(e,ls)) empty
