namespace FSharp.Idioms

open Xunit

open FSharp.xUnit
open System
open System.Collections.Generic
open Xunit
open FSharp.Idioms


type IteratorTest (output: ITestOutputHelper) =

    [<Fact>]
    member _.``新建 Iterator 时 current 应该为 None`` () =
        let list = [1; 2; 3]
        let iterator = Iterator(list)
        Assert.Equal(None, iterator.current)

    [<Fact>]
    member _.``tryNext 第一次调用应该返回第一个元素`` () =
        let list = [1; 2; 3]
        let iterator = Iterator(list)
        let result = iterator.tryNext()
        Assert.Equal(Some 1, result)
        Assert.Equal(Some 1, iterator.current)

    [<Fact>]
    member _.``可以遍历所有元素`` () =
        let list = [1; 2; 3]
        let iterator = Iterator(list)
        
        let first = iterator.tryNext()
        Assert.Equal(Some 1, first)
        Assert.Equal(Some 1, iterator.current)
        
        let second = iterator.tryNext()
        Assert.Equal(Some 2, second)
        Assert.Equal(Some 2, iterator.current)
        
        let third = iterator.tryNext()
        Assert.Equal(Some 3, third)
        Assert.Equal(Some 3, iterator.current)
        
        let fourth = iterator.tryNext()
        Assert.Equal(None, fourth)
        Assert.Equal(None, iterator.current)

    [<Fact>]
    member _.``遍历完成后继续调用 tryNext 应该始终返回 None`` () =
        let list = [1]
        let iterator = Iterator(list)
        
        iterator.tryNext() |> ignore // 获取第一个元素
        let afterFirst = iterator.tryNext() // 应该返回 None
        let afterSecond = iterator.tryNext() // 应该仍然返回 None
        
        Assert.Equal(None, afterFirst)
        Assert.Equal(None, afterSecond)
        Assert.Equal(None, iterator.current)

    [<Fact>]
    member _.``空序列的行为测试`` () =
        let emptyList: int list = []
        let iterator = Iterator(emptyList)
        
        Assert.Equal(None, iterator.current)
        
        let firstTry = iterator.tryNext()
        Assert.Equal(None, firstTry)
        Assert.Equal(None, iterator.current)
        
        let secondTry = iterator.tryNext()
        Assert.Equal(None, secondTry)

    [<Fact>]
    member _.``从 IEnumerator 构造测试`` () =
        let list = [|"a"; "b"; "c"|]
        let enumerator = (list :> IEnumerable<string>).GetEnumerator()
        let iterator = Iterator<string>(enumerator)
        
        Assert.Equal(None, iterator.current)
        
        let first = iterator.tryNext()
        Assert.Equal(Some "a", first)
        Assert.Equal(Some "a", iterator.current)
        
        let second = iterator.tryNext()
        Assert.Equal(Some "b", second)

    [<Fact>]
    member _.``复杂类型测试`` () =
        let persons = [
            {| Name = "Alice"; Age = 30 |}
            {| Name = "Bob"; Age = 25 |}
        ]
        
        let iterator = Iterator(persons)
        
        let first = iterator.tryNext()
        match first with
        | Some person ->
            Assert.Equal("Alice", person.Name)
            Assert.Equal(30, person.Age)
        | None -> Assert.True(false, "应该有一个元素")
        
        let second = iterator.tryNext()
        match second with
        | Some person ->
            Assert.Equal("Bob", person.Name)
            Assert.Equal(25, person.Age)
        | None -> Assert.True(false, "应该有第二个元素")
        
        let third = iterator.tryNext()
        Assert.Equal(None, third)

    [<Fact>]
    member _.``多次访问 current 应该返回相同值`` () =
        let list = [42]
        let iterator = Iterator(list)
        
        iterator.tryNext() |> ignore // 推进到第一个元素
        
        let current1 = iterator.current
        let current2 = iterator.current
        let current3 = iterator.current
        
        Assert.Equal(Some 42, current1)
        Assert.Equal(Some 42, current2)
        Assert.Equal(Some 42, current3)

    [<Fact>]
    member _.``tryNext 推进迭代器但不会改变之前返回的 Option 值`` () =
        let list = [1; 2]
        let iterator = Iterator(list)
        
        let firstCallResult = iterator.tryNext() // 返回 Some(1)
        iterator.tryNext() |> ignore // 推进到第二个元素
        
        // 第一次调用的结果应该保持不变
        Assert.Equal(Some 1, firstCallResult)
        // current 应该更新为最新值
        Assert.Equal(Some 2, iterator.current)
