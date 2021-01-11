[<RequireQualifiedAccess>]
module FSharp.Idioms.Set

open System.Collections.Generic

let ofHashSet (hset:HashSet<'t>) =
    [
        for elem in hset -> elem
    ]|>Set.ofList

let toHashSet(set:Set<'t>) = new HashSet<_>(set)

let groupBy (projection) (st:Set<'t>) =
    st
    |> Seq.groupBy projection
    |> Seq.map(fun(k,sq)->
        k,Set.ofSeq sq
    )
    |> Set.ofSeq

/// 對相同鍵的值集合進行並集操作
let unionByKey (entries:#seq<'k*Set<'v>>) =
    entries
    |> Seq.groupBy fst
    |> Seq.map(fun(k,sq)->
        let vs =
            sq
            |> Seq.map snd
            |> Set.unionMany
        k, vs
    )
    |> Set.ofSeq

/// 将键值对数组转化为Map，键可能重复的，相同键的值收集到Set中。
let toMap (pairs:Set<'k*'v>) = 
    pairs
    |> Seq.groupBy fst
    |> Seq.map(fun(k,pairs)->
        let st =
            pairs 
            |> Seq.map snd 
            |> Set.ofSeq
        k,st
    )
    |> Map.ofSeq

// convert to (a,b,c) from `map of map`
let ofJaggedMap (mp:Map<'a,Map<'b,'c>>) =
    mp
    |> Map.map(fun s mp ->
        mp
        |> Map.toSeq
        |> Seq.map(fun(x,t)-> s,x,t)
    )
    |> Map.toSeq
    |> Seq.collect snd
    |> Set.ofSeq

/// 将键值对数组转化为二级JaggedMap，键可能重复的，相同键的值收集到Set中。
let toJaggedMap (triples:Set<'x*'y*'z>) =
    triples
    |> Seq.groupBy Triple.first
    |> Seq.map(fun(x,triples)->
        let mp =
            triples
            |> Seq.map Triple.lastTwo
            |> Set.ofSeq
            |> toMap
        x,mp)
    |> Map.ofSeq

/// 将键值对数组转化为二级JaggedMap，相同键的值后来者赢。
let toUniqueJaggedMap (triples:Set<'x*'y*'z>) =
    triples
    |> Seq.groupBy Triple.first
    |> Seq.map(fun(x,triples)->
        let mp =
            triples
            |> Seq.map Triple.lastTwo
            |> Map.ofSeq
        x,mp)
    |> Map.ofSeq

