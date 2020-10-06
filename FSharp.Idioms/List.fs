module FSharp.Idioms.List

/// 取列表前面所有断言为真的项，加上第一个断言为假的项（如果有）
let penetrate (predicate:'t->bool) (ls:'t list) =
    let rec loop (acc:'t list) (ls:'t list) =
        match ls with
        | [] -> acc
        | h::t ->
            let acc = h :: acc
            if predicate h then
                loop acc t
            else
                acc
    loop [] ls

///// n 个元素取出2元素，组合
//let combination2 (ls:'a list) =
//    let a = ls.[..ls.Length-2]
//    let b = ls.[1..]
//    List.zip a b
