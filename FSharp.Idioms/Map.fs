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

/// 获取键的集合(Set)
let keys (mp:Map<'k,'v>) =
    let i = mp :> IDictionary<'k,'v>
    i.Keys |> Set.ofSeq

/// 从词典接口创建Map
let fromInterface (dict:IDictionary<'k,'v>) =
    dict
    |> Seq.map(fun kvp -> kvp.Key, kvp.Value)
    |> Map.ofSeq