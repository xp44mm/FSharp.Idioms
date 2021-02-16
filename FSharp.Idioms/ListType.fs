module FSharp.Idioms.ListType

open System
open System.Collections.Concurrent
open System.Reflection

let listModuleType = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.ListModule")
let methodOfArrayDef = listModuleType.GetMethod "OfArray"

let memoElementType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
let getElementType (listType:Type) =
    let valueFactory (ty:Type) = ty.GenericTypeArguments.[0]
    memoElementType.GetOrAdd(listType, valueFactory)

let memoIsEmptyProperty = ConcurrentDictionary<Type, PropertyInfo>(HashIdentity.Structural)
let getIsEmpty (listType:Type) =   
    let valueFactory (ty:Type) = ty.GetProperty("IsEmpty")
    let prop = memoIsEmptyProperty.GetOrAdd(listType, valueFactory)
    fun (ls:obj) -> prop.GetValue(ls) :?> bool

let memoHeadProperty = ConcurrentDictionary<Type, PropertyInfo>(HashIdentity.Structural)
let getHead (listType:Type) =   
    let valueFactory (ty:Type) = ty.GetProperty("Head")
    let prop = memoHeadProperty.GetOrAdd(listType, valueFactory)
    fun (ls:obj) -> prop.GetValue(ls)

let memoTailProperty = ConcurrentDictionary<Type, PropertyInfo>(HashIdentity.Structural)
let getTail (listType:Type) = 
    let valueFactory (ty:Type) = ty.GetProperty("Tail")
    let prop = memoTailProperty.GetOrAdd(listType, valueFactory)
    fun (ls:obj) -> prop.GetValue(ls)

let memoReadList = ConcurrentDictionary<Type, obj -> Type*obj[]>(HashIdentity.Structural)

///列表分解一次为元素
let readList (listType:Type) =

    let factory (ty:Type) =
        let pIsEmpty = getIsEmpty ty
        let pHead = getHead ty
        let pTail = getTail ty

        let rec objToRevList acc ls =
            if pIsEmpty ls then
                acc
            else
                let elem = pHead(ls)
                let tail = pTail(ls)
                let acc = elem :: acc
                objToRevList acc tail

        fun (ls:obj) ->
            let values =
                ls
                |> objToRevList []
                |> List.rev
                |> List.toArray
            getElementType ty, values
    memoReadList.GetOrAdd(listType,Func<_,_> factory)

let memoOfArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

let getOfArray (listType:Type) =
    let valueFactory (ty:Type) =
        let elementType = getElementType ty
        methodOfArrayDef.MakeGenericMethod(elementType)
    memoOfArray.GetOrAdd(listType, valueFactory)
