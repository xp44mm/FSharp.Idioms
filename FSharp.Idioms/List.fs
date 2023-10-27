[<RequireQualifiedAccess>]
module FSharp.Idioms.List

let tap f (source:'a list) =
    source
    |> List.map(fun x -> 
        f x
        x
    )

let tapi f (source:'a list) =
    source
    |> List.mapi(fun i x -> 
        f i x
        x
    )

let ofRevArray (source:'a []) =
    let rec loop ls i =
        if i < source.Length then
            loop (source.[i]::ls) (i+1)
        else ls
    loop [] 0
        
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

let rec takeLast (ls) =
    match ls with
    | [] -> failwith $"{nameof List}:no found the last."
    | [_] -> ls
    | _ :: tail -> takeLast tail

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
[<System.Obsolete("=>advanceWhile")>]
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

/// 列表开始的元素满足条件，逆序放入第一个列表，第二个列表为剩余列表。
let advanceWhile (predicate:'t->bool) (ls:'t list) =
    let rec loop (target:'t list) (source:'t list) =
        match source with
        | [] -> target,[]
        | hd::tl ->
            if predicate hd then
                loop (hd :: target) tl
            else
                target,source
    loop [] ls

/// 列表开始的n个元素逆序放入第一个列表，第二个列表为剩余列表。
let advance n (ls:list<'t>) =
    let rec loop target (source:list<'t>) i =
        match i with
        | 0 -> target, source
        | _ ->
            match source with
            | [] -> failwith $"{nameof List}:n should < ls.length"
            | hd::tail-> loop (hd::target) tail (i-1)
    loop [] ls n

///返回符号的深度优先顺序列表。
let depthFirstSort (nodes:Map<'t,'t list>) (start:'t) =
    let rec loop (discovered:list<'t>) (unfinished:list<'t>) =
        //Console.WriteLine(stringify (discovered,unfinished))
        match unfinished with
        | [] -> discovered |> List.rev
        | current::tail ->
            match
                if nodes.ContainsKey current then
                    nodes.[current]
                    |> List.tryFind(fun x ->
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

/// 计算长度，并返回反向列表
//todo:写测试代码
let countRev ls =
    let rec loop i acc rest =
        match rest with
        | [] -> i,acc
        | h::t -> loop (i+1) (h::acc) t
    loop 0 [] ls

///分割字符串如：abcbcde会被c分解成[ab][c][b][c][de]
let splitBy (symbol:'a) (prodBody:'a list) :'a list list =
    let newGroups groups group =
        match group with
        | [] -> groups
        | _ -> (List.rev group)::groups

    let rec loop (groups:'a list list) (group:'a list) (body:'a list) =
        match body with
        | [] ->
            List.rev (newGroups groups group)
        | h::t ->
            if h = symbol then
                let groups = [h]::newGroups groups group
                loop groups [] t
            else
                loop groups (h::group) t

    loop [] [] prodBody

/// 叉幂
let crosspower n (ls:list<'a >) =
    let rec loop n (acc:list< list<'a >>) =
        if n > 1 then
            let acc = 
                List.allPairs acc ls
                |> List.map(fun (ls, a) -> ls @ [a])
            loop (n-1) acc
        else
            acc
    ls
    |> List.map(fun a -> [a])
    |> loop n
