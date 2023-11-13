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

let getIndex e st =
    if Set.contains e st then
        st
        |> Seq.findIndex(fun x -> x = e)
    else -1

let allPairs (st1:Set<'T1>) (st2:Set<'T2>) =
    let ls1 = st1 |> Set.toList
    let ls2 = st2 |> Set.toList

    List.allPairs ls1 ls2
    |> Set.ofList

///集合中任意两个元素的组合
let combine2 st =
    st
    |> Set.map(fun x ->
        st
        |> Set.filter(fun y -> y > x)
        |> Set.map(fun y -> 
            x,y
        )
    )
    |> Set.unionMany

/// 叉幂
let crosspower (n:int) (st:Set<'a>) =
    st
    |> Set.toList
    |> List.crosspower n
    |> Set.ofList

///返回符号的深度优先顺序列表。
let depthFirstSort (nodes:Map<'t,Set<'t>>) (start:'t) =
    let rec loop (discovered:list<'t>) (unfinished:list<'t>) =
        //Console.WriteLine(stringify (discovered,unfinished))
        match unfinished with
        | [] -> discovered |> List.rev
        | current::tail ->
            match
                if nodes.ContainsKey current then
                    nodes.[current]
                    |> Seq.tryFind(fun x ->
                        discovered
                        |> Set.ofList
                        |> Set.contains x
                        |> not
                    )
                else None
            with
            | Some next ->
                //发现next立即加入discovered
                //next加入unfinished继续查找自己的下一个
                loop (next::discovered) (next::unfinished)
            | None ->
                //currnet丢弃即可。在next时已经加入discovered，切勿重复加入。
                loop discovered tail

    loop [start] [start]

