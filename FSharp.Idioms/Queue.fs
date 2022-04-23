namespace FSharp.Idioms

[<NoEquality; NoComparison>]
type Queue<'a> =
    | Queue of fs:'a list * bs:'a list

[<RequireQualifiedAccess>]
module Queue =
    let empty = Queue([], [])

    let isEmpty = function Queue([], []) -> true | _ -> false

    let enqueue e q = 
        match q with
        | Queue(fs, bs) -> Queue(e :: fs, bs)

    let dequeue q = 
        match q with
        | Queue([], []) -> failwith "Empty queue!"
        | Queue(fs, b :: bs) -> b, Queue(fs, bs)
        | Queue(fs, []) -> 
            let bs = List.rev fs
            bs.Head, Queue([], bs.Tail)

    let tryDequeue q =
        match q with
        | Queue([], []) -> None
        | Queue(fs, b :: bs) -> (b, Queue(fs, bs)) |> Some
        | Queue(fs, []) -> 
            let bs = List.rev fs
            (bs.Head, Queue([], bs.Tail)) |> Some
