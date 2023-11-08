namespace FSharp.Idioms.EqualityCheckers

open Xunit
open Xunit.Abstractions
open System
open System.Text.RegularExpressions
open System.Collections.Generic

open FSharp.xUnit
open FSharp.Idioms.Literals
open FSharp.Idioms

open FSharp.Idioms.EqualityCheckers.EqualityCheckerUtils

//type Point = {x:int;y:int}

type EqualityCheckerUtilsTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``AtomEqualityChecker``() =
        Assert.True(equals 1 1)
        Assert.False(equals 0 1)

    [<Fact>]
    member this.``EnumEqualityChecker``() =
        Assert.True(equals RegexOptions.None RegexOptions.None)
        Assert.False(equals RegexOptions.None RegexOptions.Compiled)

    [<Fact>]
    member this.``UnitEqualityChecker``() =
        Assert.True(equals () ())

    [<Fact>]
    member this.``DBNullEqualityChecker``() =
        Assert.True(equals DBNull.Value DBNull.Value)

    [<Fact>]
    member this.``NullableEqualityChecker``() =
        Assert.True(equals (Nullable 0) (Nullable 0))
        Assert.False(equals (Nullable 0) (Nullable 1))

    [<Fact>]
    member this.``ArrayEqualityChecker``() =
        Assert.True(equals [|0|] [|0|])
        Assert.False(equals [|0|] [|1|])

    [<Fact>]
    member this.``TupleEqualityChecker``() =
        Assert.True(equals (1,"") (1,""))
        Assert.False(equals (1,"") (1,"1"))

    [<Fact>]
    member this.``RecordEqualityChecker``() =
        Assert.True(equals {x=1;y=1} {x=1;y=1})
        Assert.False(equals {x=1;y=1} {x=1;y=2})

    [<Fact>]
    member this.``ListEqualityChecker``() =
        Assert.True(equals [1;1] [1;1])
        Assert.False(equals [1;1] [1;2])

    [<Fact>]
    member this.``SetEqualityChecker``() =
        Assert.True(equals (Set[1;1]) (Set[1;1]))
        Assert.False(equals (Set[1;1]) (Set[1;2]))

    [<Fact>]
    member this.``MapEqualityChecker``() =
        Assert.True(equals (Map[1,"1";2,"2"]) (Map[1,"1";2,"2"]))
        Assert.False(equals (Map[1,"1";3,"3"]) (Map[1,"1";2,"2"]))

    [<Fact>]
    member this.``UnionEqualityChecker``() =
        Assert.True(equals (Some 1) (Some 1))
        Assert.False(equals (Some 1) (Some 2))

    [<Fact>]
    member this.``op_EqualityEqualityChecker``() =
        Assert.True(equals typeof<int> typeof<int>)
        Assert.False(equals typeof<int> typeof<float>)

    [<Fact>]
    member this.``SeqEqualityChecker``() =
        let ex = Assert.Throws<System.NotImplementedException>(
            fun () ->
            equals (seq [0]) (seq [0])
            |> ignore
        )
        output.WriteLine(ex.Message)
