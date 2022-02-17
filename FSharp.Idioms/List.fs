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
    | [] -> failwith "List.takeLast from []"
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
[<System.Obsolete("")>]
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
            | [] -> failwith "advance source is empty"
            | hd::tail-> loop (hd::target) tail (i-1)
    loop [] ls n

///// n 个元素取出2元素，组合
//let combination2 (ls:'a list) =
//    let a = ls.[..ls.Length-2]
//    let b = ls.[1..]
//    List.zip a b
