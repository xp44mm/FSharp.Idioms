namespace FSharp.Idioms

open System

/// 专用于BufferIterator
type BufferQueue<'a>() =
    let mutable forwards :list<'a> = [] // [e;f;g;h]
    let mutable backwards:list<int*'a> = [] // [(3,d);(2,c);(1,b);(0,a)]

    /// get current pointer: 指针位于backwards顶部
    let getBackCount() =
        match backwards with
        | [] -> 0
        | (i,_)::_ -> i+1

    member this.isEmpty = forwards.IsEmpty && backwards.IsEmpty

    member this.enqueue (elem) =
        match forwards with
        | [] ->
            let i = getBackCount()
            backwards <- (i,elem)::backwards
        | _ -> raise(invalidOp $"{nameof BufferQueue}:Do not allow queue jumping.")

    /// 出队0个元素，指针置于队头
    member _.dequequeNothing() =
        // get new forwards: bs -> fs
        let rec loop bs fs =
            match bs with
            | [] -> fs
            | (_,hd)::bs -> loop bs (hd::fs)
        forwards <- loop backwards forwards
        backwards <- []

    member this.dequeueToCurrent() =
        backwards <- []

    member this.dequeque(count) =
        if count < 0 then failwith "count should > 0"
        if count = 0 then this.dequequeNothing()

        if count > getBackCount() then
            raise(invalidOp $"{nameof BufferQueue}:Not count enough queue.")
        else
            // get new forwards: bs -> fs
            let rec loop bs fs =
                match bs with
                | [] -> failwith $"{nameof BufferQueue}:never"
                | (i,e)::bs -> 
                    if count=i+1 then
                        fs
                    else
                        loop bs (e::fs)
            forwards <- loop backwards forwards
            backwards <- []

    member this.tryNext() =
        match forwards with
        | [] -> None
        | head::tail ->
            let i = getBackCount()
            backwards <- (i,head)::backwards
            forwards <- tail
            Some head


