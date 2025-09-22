namespace FSharp.Idioms

open System.Collections.Generic
open System

type ArrayStack<'T>(items: 'T array) =
    let mutable iStart = 0
    // iCurrent比最小值小1表示迭代还没有开始，比最大值count-1大1表示迭代已经超过一次结束。
    let mutable iCurrent = -1

    member _.count = items.Length - iStart
    member this.isEmpty = this.count = 0

    member _.current = iCurrent

    /// 尝试移动到下一个元素
    member this.tryNext() =
        if iCurrent < this.count then
            iCurrent <- iCurrent + 1
        //iCurrent已经变大了
        if iCurrent < this.count then
            //返回移动后的元素
            Some items.[iStart + iCurrent]
        else
            None

    /// 弹出指定数量的元素
    member this.pop(count) =
        if count < 0 then
            raise (ArgumentException "Count must be non-negative")
        elif count > Math.Min(iCurrent + 1, this.count) then
            raise (ArgumentException "只能弹出已经看到的元素")

        // 移动istart表示弹出新旧之间的元素
        iStart <- iStart + count
        this.reset ()

    /// 向前的指针重回开始之前
    member this.reset() = iCurrent <- -1
