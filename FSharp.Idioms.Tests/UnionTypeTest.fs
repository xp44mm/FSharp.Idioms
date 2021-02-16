namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open Microsoft.FSharp.Reflection

type UionExample =
| Zero
| OnlyOne of int
| Pair of int * string

type UnionTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
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
