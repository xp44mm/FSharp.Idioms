//[<System.ObsoleteAttribute("旨在演示反射代码，不要调用")>]
module FSharp.Idioms.EqualityCheckers.EqualityCheckerInternal

open System
open System.Collections

let SeqEqualityChecker (ty:Type) =
    let iname = "IEnumerable"
    let ity = ty.GetInterface(iname)
    {
    check = ity <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let mGetEnumerator = ity.GetMethod("GetEnumerator")
        let enumType = mGetEnumerator.ReturnType
        let mMoveNext = enumType.GetMethod("MoveNext")
        let pCurrent = enumType.GetProperty("Current")
        let loopElement = loop ty.GenericTypeArguments.[0]
        let e1 = mGetEnumerator.Invoke(x,[||])
        let e2 = mGetEnumerator.Invoke(y,[||])
        let rec loopNext i =
            let hasNext1 = mMoveNext.Invoke(e1,[||]) |> unbox<bool>
            let hasNext2 = mMoveNext.Invoke(e2,[||]) |> unbox<bool>
            if hasNext1 && hasNext2 then
                let c1 = pCurrent.GetValue(e1)
                let c2 = pCurrent.GetValue(e2)
                if loopElement c1 c2 && i < 999 then
                    loopNext(i+1)
                else false
            else hasNext1 = hasNext2
        loopNext 0
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




