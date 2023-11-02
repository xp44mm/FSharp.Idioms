namespace FSharp.Idioms.DefaultValues

open Xunit
open Xunit.Abstractions
open System
open FSharp.Idioms.Literals
open FSharp.xUnit
open FSharp.Idioms

type UionExample0 =
| Zero

type UionExample1 =
| OnlyOne of int

type UionExample2 =
| Pair of int * string

type UnionTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``UionExample0``() =
        let x = Zero
        let y = Literal.defaultValueDynamic typeof<UionExample0> :?> UionExample0
        Should.equal x y 

    [<Fact>]
    member this.``UionExample1``() =
        let x = OnlyOne 0
        let y = Literal.defaultValueDynamic typeof<UionExample1> :?> UionExample1
        Should.equal x y 

    [<Fact>]
    member this.``UionExample2``() =
        let x = Pair (0,"")
        let y = Literal.defaultValueDynamic typeof<UionExample2> :?> UionExample2
        Should.equal x y 

