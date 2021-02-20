namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals

type SeqTypeTest(output: ITestOutputHelper) =

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
        Render.stringify arr
        |> output.WriteLine

