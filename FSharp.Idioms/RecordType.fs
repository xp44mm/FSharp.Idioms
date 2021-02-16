module FSharp.Idioms.RecordType

open System
open Microsoft.FSharp.Reflection
open System.Collections.Concurrent
open System.Reflection

let recordFields = ConcurrentDictionary<Type, PropertyInfo[]>(HashIdentity.Structural)

let getRecordFields (recordType:Type) = 
    recordFields.GetOrAdd(recordType, FSharpType.GetRecordFields)

let reads = ConcurrentDictionary<Type, obj -> (PropertyInfo * obj)[]>(HashIdentity.Structural)

let readRecord (recordType:Type) =
    let valueFactory (ty:Type) =
        let fields = getRecordFields ty
        let getValues = FSharpValue.PreComputeRecordReader ty
        fun (value:obj) -> value |> getValues |> Array.zip fields
    reads.GetOrAdd(recordType,Func<_,_>valueFactory)

