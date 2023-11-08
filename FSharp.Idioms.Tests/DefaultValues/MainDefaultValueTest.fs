namespace FSharp.Idioms.DefaultValues

open Xunit
open Xunit.Abstractions
open FSharp.Idioms
open FSharp.xUnit

type MainDefaultValueTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``char dynamic test``() =
        let x = typeof<char>
        let y = Literal.zeroDynamic x :?> char
        Should.equal y '\u0000'

    [<Fact>]
    member this.``char test``() =
        let x = typeof<char>
        let y = Literal.zero<char>
        Should.equal y '\u0000'

