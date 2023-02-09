namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open FSharp.Literals.Literal
open FSharp.Idioms.PointFree

type PointFreeTest(output: ITestOutputHelper) =
    
    [<Fact>]
    member this.``001 flip``() =
        let y = flip (-) 3 7
        Should.equal y 4

    [<Fact>]
    member this.``001 tap``() =
        let mutable ls = []

        let y = 100 |> tap (fun x -> ls <- x::ls)

        Should.equal y 100
        Should.equal ls [100]


    [<Fact>]
    member this.``002 both``() =
        let gt10 = flip (>) 10
        let lt20 = flip (<) 20
        let f = both gt10 lt20
        Assert.True(f 15)
        Assert.False(f(30))

    [<Fact>]
    member this.``003 either``() =
        let gt10 = flip (>) 10
        let even = flip (%) 2 >> (=) 0

        Assert.True(even 100)
        Assert.False(even 101)

        let f = either gt10 even
        Assert.True(f 101)
        Assert.True(f 8)

    [<Fact>]
    member this.``004 complement``() =
        let isNil = (=) null

        Assert.True(isNil(null))
        Assert.False(isNil(""))

        let isNotNil = complement isNil
        Assert.False(isNotNil null)
        Assert.True(isNotNil "")

    [<Fact>]
    member this.``005 always``() =
        //let t<'a> = always<_,'a> "kcomb"
        Should.equal "kcomb" (always "kcomb" 0)
        Should.equal "kcomb" (always "kcomb" "")
        Should.equal "kcomb" (always "kcomb" true)

    [<Fact>]
    member this.``006 truthy falsy``() =
        Assert.True(truthy 0)
        Assert.True(truthy "")
        Assert.True(truthy true)

        Assert.False(falsy 0)
        Assert.False(falsy "")
        Assert.False(falsy true)

    [<Fact>]
    member this.``007 ifElse``() =
        let forever21 = 
            ifElse
                (flip (>=) 21)
                (always 21)
                ((+)1)

        Should.equal (forever21 23) 21
        Should.equal (forever21 18) 19

    [<Fact>]
    member this.``008 case``() =
        let alwaysDrivingAge = case (flip (<) 16) (always 16)

        //满足条件，执行函数
        match 15 with x ->
        Should.equal (alwaysDrivingAge x) 16

        //否则，不变
        match 18 with x ->
        Should.equal (alwaysDrivingAge x) x

    [<Fact>]
    member this.``009 unless``() =
        let y = unless (flip (<) 0) ((+)1)

        //满足条件，不变
        match -1 with x ->
        Should.equal (y x) x

        //否则，执行函数
        match 3 with x ->
        Should.equal (y x) (x+1)


    [<Fact>]
    member this.``010 cond``() =
        let fn = cond [
            ((=)0)  , (always "water freezes at 0°C")
            ((=)100), (always "water boils at 100°C")
            truthy  , (fun temp -> $"nothing special happens at {temp}°C")
        ]
        Should.equal (fn 0)  "water freezes at 0°C"
        Should.equal (fn 50) "nothing special happens at 50°C"
        Should.equal (fn 100)"water boils at 100°C"


    [<Fact>]
    member this.``011 thunk``() =
        let fn = thunk 0
        Should.equal (fn())  0

