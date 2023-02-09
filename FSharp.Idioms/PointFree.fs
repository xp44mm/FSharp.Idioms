// tacit program
module FSharp.Idioms.PointFree

let flip g x y = g y x

let tap exec x = 
    exec x
    x

let both f g x = f x && g x

let either f g x = f x || g x

let complement f = not << f

let always<'a,'b> (x:'a) (_:'b) = x

let truthy<'a> = always<_,'a> true

let falsy<'a> = always<_,'a> false

let ifElse predicate onTrue onFalse x =
    if predicate x 
    then onTrue x
    else onFalse x

let case predicate onTrue = ifElse predicate onTrue id

let unless predicate onFalse = ifElse predicate id onFalse

let cond (ls:(('a->bool)*('a->'b))list) x =
    match ls
        |> List.tryPick(fun(predicate,transformer)->
            if predicate x 
            then Some transformer
            else None
        ) 
    with
    | Some transformer -> transformer x
    | _ -> failwith "cond fail because all false"

let thunk x () = x
