[<RequireQualifiedAccess>]
module FSharp.Idioms.Map

open System.Collections.Generic

/// 收集可能重复的键，值变成值的序列
let fromDuplicateKeys (pairs:#seq<'k*'v>) =
    pairs
    |> Seq.groupBy fst
    |> Seq.map(fun(k,pairs)->
        let values =
            pairs |> Seq.map snd
        k,values
    )
    |> Map.ofSeq

let mapKey (getKey:'k->'v->'kk) (mp:Map<'k,'v>) =
    mp
    |> Map.toSeq
    |> Seq.map(fun(k,v)->
        let kk = getKey k v
        kk,v)
    |> fromDuplicateKeys

let mapEntry (getEntry:'k->'v->'kk*'vv) (mp:Map<'k,'v>) =
    mp
    |> Map.toSeq
    |> Seq.map(fun(k,v)-> getEntry k v)
    |> fromDuplicateKeys

let concat (mps:#seq<Map<'k,'v>>) =
    mps
    |> Seq.collect Map.toSeq
    |> fromDuplicateKeys

let append (mp1:Map<'k,'v>) (mp2:Map<'k,'v>) =
    concat [mp1;mp2]

let keys (mp:Map<'k,'v>) =
    let i = mp :> IDictionary<'k,'v>
    i.Keys :> seq<'k>