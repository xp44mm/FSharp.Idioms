namespace FSharp.Idioms.Literals

open Xunit

open FSharp.Idioms

[<RequireQualifiedAccess>]
type Align = 
    | Left 
    | Center 
    | Right

[<RequireQualifiedAccess>]
type RQ =
    | Zero
    | One of RQ
    | Two of RQ * RQ

type RequireQualifiedAccessTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``render some test``() =
        let flags = Some 123
        let res = Literal.stringifyDynamic typeof<int option> flags
        Assert.Equal("Some 123",res)

    [<Fact>]
    member this.``render RequireQualifiedAccess union list test``() =
        let x = [Align.Left; Align.Center; Align.Right]
        let y = Literal.stringify x
        output.WriteLine(y)
        Assert.Equal("[Align.Left;Align.Center;Align.Right]",y)

    [<Fact>]
    member this.``render RequireQualifiedAccess union test``() =
        let x = Align.Left
        let y = Literal.stringify x
        output.WriteLine(y)
        Assert.Equal("Align.Left",y)

    [<Fact>]
    member this.``render RQ test``() =
        let x = RQ.Zero
        let y = Literal.stringify x
        output.WriteLine(y)
    [<Fact>]
    member this.``render RQ list test``() =
        let x = [RQ.Zero;RQ.One RQ.Zero;RQ.Two(RQ.Zero,RQ.Zero);]
        let y = Literal.stringify x
        output.WriteLine(y)
