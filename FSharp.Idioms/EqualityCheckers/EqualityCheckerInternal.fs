//[<System.ObsoleteAttribute("旨在演示反射代码，不要调用")>]
module FSharp.Idioms.EqualityCheckers.EqualityCheckerInternal

open System
open System.Collections

//let IReadOnlySetEqualityChecker (ty:Type) =
//    let iname = "IReadOnlySet`1"
//    let ity = ty.GetInterface(iname)
//    {
//    check = ity <> null
//    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
//        let setEquals = ity.GetMethod("SetEquals")
//        setEquals.Invoke(x, [|y|])
//        |> unbox<bool>
//    }

//let ISetEqualityChecker (ty:Type) =
//    let iname = "ISet`1"
//    let ity = ty.GetInterface(iname)
//    {
//    check = ity <> null
//    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
//        let setEquals = ity.GetMethod("SetEquals")
//        setEquals.Invoke(x, [|y|])
//        |> unbox<bool>
//    }


let SeqEqualityChecker (ty:Type) =
    let iname = "IEnumerable"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let getEnum = ity.GetMethod("GetEnumerator")
        let ienum = getEnum.ReturnType
        let moveNext = ienum.GetMethod("MoveNext")
        let current = ienum.GetProperty("Current")
        let loopElement = loop ty.GenericTypeArguments.[0]
        let e1 = getEnum.Invoke(x,[||])
        let e2 = getEnum.Invoke(y,[||])
        let rec loopNext i =
            let hasNext1 = moveNext.Invoke(e1,[||]) |> unbox<bool>
            let hasNext2 = moveNext.Invoke(e2,[||]) |> unbox<bool>
            if hasNext1 && hasNext2 then
                let c1 = current.GetValue(e1)
                let c2 = current.GetValue(e2)
                if loopElement c1 c2 && i < 999 then
                    loopNext(i+1)
                else false
            else hasNext1 = hasNext2
        loopNext 0
    }


let IEquatableEqualityChecker (ty:Type) =
    let iname = "IEquatable`1"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let mi = ity.GetMethod("Equals")
        mi.Invoke(x, [|y|])
        |> unbox<bool>
    }

let IComparableEqualityChecker (ty:Type) =
    let iname = "IComparable"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let mi = ity.GetMethod("CompareTo")
        mi.Invoke(x,[|y|])
        |> unbox<int>
        |> (=) 0
    }

let IStructuralEquatableEqualityChecker (ty:Type) =
    let iname = "IStructuralEquatable"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let mi = ity.GetMethod("Equals")
        mi.Invoke(x, [|y;StructuralComparisons.StructuralEqualityComparer|])
        |> unbox<bool>
    }

let IStructuralComparableEqualityChecker (ty:Type) =
    let iname = "IStructuralComparable"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let mi = ity.GetMethod("CompareTo")
        mi.Invoke(x,[|y;StructuralComparisons.StructuralComparer|])
        |> unbox<int>
        |> (=) 0
    }




