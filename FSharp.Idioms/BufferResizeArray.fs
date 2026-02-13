namespace FSharp.Idioms

open System

///// 专用于BufferIterator
//type BufferResizeArray<'a>(arr: ResizeArray<'a>) =
//    // current min = -1 max = arr.Count-1
//    let mutable _current = -1

//    new() = BufferResizeArray(ResizeArray<'a>())

//    member _.current = _current
//    member _.count = arr.Count

//    /// 在缓存中向前移动指针
//    member _.tryNext() =
//        if _current < arr.Count - 1 then
//            _current <- _current + 1
//            Some arr.[_current]
//        else
//            None

//    ///当指针位于最后一个位置时候，可以入队一个元素
//    member _.enqueue(elem) =
//        if _current = arr.Count - 1 then
//            _current <- _current + 1
//            arr.Add elem
//        else
//            raise(invalidOp "should tryNext first.")

//    /// 出队若干元素，当前指针变为最小-1
//    member _.dequeue(count) =
//        if count < 0 then
//            raise(invalidOp "should count > 0")
//        elif count <= _current + 1 then
//            if count > 0 then
//                arr.RemoveRange(0, count)
//            _current <- -1
//        else
//            raise(invalidOp "should count <= current + 1")
