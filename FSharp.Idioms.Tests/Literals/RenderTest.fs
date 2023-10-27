namespace FSharp.Idioms.Literals

open Xunit
open Xunit.Abstractions
open System
open System.Reflection
open System.IO
open System.Text.RegularExpressions
open FSharp.xUnit

type RenderTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``render sbyte test``() =
        let x = 0y
        let y = ParenRender.stringify x
        Assert.Equal("0y",y)

    [<Fact>]
    member this.``render byte test``() =
        let x = 0uy
        let y = ParenRender.stringify x
        Assert.Equal("0uy",y)

    [<Fact>]
    member this.``render int16 test``() =
        let x = 0s
        let y = ParenRender.stringify x
        Assert.Equal("0s",y)

    [<Fact>]
    member this.``render uint16 test``() =
        let x = 0us
        let y = ParenRender.stringify x
        Assert.Equal("0us",y)

    [<Fact>]
    member this.``render int test``() =
        let x = 0
        let y = ParenRender.stringify x
        Assert.Equal("0",y)

    [<Fact>]
    member this.``render uint32 test``() =
        let x = 0u
        let y = ParenRender.stringify x
        Assert.Equal("0u",y)

    [<Fact>]
    member this.``render int64 test``() =
        let x = 0L
        let y = ParenRender.stringify x
        Assert.Equal("0L",y)

    [<Fact>]
    member this.``render uint64 test``() =
        let x = 0UL
        let y = ParenRender.stringify x
        Assert.Equal("0UL",y)

    [<Fact>]
    member this.``render nativeint test``() =
        let x = 0n
        let y = ParenRender.stringify x
        Assert.Equal("0n",y)

    [<Fact>]
    member this.``render unativeint test``() =
        let x = 0un
        let y = ParenRender.stringify x
        Assert.Equal("0un",y)

    [<Fact>]
    member this.``render single test``() =
        let x = (0.1f/3.0f)
        let y = ParenRender.stringify x
        Assert.Equal("0.033333335f",y)

        let x1 = 1.2f
        let y1 = ParenRender.stringify x1
        Assert.Equal("1.2f",y1)

        let x0 = 1.0f
        let y0 = ParenRender.stringify x0
        Assert.Equal("1.0f",y0)

    [<Fact>]
    member this.``render float test``() =
        let x = (0.1/3.)
        let y = ParenRender.stringify x
        Assert.Equal("0.03333333333333333",y)

        let x1 = 1.2
        let y1 = ParenRender.stringify x1
        Assert.Equal("1.2",y1)

        let x0 = 1.0
        let y0 = ParenRender.stringify x0
        Assert.Equal("1.0",y0)


    [<Fact>]
    member this.``render decimal test``() =
        let x = 0M
        let y = ParenRender.stringify x
        Assert.Equal("0M",y)

    [<Fact>]
    member this.``render bigint test``() =
        let x = 0I
        let y = ParenRender.stringify x
        Assert.Equal("0I",y)

    [<Fact>]
    member this.``render char test``() =
        let ls = '\t'
        let res = ParenRender.instanceToString 0 typeof<char> ls
        Assert.Equal(@"'\t'",res)

    [<Fact>]
    member this.``render string test``() =
        let ls = ""
        let res = ParenRender.instanceToString 0 typeof<string> ls
        Assert.Equal("\"\"",res)

    [<Fact>]
    member this.``render DBNull test``() =
        let x = box DBNull.Value
        let y = ParenRender.instanceToString 0 typeof<DBNull> x
        Assert.Equal("DBNull.Value",y)

    [<Fact>]
    member this.``render DateTimeOffset test``() =
        let dto = DateTimeOffset(2019,9,19,15,18,16,757,TimeSpan(0,8,0,0,0)) //DateTimeOffset.Now
        let res = ParenRender.instanceToString 0 typeof<DateTimeOffset> dto
        Assert.Equal("DateTimeOffset(2019,9,19,15,18,16,757,TimeSpan(0,8,0,0,0))",res)

    [<Fact>]
    member this.``render Guid test``() =
        let x = Guid("936da01f-9abd-4d9d-80c7-02af85c822a8") //Guid.NewGuid()
        let y = Render.stringify x
        Should.equal y "Guid(\"936da01f-9abd-4d9d-80c7-02af85c822a8\")"
        let y0 = x.ToString()
        Should.equal y0 "936da01f-9abd-4d9d-80c7-02af85c822a8"

    [<Fact>]
    member this.``render nullable test``() =
        let x = Nullable(3)
        let ni = Nullable()

        let ls = [x;ni]

        let resx = Render.stringify<Nullable<int>> x
        let resn = Render.stringify<Nullable<_>> ni
        let resl = Render.stringify<Nullable<int> list> ls

        Assert.Equal("Nullable 3",resx)
        Assert.Equal("Nullable()",resn)
        Assert.Equal("[Nullable 3;Nullable()]",resl)

    [<Fact>]
    member this.``render array test``() =
        let ls = [|1;2;3|]
        let res = ParenRender.stringify ls
        Assert.Equal("[|1;2;3|]",res)

    [<Fact>]
    member this.``render list test``() =
        let ls = [1;2;3]
        let res = ParenRender.instanceToString 0 typeof<List<int>> ls
        Assert.Equal("[1;2;3]",res)

    [<Fact>]
    member this.``render set test``() =
        Assert.Equal(ParenRender.stringify Set.empty, "set []")

        let ls = Set.ofList [1;2;3]
        let res = ParenRender.stringify ls
        Assert.Equal("set [1;2;3]",res)

    [<Fact>]
    member this.``render map test``() =
        Assert.Equal(ParenRender.stringify Map.empty, "Map []")

        let ls = Map ["1",1;"2",2;"3", 3]
        let res = ParenRender.stringify ls

        Assert.Equal("""Map ["1",1;"2",2;"3",3]""",res)

    [<Fact>]
    member this.``render HashSet test``() =
        let ls = System.Collections.Generic.HashSet [1;2;3]
        let res = ParenRender.stringify ls
        Assert.Equal("HashSet [1;2;3]",res)

    [<Fact>]
    member this.``render tuple test``() =
        let ls = ([1;2;3],"x")
        let res = ParenRender.instanceToString 0 typeof<int list * string> ls
        Should.equal res "[1;2;3],\"x\""

    [<Fact>]
    member this.``render some test``() =
        let flags = Some 123
        let res = ParenRender.instanceToString 0 typeof<int option> flags
        Assert.Equal("Some 123",res)

    [<Fact>]
    member this.``render record test``() =
        let record = {| name = "xyz"; ``your age`` = 18 |}
        let res = ParenRender.stringify record
        Assert.Equal("""{name= "xyz";``your age``= 18}""",res)

    [<Fact>]
    member this.``render null test``() =
        let ls = null
        let res = ParenRender.instanceToString 0 typeof<_> ls
        Assert.Equal("null",res)

    [<Fact>]
    member this.``render enum test``() =
        let e = FileMode.Open
        let res = ParenRender.instanceToString 0 typeof<FileMode> e
        Should.equal res "FileMode.Open"
    [<Fact>]
    member this.``render flags enum test``() =
        let flags = BindingFlags.Public ||| BindingFlags.NonPublic
        let res = ParenRender.instanceToString 0 typeof<BindingFlags> flags
        Should.equal res "BindingFlags.Public|||BindingFlags.NonPublic"

    [<Fact>]
    member this.``render flags none enum test``() =
        let none = RegexOptions.None
        let res = ParenRender.instanceToString 0 typeof<RegexOptions> none
        Should.equal res "RegexOptions.None"

    [<Fact>]
    member this.``render enum underlying value test``() =
        let none = RegexOptions.None
        let res = ParenRender.stringifyNullableType (typeof<RegexOptions>.GetEnumUnderlyingType()) none
        Should.equal res "0"

    [<Fact>]
    member this.``render type test``() =
        let ty = typeof<RegexOptions>
        let res = Render.stringify ty
        Should.equal res "typeof<RegexOptions>"

