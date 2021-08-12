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
let rec propagate<'a when 'a:comparison> (result:Map<'a,Set<'a>>) (pairs:Set<'a*'a>) =
    let resultKeys = result |> Map.keys
    let leftElements = pairs |> Set.map fst
    let rightElements = pairs |> Set.map snd

    // 仅在子集字段上出现的符号X，表明X不能再被其他符号填充。map中的X已经是完整的集合了。
    let processings =
        let result_and_known = Set.intersect resultKeys rightElements
        let result_and_known_not_unkown = result_and_known - leftElements
        pairs
        |> Set.filter(snd >> result_and_known_not_unkown.Contains)

    //单向填充
    if processings.Count > 0 then
        let remains = pairs - processings

        //根据表达式A -> B，用A填充B
        let newMap =
            processings
            |> Set.map(fun(l,r)-> l, result.[r])
            |> Seq.append <| Map.toSeq result
            |> Set.unionByKey
            |> Map.ofSeq

        if remains.IsEmpty then
            newMap
        else
            propagate newMap remains

    else 
        ///环的元素必须左边，右边，两边都有存在
        let urings = 
            let st = Set.intersect leftElements rightElements
            pairs
            |> Set.filter(fun(l,r) -> st.Contains l && st.Contains r)

        let remains = pairs - urings

        ///互相包含的集合是等价的
        //环：一组等价的集合构成环
        //如：A->B,B->C, ... C->A
        let noninersections =
            urings
            |> Set.map(fun(a,b)->set [a;b])
            |> disjoint

        // A = B = ... C = (A U B U ... C)
        //元素，元素所在集合
        let acc =
            noninersections
            |> Seq.collect(fun st ->
                let result =
                    st
                    |> Set.filter result.ContainsKey
                    |> Set.map(fun k -> result.[k])
                    |> Set.unionMany
                st
                |> Set.map(fun e -> e, result)
            )
            |> Seq.toList

        let newResult =
            result
            |> Map.toSeq
            |> Seq.append acc
            |> Set.unionByKey
            |> Map.ofSeq

        if remains.IsEmpty then
            newResult
        else
            propagate newResult remains
