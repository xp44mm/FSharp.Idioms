[<RequireQualifiedAccess>]
module FSharp.Idioms.Graph

///输入任意的集合列表sets，
///如果集合列表的任意二个集合有相同的元素，就将其合并。
///最后返回的集合列表，没有任意的两个集合是有共同元素的。
let disjoint (sets:Set<Set<'a>>) =
    //合并与st有交集的集合，放在列表的开始
    let loop acc st =
        let nonintersect = acc |> Set.filter(Set.intersect st >> Set.isEmpty)
        (acc - nonintersect)
        |> Set.unionMany
        |> Set.add <| nonintersect

    //依次对每个集合都执行一次合并
    //前一个SETS作为迭代的累加器，后一个SETS用于遍历每一个元素
    Set.fold loop sets sets

///根据pairs提供的推理关系，从右侧开始计算，不断用右側的子集（已知）填充左側的超集(FST)
let rec propagate<'a when 'a:comparison> (map:Map<'a,Set<'a>>) (pairs:Set<'a*'a>) = //spread
    let unknown = pairs |> Set.map fst
    let known = pairs |> Set.map snd

    ///互相包含的集合是等价的
    if unknown = known then
        //环：一组等价的集合构成环
        //如：A->B,B->C, ... C->A
        let noninersections =
            pairs
            |> Set.map(fun(a,b)->set [a;b])
            |> disjoint

        // A = B = ... C = (A U B U ... C)
        //元素，元素所在集合
        let acc =
            noninersections
            |> Seq.collect(fun st ->
                let result =
                    st
                    |> Set.filter map.ContainsKey
                    |> Set.map(fun k -> map.[k])
                    |> Set.unionMany
                st
                |> Set.map(fun e -> e, result)
                //|> Set.toList
            )
            |> Seq.toList

        //Maps.mergeMap map acc
        map
        |> Map.toSeq
        |> Seq.append acc
        |> Set.unionByKey
        |> Map.ofSeq

    else

        // 仅在子集字段上出现的符号，表明X不能再被其他符号填充。map中的X已经是完整的集合了。
        let processings =
            pairs
            |> Set.filter(fun (l,r) -> map.ContainsKey r && not(unknown.Contains r))

        let remains = pairs - processings

        //防止死循环
        if processings.Count = 0 then failwithf "propagate:(map:%A,remains:%A)" map remains

        //根据表达式A -> B，用A填充B
        let newMap =
            processings
            |> Set.map(fun(l,r)-> l, map.[r])
            |> Seq.append <| Map.toSeq map
            |> Set.unionByKey
            |> Map.ofSeq

        if remains.IsEmpty then
            newMap
        else
            propagate newMap remains

