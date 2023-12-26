namespace FSharp.Idioms.Zeros

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open FSharp.Idioms

open System
open System.Reflection

type MainDefaultValueTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``char dynamic test``() =
        let x = typeof<char>
        let y = Literal.defaultofDynamic x :?> char
        Should.equal y '\u0000'

    [<Fact>]
    member this.``char test``() =
        let x = typeof<char>
        let y = Literal.defaultof<char>
        Should.equal y '\u0000'

    [<Fact>]
    member _.``Activator``() =
        let x = typeof<float>
        Assert.True(x.IsValueType)

        let y = Activator.CreateInstance(x) :?> float
        Should.equal 0.0 y

