module FSharp.Idioms.Reflection.UnionType
open FSharp.Reflection

open System
open System.Collections.Concurrent
open System.Reflection

let memoUnionCases = ConcurrentDictionary<Type, UnionCaseInfo[]>(HashIdentity.Structural)

let getUnionCases (unionType:Type) =
    let valueFacotry (unionType:Type) = FSharpType.GetUnionCases(unionType)
    memoUnionCases.GetOrAdd(unionType, valueFacotry)

let memoCaseFields = ConcurrentDictionary<UnionCaseInfo, PropertyInfo[]>(HashIdentity.Structural)
let getCaseFields(unionCase:UnionCaseInfo) =
    let valueFacotry (unionCase:UnionCaseInfo) = unionCase.GetFields()
    memoCaseFields.GetOrAdd(unionCase, valueFacotry)

let memoReadUnion = ConcurrentDictionary<Type, obj -> string * (Type*obj)[]>(HashIdentity.Structural)
///可区分联合分解一次
let readUnion (unionType:Type) =
    let valueFacotry (unionType:Type) =
        let unionCases = getUnionCases unionType
        let tagReader = FSharpValue.PreComputeUnionTagReader unionType

        let unionCases =
            unionCases
            |> Array.map(fun uc ->
                {|
                    name =  uc.Name
                    fieldTypes = 
                        uc 
                        |> getCaseFields 
                        |> Array.map(fun pi -> pi.PropertyType)
                    reader = FSharpValue.PreComputeUnionReader uc
                |})
        fun (value:obj) ->
            let tag = tagReader value
            let unionCase = unionCases.[tag]

            let types = unionCase.fieldTypes
            let values = unionCase.reader value
            let fields = Array.zip types values
            unionCase.name, fields

    memoReadUnion.GetOrAdd(unionType,Func<_,_> valueFacotry)

let memoQualifiedAccess = ConcurrentDictionary<Type, string>(HashIdentity.Structural)
let getQualifiedAccess(unionType:Type) =
    let valueFacotry (unionType:Type) = 
        if unionType.IsDefined(typeof<RequireQualifiedAccessAttribute>,true) then
            unionType.Name + "."
        else ""

    memoQualifiedAccess.GetOrAdd(unionType, valueFacotry)

let getQualifiedNameIfNeed =
    let memo = ConcurrentDictionary<Type, string -> string >(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        fun (name:string) ->
            if ty.IsDefined(typeof<RequireQualifiedAccessAttribute>,false) then
                $"{ty.Name}.{name}"
            else name
    )
