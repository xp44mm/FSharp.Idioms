[<RequireQualifiedAccess>]
module FSharp.Idioms.Array

let tap f (source:'a[]) =
    source
    |> Array.map(fun x -> 
        f x
        x
    )

let tapi f (source:'a[]) =
    source
    |> Array.mapi(fun i x -> 
        f i x
        x
    )

let toRevList (source:'a []) = 
    source
    |> List.ofRevArray
        

/// 将键值对数组转化为Map，键可能重复的，相同键的值收集到数组中。
let toMap (pairs:('k*'v)[]) =
    pairs
    |> Array.groupBy fst
    |> Array.map(fun(k,pairs)->
        let values =
            pairs |> Array.map snd
        k,values
    )
    |> Map.ofArray

/// 将键值对数组转化为二级JaggedMap，键可能重复的，相同键的值收集到数组中。
let toJaggedMap (triples:('x*'y*'z)[]) =
    triples
    |> Array.groupBy Triple.first
    |> Array.map(fun(x,triples)->
        let mp =
            triples
            |> Array.map Triple.lastTwo
            |> toMap
        x,mp)
    |> Map.ofArray

/// 将键值对数组转化为二级JaggedMap，相同键的值后来者赢。
let toUniqueJaggedMap (triples:('x*'y*'z)[]) =
    triples
    |> Array.groupBy Triple.first
    |> Array.map(fun(x,triples)->
        let mp =
            triples
            |> Array.map Triple.lastTwo
            |> Map.ofArray
        x,mp)
    |> Map.ofArray


/// 从二级JaggedMap构造数组，生成三元组的数组
let ofJaggedMap (mp:Map<'u,Map<'v,'w>>) =
    mp
    |> Map.toArray
    |> Array.map(fun(u, v) ->
        v
        |> Map.toArray
        |> Array.map(fun(v,w)->u,v,w)
    )
    |> Array.concat