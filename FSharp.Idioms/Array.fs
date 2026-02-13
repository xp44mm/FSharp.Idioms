[<RequireQualifiedAccess>]
module FSharp.Idioms.Array

let tap f (source: 'a[]) =
    source
    |> Array.map(fun x ->
        f x
        x
    )

let tapi f (source: 'a[]) =
    source
    |> Array.mapi(fun i x ->
        f i x
        x
    )

let toRevList (source: 'a[]) = source |> List.ofRevArray

/// 将键值对数组转化为Map，键可能重复的，相同键的值收集到数组中。
let toMap (pairs: ('k * 'v)[]) =
    pairs
    |> Array.groupBy fst
    |> Array.map(fun (k, pairs) ->
        let values = pairs |> Array.map snd
        k, values
    )
    |> Map.ofArray

/// 将键值对数组转化为二级JaggedMap，键可能重复的，相同键的值收集到数组中。
let toJaggedMap (triples: ('x * 'y * 'z)[]) =
    triples
    |> Array.groupBy Triple.first
    |> Array.map(fun (x, triples) ->
        let mp = triples |> Array.map Triple.lastTwo |> toMap
        x, mp
    )
    |> Map.ofArray

/// 将键值对数组转化为二级JaggedMap，相同键的值后来者赢。
let toUniqueJaggedMap (triples: ('x * 'y * 'z)[]) =
    triples
    |> Array.groupBy Triple.first
    |> Array.map(fun (x, triples) ->
        let mp = triples |> Array.map Triple.lastTwo |> Map.ofArray
        x, mp
    )
    |> Map.ofArray

/// 从二级JaggedMap构造数组，生成三元组的数组
let ofJaggedMap (mp: Map<'u, Map<'v, 'w>>) =
    mp
    |> Map.toArray
    |> Array.map(fun (u, v) ->
        v
        |> Map.toArray
        |> Array.map(fun (v, w) -> u, v, w)
    )
    |> Array.concat

/// 把所有行的列数对齐到最大,value是填充值
let alignCols (value: 'a) (cells: 'a[][]) =
    if cells.Length > 0 then
        let mlen =
            cells
            |> Array.map(fun arr -> arr.Length)
            |> Array.max
        cells
        |> Array.map(fun arr ->
            match mlen - arr.Length with
            | 0 -> arr
            | dlen -> [|
                yield! arr
                for _ in [ 1..dlen ] do
                    yield value
              |]
        )
    else
        [||]

let createJaggedArray<'a> rows cols = [|
    for r in [ 0 .. rows - 1 ] do
        [|
            for c in [ 0 .. cols - 1 ] do
                Unchecked.defaultof<'a>
        |]
|]

/// 根据给定的索引列表分割数组
/// indices: 分割点的索引列表（每个索引是各个部分的起始位置）
/// array: 要分割的数组
/// 返回：每个部分的 (起始索引, 结束索引, 子数组)
let splitByIndex (indices: int[]) (array: 'T[]) =
    if Array.isEmpty indices then
        [| (0, array.Length - 1, array) |]
    else
        // 构建完整的索引序列，包含结尾
        let allIndices = [|
            if indices.[0] > 0 then 0
            yield! indices
            array.Length
        |]

        // 使用pairwise获取范围
        allIndices
        |> Array.pairwise
        |> Array.mapi(fun i (start, next) ->
            let endIdx = next - 1
            let slice =
                if endIdx >= start then
                    array.[start..endIdx]
                else
                    [||]
            (start, endIdx, slice)
        )
