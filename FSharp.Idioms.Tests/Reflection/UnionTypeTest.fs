namespace FSharp.Idioms.Reflection

open Xunit

open FSharp.Idioms
open FSharp.xUnit

type UionExample =
| Zero
| OnlyOne of int
| Pair of int * string

[<RequireQualifiedAccess>]
type Align = 
    | Left 
    | Center 
    | Right

type UnionTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getUnionCases``() =
        let props = 
            UnionType.getUnionCases typeof<UionExample>
            |> Array.map(fun pi -> pi.Name)
        Should.equal props [|"Zero";"OnlyOne";"Pair"|]

    [<Fact>]
    member this.``getCaseFields``() =
        let unionCases = 
            UnionType.getUnionCases typeof<UionExample>
        let y = UnionType.getCaseFields unionCases.[0]
        Should.equal y Array.empty

    [<Fact>]
    member this.``readUnion``() =
        let read = 
            UnionType.readUnion typeof<UionExample>
        let y = read <| OnlyOne 1
        Should.equal y ("OnlyOne",[|typeof<int>,box 1|])

    [<Fact>]
    member this.``getQualifiedAccess``() =
        let y = 
            UnionType.getQualifiedAccess typeof<Align>
        Should.equal y "Align."
