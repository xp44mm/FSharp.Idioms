namespace FSharp.Idioms.EqualityCheckers

open Xunit
open Xunit.Abstractions
open System
open System.Text.RegularExpressions
open FSharp.xUnit
open FSharp.Idioms.Literals
open FSharp.Idioms
open System.Collections.Generic

type Point = {x:int;y:int}

type EqualityCheckersTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``AtomEqualityChecker``() =
        let checker = EqualityCheckers.AtomEqualityChecker typeof<int>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> true)
        Assert.True(eq 1 1)
        Assert.False(eq 0 1)

    [<Fact>]
    member this.``EnumEqualityChecker``() =
        let checker = EqualityCheckers.EnumEqualityChecker typeof<RegexOptions> 

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> true)
        Assert.True(eq RegexOptions.None RegexOptions.None)

    [<Fact>]
    member this.``UnitEqualityChecker``() =
        let checker = EqualityCheckers.UnitEqualityChecker typeof<unit>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> true)
        Assert.True(eq () ())

    [<Fact>]
    member this.``DBNullEqualityChecker``() =
        let checker = EqualityCheckers.DBNullEqualityChecker typeof<DBNull>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> true)
        Assert.True(eq DBNull.Value DBNull.Value)

    [<Fact>]
    member this.``NullableEqualityChecker``() =
        let checker = EqualityCheckers.NullableEqualityChecker typeof<Nullable<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (Nullable 0) (Nullable 0))
        Assert.False(eq (Nullable 0) (Nullable 1))

    [<Fact>]
    member this.``ArrayEqualityChecker``() =
        let checker = EqualityCheckers.ArrayEqualityChecker typeof<array<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq [|0|] [|0|])
        Assert.False(eq [|0|] [|1|])

    [<Fact>]
    member this.``TupleEqualityChecker``() =
        let checker = EqualityCheckers.TupleEqualityChecker typeof<int*string>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (1,"") (1,""))
        Assert.False(eq (1,"") (1,"1"))

    [<Fact>]
    member this.``RecordEqualityChecker``() =
        let checker = EqualityCheckers.RecordEqualityChecker typeof<Point>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq {x=1;y=1} {x=1;y=1})
        Assert.False(eq {x=1;y=1} {x=1;y=2})

    [<Fact>]
    member this.``ListEqualityChecker``() =
        let checker = EqualityCheckers.ListEqualityChecker typeof<list<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq [1;1] [1;1])
        Assert.False(eq [1;1] [1;2])

    [<Fact>]
    member this.``SetEqualityChecker``() =
        let checker = EqualityCheckers.SetEqualityChecker typeof<Set<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (Set[1;1]) (Set[1;1]))
        Assert.False(eq (Set[1;1]) (Set[1;2]))

    [<Fact>]
    member this.``MapEqualityChecker``() =
        let checker = EqualityCheckers.MapEqualityChecker typeof<Map<int,string>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (Map[1,"1";2,"2"]) (Map[1,"1";2,"2"]))
        Assert.False(eq (Map[1,"1";3,"3"]) (Map[1,"1";2,"2"]))

    [<Fact>]
    member this.``UnionEqualityChecker``() =
        let checker = EqualityCheckers.UnionEqualityChecker typeof<option<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (Some 1) (Some 1))
        Assert.False(eq (Some 1) (Some 2))

    [<Fact>]
    member this.``HashSetEqualityChecker``() =
        let checker = EqualityCheckers.HashSetEqualityChecker typeof<HashSet<Type>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (HashSet[typeof<int>]) (HashSet[typeof<int>]))
        Assert.False(eq (HashSet[typeof<int>]) (HashSet[typeof<float>]))

    [<Fact>]
    member this.``op_EqualityEqualityChecker``() =
        let checker = EqualityCheckers.op_EqualityEqualityChecker typeof<Type>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq typeof<int> typeof<int>)
        Assert.False(eq typeof<int> typeof<float>)



    [<Fact>]
    member this.``SeqEqualityChecker``() =
        let checker = EqualityCheckers.SeqEqualityChecker typeof<seq<int>>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)

        let ex = Assert.Throws<NotImplementedException>(
            fun () ->
            eq [|0|] [|0|]
            |> ignore
        )
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``SeqEqualityChecker Internal``() =
        let checker = EqualityCheckerInternal.SeqEqualityChecker typeof<seq<int>>
        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq [0] [0])

    [<Fact>]
    member this.``IEquatableEqualityChecker``() =
        let checker = EqualityCheckers.IEquatableEqualityChecker typeof<bool>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)

        let ex = Assert.Throws<NotImplementedException>(
            fun () ->
            eq true true
            |> ignore
        )
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``IEquatableEqualityChecker Internal``() =
        let checker = EqualityCheckerInternal.IEquatableEqualityChecker typeof<bool>
        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq true true)

    [<Fact>]
    member this.``IComparableEqualityChecker``() =
        let checker = EqualityCheckers.IComparableEqualityChecker typeof<bool>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)

        let ex = Assert.Throws<NotImplementedException>(
            fun () ->
            eq true true
            |> ignore
        )
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``IComparableEqualityChecker Internal``() =
        let checker = EqualityCheckerInternal.IComparableEqualityChecker typeof<bool>
        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq true true)

    [<Fact>]
    member this.``IStructuralEquatableEqualityChecker``() =
        let checker = EqualityCheckers.IStructuralEquatableEqualityChecker typeof<int*string>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)

        let ex = Assert.Throws<NotImplementedException>(
            fun () ->
            eq (1,"") (1,"")
            |> ignore
        )
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``IStructuralEquatableEqualityChecker Internal``() =
        let checker = EqualityCheckerInternal.IStructuralEquatableEqualityChecker typeof<int*string>
        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (1,"") (1,""))


    [<Fact>]
    member this.``IStructuralComparableEqualityChecker``() =
        let checker = EqualityCheckers.IStructuralComparableEqualityChecker typeof<int*string>

        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)

        let ex = Assert.Throws<NotImplementedException>(
            fun () ->
            eq (1,"") (1,"")
            |> ignore
        )
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``IStructuralComparableEqualityChecker Internal``() =
        let checker = EqualityCheckerInternal.IStructuralComparableEqualityChecker typeof<int*string>
        Assert.True(checker.check)
        let eq = checker.equal (fun ty x y -> x=y)
        Assert.True(eq (1,"") (1,""))
