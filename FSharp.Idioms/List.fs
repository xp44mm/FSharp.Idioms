[<RequireQualifiedAccess>]
module FSharp.Idioms.List

/// 将键值对列表转化为Map，键可能重复的，相同键的值收集到列表中。
let toMap (pairs:('k*'v)list) =
    pairs
    |> List.groupBy fst
    |> List.map(fun(k,pairs)->
        let values =
            pairs |> List.map snd
        k,values
    )
    |> Map.ofList

/// 将键值对列表转化为二级JaggedMap，键可能重复的，相同键的值收集到列表中。
let toJaggedMap (triples:('x*'y*'z) list) =
    triples
    |> List.groupBy Triple.first
    |> List.map(fun(x,triples)->
        let mp =
            triples
            |> List.map Triple.lastTwo
            |> toMap
        x,mp)
    |> Map.ofList

/// 将键值对列表转化为二级JaggedMap，相同键的值后来者赢。
let toUniqueJaggedMap (triples:('x*'y*'z)list) =
    triples
    |> List.groupBy Triple.first
    |> List.map(fun(x,triples)->
        let mp =
            triples
            |> List.map Triple.lastTwo
            |> Map.ofList
        x,mp)
    |> Map.ofList

/// 从二级JaggedMap构造列表，生成三元组的列表
let ofJaggedMap (mp:Map<'u,Map<'v,'w>>) =
    mp
    |> Map.toList
    |> List.map(fun(u, v) ->
        v
        |> Map.toList
        |> List.map(fun(v,w)->u,v,w)
    )
    |> List.concat

/// 取列表前面所有断言为真的项，加上第一个断言为假的项（如果有）
let takeUntilNot (predicate:'t->bool) (ls:'t list) =
    let rec loop (acc:'t list) (ls:'t list) =
        match ls with
        | [] -> acc
        | h::t ->
            let acc = h :: acc
            if predicate h then
                loop acc t
            else
                acc
    loop [] ls |> List.rev

///// n 个元素取出2元素，组合
//let combination2 (ls:'a list) =
//    let a = ls.[..ls.Length-2]
//    let b = ls.[1..]
//    List.zip a b
