[<RequireQualifiedAccess>]
module FSharp.Idioms.Map

open System.Collections.Generic

/// 收集可能重复的键，分组的值集合类型信息丢弃，返回seq类型。如输入(string*obj)[]类型，输出将是Map<string,seq<obj>>类型。
let fromDuplicateKeys (pairs:#seq<'k*'v>) =
    pairs
    |> Seq.groupBy fst
    |> Seq.map(fun(k,pairs)->
        let values =
            pairs |> Seq.map snd
        k,values
    )
    |> Map.ofSeq

/// 值不变，修改键。
/// 返回的键有重复，值将合并进seq<'v>中。合并后的值集将至少有一个元素。
let mapKey (getKey:'k->'v->'kk) (mp:Map<'k,'v>) =
    mp
    |> Map.toSeq
    |> Seq.map(fun(k,v)->
        let kk = getKey k v
        kk,v)
    |> fromDuplicateKeys

/// 整个词条包括键，包括值，都需要改变。
/// 返回的键有重复，值将合并进seq<'v>中。合并后的值集将至少有一个元素。
let mapEntry (getEntry:'k->'v->'kk*'vv) (mp:Map<'k,'v>) =
    mp
    |> Map.toSeq
    |> Seq.map(fun(k,v)-> getEntry k v)
    |> fromDuplicateKeys

/// 同Seq.concat
/// 返回的键有重复，值将合并进seq<'v>中。合并后的值集将至少有一个元素。
let concat (mps:#seq<Map<'k,'v>>) =
    mps
    |> Seq.collect Map.toSeq
    |> fromDuplicateKeys

let append (mp1:Map<'k,'v>) (mp2:Map<'k,'v>) =
    concat [mp1;mp2]

/// 从词典接口创建Map
let fromInterface (dict:IDictionary<'k,'v>) =
    dict
    |> Seq.map(fun kvp -> kvp.Key, kvp.Value)
    |> Map.ofSeq

let keyIsSubset (mp1:Map<'k,_>) (mp2:Map<'k,_>) =
    let k1 = mp1 |> Map.keys |> Set.ofSeq
    let k2 = mp2 |> Map.keys |> Set.ofSeq
    Set.isSubset k1 k2

let keyIsSuperset (mp1:Map<'k,_>) (mp2:Map<'k,_>) =
    let k1 = mp1 |> Map.keys |> Set.ofSeq
    let k2 = mp2 |> Map.keys |> Set.ofSeq
    Set.isSuperset k1 k2

let keyIsEqual (mp1:Map<'k,_>) (mp2:Map<'k,_>) =
    let k1 = mp1 |> Map.keys |> Set.ofSeq
    let k2 = mp2 |> Map.keys |> Set.ofSeq
    k1 = k2

let intersectByKey (mp1:Map<'k,_>) (mp2:Map<'k,_>) =
    let k1 = mp1 |> Map.keys |> Set.ofSeq
    let k2 = mp2 |> Map.keys |> Set.ofSeq
    let kk = Set.intersect k1 k2
    
    let ls1 = mp1 |> Map.filter(fun k v -> kk.Contains k) |> Map.toList
    let ls2 = mp2 |> Map.filter(fun k v -> kk.Contains k) |> Map.toList
    
    List.zip ls1 ls2
    |> List.map(fun ((k1,v1),(_,v2))-> k1,(v1,v2))
    
let differenceByKey (mp1:Map<'k,_>) (mp2:Map<'k,_>) =
    let k1 = mp1 |> Map.keys |> Set.ofSeq
    let k2 = mp2 |> Map.keys |> Set.ofSeq
    let kk = k1 |> Set.difference <| k2

    mp1 |> Map.filter(fun k v -> kk.Contains k)

/// 交换key,value即value-key
let inverse (mp:Map<'k,'v>) =
    mp
    |> Map.toSeq
    |> Seq.map Pair.swap
    |> fromDuplicateKeys
    |> Map.map(fun k v -> List.ofSeq v)

let getIndex k (mp:Map<'k,'v>) =
    if mp.ContainsKey k then
        mp
        |> Seq.findIndex(fun p -> p.Key = k)
    else -1
