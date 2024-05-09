// tacit program
module FSharp.Idioms.PointFree

let flip g x y = g y x

let tap exec x = 
    exec x
    x

let both f g x = f x && g x

let either f g x = f x || g x

let complement f = not << f

///两个输入值，结果总是第一个值
let always<'a,'b> (x:'a) (_:'b) = x

let truthy<'b> = always<bool,'b> true

let falsy<'b> = always<bool,'b> false

let ifElse predicate onTrue onFalse x =
    if predicate x then onTrue x else onFalse x

/// if not pred then nochange else .
let case predicate onTrue (*x*) = 
    ifElse predicate onTrue id (*x*)

/// if pred=true then onTrue x else nochange.
let unless predicate onFalse = 
    ifElse predicate id onFalse

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

//柯里函数元组化
let tuple2 fn (a,b) = fn a b
let tuple3 fn (a,b,c) = fn a b c
let tuple4 fn (a,b,c,d) = fn a b c d
let tuple5 fn (a,b,c,d,e) = fn a b c d e

//元组函数柯里化
let curry2 fn a b = fn(a, b)
let curry3 fn a b c = fn(a, b, c)
let curry4 fn a b c d = fn(a, b, c, d)
let curry5 fn a b c d e = fn(a, b, c, d, e)

let twice fn a = fn a a
