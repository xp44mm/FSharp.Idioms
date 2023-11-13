namespace FSharp.Idioms.EqualityCheckers

open Xunit
open Xunit.Abstractions
open System
open System.Text.RegularExpressions
open System.Collections.Generic

open FSharp.xUnit
open FSharp.Idioms.Literals
open FSharp.Idioms

open FSharp.Idioms.EqualityCheckers.EqualityComparerUtils

//type Point = {x:int;y:int}

type EqualityCheckerUtilsTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``AtomEqualityChecker``() =
        Assert.True(equal(1, 1))
        Assert.False(equal(0, 1))

    [<Fact>]
    member this.``EnumEqualityChecker``() =
        Assert.True(equal (RegexOptions.None, RegexOptions.None))
        Assert.False(equal( RegexOptions.None ,RegexOptions.Compiled))

    [<Fact>]
    member this.``UnitEqualityChecker``() =
        Assert.True(equal( (), ()))

    [<Fact>]
    member this.``DBNullEqualityChecker``() =
        Assert.True(equal (DBNull.Value, DBNull.Value))

    [<Fact>]
    member this.``NullableEqualityChecker``() =
        Assert.True(equal ((Nullable 0), (Nullable 0)))
        Assert.False(equal( (Nullable 0), (Nullable 1)))

    [<Fact>]
    member this.``ArrayEqualityChecker``() =
        Assert.True(equal ([|0|], [|0|]))
        Assert.False(equal( [|0|], [|1|]))

    [<Fact>]
    member this.``TupleEqualityChecker``() =
        Assert.True(equal ((1,""), (1,"")))
        Assert.False(equal ((1,""), (1,"1")))

    [<Fact>]
    member this.``RecordEqualityChecker``() =
        Assert.True(equal ({x=1;y=1}, {x=1;y=1}))
        Assert.False(equal( {x=1;y=1}, {x=1;y=2}))

    [<Fact>]
    member this.``ListEqualityChecker``() =
        Assert.True(equal( [1;1], [1;1]))
        Assert.False(equal ([1;1], [1;2]))

    [<Fact>]
    member this.``SetEqualityChecker``() =
        Assert.True(equal ((Set[1;1]), (Set[1;1])))
        Assert.False(equal ((Set[1;1]), (Set[1;2])))

    [<Fact>]
    member this.``MapEqualityChecker``() =
        Assert.True(equal ((Map[1,"1";2,"2"]), (Map[1,"1";2,"2"])))
        Assert.False(equal( (Map[1,"1";3,"3"]), (Map[1,"1";2,"2"])))

    [<Fact>]
    member this.``UnionEqualityChecker``() =
        Assert.True(equal ((Some 1), (Some 1)))
        Assert.False(equal( (Some 1), (Some 2)))

    [<Fact>]
    member this.``op_EqualityEqualityChecker``() =
        Assert.True(equal (typeof<int>, typeof<int>))
        Assert.False(equal (typeof<int>, typeof<float>))

    [<Fact>]
    member this.``SeqEqualityChecker``() =
        Assert.True(equal ((seq [0]), (seq [0])))

    [<Fact>]
    member _.``ProductionCrewEqualityChecker test`` () =
        let x = ProductionCrew([""],"",[])
        let y = ProductionCrew([""],"",[])
        let z = ProductionCrew(["w"],"w",[])

        Assert.True(CustomDataExample.equal(x, y))
        Assert.False(CustomDataExample.equal(x, z))

        let m1 = Map [1,x;3,z]
        let m2 = Map [1,y;3,z]

        Assert.True(CustomDataExample.equal(m1, m2))

    [<Fact>]
    member _.``testInterface test`` () =
        let testInterface (ty:Type) ifnm =
            let nm = 
                match ty.GetInterface(ifnm) with 
                null -> "<null>" 
                | x -> x.Name
            output.WriteLine($"{nm}:{ty.Name}")
        let ifnm1 = "IStructuralEquatable"
        testInterface typeof<list<int>>       ifnm1
        testInterface typeof<Set<int>>        ifnm1
        testInterface typeof<Map<int,string>> ifnm1
        testInterface typeof<int*string>      ifnm1
        testInterface typeof<int[]>           ifnm1
        testInterface typeof<option<int>>     ifnm1
        output.WriteLine("")
        let ifnm2 = "IStructuralComparable" 
        testInterface typeof<list<int>>       ifnm2
        testInterface typeof<Set<int>>        ifnm2
        testInterface typeof<Map<int,string>> ifnm2
        testInterface typeof<int*string>      ifnm2
        testInterface typeof<int[]>           ifnm2
        testInterface typeof<option<int>>     ifnm2


