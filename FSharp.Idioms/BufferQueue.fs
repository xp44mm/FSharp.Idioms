namespace FSharp.Idioms

open System

///// 专用于BufferIterator
//type BufferQueue<'a>() =
//    let mutable forwards: list<'a> = [] // [e;f;g;h]
//    let mutable backwards: list<int * 'a> = [] // [(3,d);(2,c);(1,b);(0,a)]

//    /// get current pointer: 指针位于backwards顶部
//    let getBackCount () =
//        //背向元素需要记录索引，新增元素的索引就是backwards.length
//        match backwards with
//        | [] -> 0
//        | (i, _) :: _ -> i + 1

//    member _.isEmpty = forwards.IsEmpty && backwards.IsEmpty

//    member _.tryNext() =
//        //正向读取
//        match forwards with
//        | [] -> None
//        | head :: tail ->
//            let i = getBackCount ()
//            backwards <- (i, head) :: backwards
//            forwards <- tail
//            Some head

//    ///入队一个元素
//    member _.enqueue(elem) =
//        match forwards with
//        | [] ->
//            let i = getBackCount ()
//            backwards <- (i, elem) :: backwards
//        | _ ->
//            raise (
//                invalidOp $"{nameof BufferQueue}:Do not allow queue jumping."
//            )

//    member this.dequeue(count) =
//        if count < 0 then
//            failwith "count should > 0"
//        if count = 0 then
//            this.dequeueNothing ()

//        if count > getBackCount () then
//            raise (invalidOp $"{nameof BufferQueue}:Not count enough queue.")
//        else
//            let j = count - 1
//            // get new forwards: bs -> fs
//            let rec loop bs fs =
//                match bs with
//                | [] -> failwith $"{nameof BufferQueue}:never"
//                | (i, e) :: bs -> if j >= i then fs else loop bs (e :: fs)
//            forwards <- loop backwards forwards
//            backwards <- []

//    /// 出队0个元素，指针置于队头
//    member _.dequeueNothing() =
//        // get new forwards: bs -> fs
//        let rec loop bs fs =
//            match bs with
//            | [] -> fs
//            | (_, hd) :: bs -> loop bs (hd :: fs)
//        forwards <- loop backwards forwards
//        backwards <- []

//    member this.dequeueToCurrent() = backwards <- []
