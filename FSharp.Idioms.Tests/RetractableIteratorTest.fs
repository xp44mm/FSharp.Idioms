namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open FSharp.Literals.Literal

type RetractableIteratorTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``Iterator``() =       
        let iterable = Seq.ofList ['a'; 'b']       
        let iterator = Iterator(iterable.GetEnumerator())
        
        let res1 =   iterator.tryNext()
        Should.equal res1 (Some 'a')
          
        let res2 =   iterator.tryNext()
        Should.equal res2 (Some 'b')

        let res3 =   iterator.tryNext()
        Should.equal res3 (None)

        let res4 =   iterator.tryNext()
        Should.equal res4 (None)

    [<Fact>]
    member this.``retractable Iterator``() =       
        let iterable = Seq.ofList ['a'; 'b']       
        let iterator = RetractableIterator(iterable.GetEnumerator())
        
        let res1 =   iterator.tryNext()
        Should.equal res1 (Some 'a')
          
        let res2 =   iterator.tryNext()
        Should.equal res2 (Some 'b')

        let res3 =   iterator.tryNext()
        Should.equal res3 None

        let res4 =   iterator.tryNext()
        Should.equal res4 None

        let tokens =iterator.dequeue(1)
        Should.equal tokens [|'a'|]

        let res5 =   iterator.tryNext()
        Should.equal res5 (Some 'b')

        let res6 =   iterator.tryNext()
        Should.equal res6 (None)

        let res7 =   iterator.tryNext()
        Should.equal res7 (None)

        let tokens =iterator.dequeue(0)
        Should.equal tokens [||]

    [<Fact>]
    member this.``dequeque of retractable Iterator``() =       
        let iterable = Seq.ofList ['a'; 'b']       
        let iterator = RetractableIterator(iterable.GetEnumerator())
        
        let res1 = iterator.tryNext()
        Should.equal res1 (Some 'a')
          
        iterator.dequequeNothing()

        /// 除非程序确定知道缓存的大小，否则应该试探者读取缓存
        while iterator.ongoing() do
            match iterator.tryNext() with
            | Some _ ->
                let elem = iterator.dequeueHead()
                output.WriteLine($"{stringify elem}")
            | None -> ()
