namespace FSharp.Idioms.Reflection

open Xunit

open FSharp.Idioms
open System.Collections

type IEnumerableTypeTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``get seq reader``() =
        let seqTypeDef = typeof<seq<_>>.GetGenericTypeDefinition()
        let giterorDef = typeof<System.Collections.Generic.IEnumerator<_>>.GetGenericTypeDefinition()
        let biteror = typeof<System.Collections.IEnumerator>
        let mMoveNext = biteror.GetMethod("MoveNext")

        //以上是反射方法的准备，下面是读取元素的过程
        let ls = seq [1;2;3]
        let ty = ls.GetType()
        let arguments = ty.GetGenericArguments()
        
        let seqType = seqTypeDef.MakeGenericType arguments
        let mGetEnumerator = seqType.GetMethod("GetEnumerator")

        let enumerator = mGetEnumerator.Invoke(ls,[||])
        let giteror = giterorDef.MakeGenericType arguments
        let pCurrent = giteror.GetProperty("Current")

        let arr =
            [|
                while(mMoveNext.Invoke(enumerator,[||])|>unbox<bool>) do
                    yield pCurrent.GetValue(enumerator)
            |]
        Literal.stringify arr
        |> output.WriteLine

    [<Fact>]
    member this.``GetEnumerator Test``() =
        let mi = IEnumerableType.GetEnumerator
        Assert.Equal(mi.Name,"GetEnumerator")

    [<Fact>]
    member this.``getEnumerator Test``() =
        let sq = seq [1..5] |> box
        let enm = 
            IEnumerableType.getEnumerator sq

        Assert.True(enm.MoveNext())
        Assert.Equal(enm.Current,1)

    [<Fact>]
    member this.``toarray Test``() =
        let sq = seq [1..5] |> box
        let enm = 
            IEnumerableType.getEnumerator sq

        let arr = [|
            while enm.MoveNext() do
                yield enm.Current :?> int
        |]

        sq 
        :?> seq<int> 
        |> Seq.toArray
        |> Array.zip arr
        |> Array.iter(fun(a,e)->
            Assert.Equal(a,e)
        )

    [<Fact>]
    member this.``MoveNext Test``() =
        let mi = IEnumeratorType.MoveNext
        Assert.Equal(mi.Name,"MoveNext")

    [<Fact>]
    member this.``Current Test``() =
        let mi = IEnumeratorType.Current
        Assert.Equal(mi.Name,"Current")


    [<Fact>]
    member this.``moveNext current Test``() =
        let sq = seq [1..5] |> box
        let enm = 
            IEnumerableType.getEnumerator sq

        Assert.True(IEnumeratorType.moveNext enm)
        Assert.Equal(IEnumeratorType.current enm,1)

