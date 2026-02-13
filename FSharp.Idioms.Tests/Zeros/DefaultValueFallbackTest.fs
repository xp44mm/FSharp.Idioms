namespace FSharp.Idioms.Zeros

open Xunit

open System
open FSharp.xUnit
open System.Reflection
open System.Text.RegularExpressions
open FSharp.Idioms

type DefaultValueFallbackTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``bool test``() =
        let x = typeof<bool>
        let y = Literal.defaultofDynamic x :?> bool
        Should.equal y false

    [<Fact>]
    member this.``char test``() =
        let x = typeof<char>
        let y = Literal.defaultofDynamic x :?> char
        Should.equal y '\u0000'

    [<Fact>]
    member this.``sbyte test``() =
        let x = 0y
        let y = Literal.defaultofDynamic typeof<sbyte> :?> sbyte
        Should.equal x y

    [<Fact>]
    member this.``byte test``() =
        let x = 0uy
        let y = Literal.defaultofDynamic typeof<byte> :?> byte
        Should.equal x y


    [<Fact>]
    member this.``int16 test``() =
        let x = 0s
        let y = Literal.defaultofDynamic typeof<int16> :?> int16
        Should.equal x y

    [<Fact>]
    member this.``uint16 test``() =
        let x = 0us
        let y = Literal.defaultofDynamic typeof<uint16> :?> uint16
        Should.equal x y

    [<Fact>]
    member this.``int32 test``() =
        let x = 0
        let y = Literal.defaultofDynamic typeof<int32> :?> int32
        Should.equal x y

    [<Fact>]
    member this.``uint32 test``() =
        let x = 0u
        let y = Literal.defaultofDynamic typeof<uint32> :?> uint32
        Should.equal x y

    [<Fact>]
    member this.``int64 test``() =
        let x = 0L
        let y = Literal.defaultofDynamic typeof<int64> :?> int64
        Should.equal x y

    [<Fact>]
    member this.``uint64 test``() =
        let x = 0UL
        let y = Literal.defaultofDynamic typeof<uint64> :?> uint64
        Should.equal x y

    [<Fact>]
    member this.``nativeint test``() =
        let x = 0n
        let y = Literal.defaultofDynamic typeof<nativeint> :?> nativeint
        Should.equal x y

    [<Fact>]
    member this.``unativeint test``() =
        let x = 0un
        let y = Literal.defaultofDynamic typeof<unativeint> :?> unativeint
        Should.equal x y

    [<Fact>]
    member this.``single test``() =
        let x = 0.0f
        let y = Literal.defaultofDynamic typeof<single> :?> single
        Should.equal x y

    [<Fact>]
    member this.``float test``() =
        let x = 0.0
        let y = Literal.defaultofDynamic typeof<float> :?> float
        Should.equal x y

    [<Fact>]
    member this.``decimal test``() =
        let x = 0M
        let y = Literal.defaultofDynamic typeof<decimal> :?> decimal
        Should.equal x y

    [<Fact>]
    member this.``bigint test``() =
        let x = 0I
        let y = Literal.defaultofDynamic typeof<bigint> :?> bigint
        Should.equal x y

    [<Fact>]
    member this.``string test``() =
        let x = ""
        let y = Literal.defaultofDynamic typeof<string> :?> string
        Should.equal x y

    [<Fact>]
    member this.``array test``() =
        let x:int[] = [||]
        let y = Literal.defaultofDynamic typeof<int[]> :?> int[]
        Should.equal x y
        //output.WriteLine($"{x},{y.Length}")
    [<Fact>]
    member this.``tuple test``() =
        let x = 0,0.0,""
        let y = Literal.defaultofDynamic typeof<int*float*string> :?> int*float*string
        Should.equal x y

    [<Fact>]
    member this.``DBNull test``() =
        let x = DBNull.Value
        let y = Literal.defaultofDynamic typeof<DBNull> :?> DBNull
        Should.equal x y

    [<Fact>]
    member this.``Nullable test``() =
        let x = Nullable()
        let y = Literal.defaultofDynamic typeof<Nullable<int>> :?> Nullable<int>
        Should.equal x y

    [<Fact>]
    member this.``flags test``() =
        let x = BindingFlags.Default
        let y = Literal.defaultofDynamic typeof<BindingFlags> :?> BindingFlags
        Should.equal x y

    [<Fact>]
    member this.``enum test``() =
        let x = RegexOptions.None
        let y = Literal.defaultofDynamic typeof<RegexOptions> :?> RegexOptions
        Should.equal x y

    [<Fact>]
    member this.``datetimeoffset test``() =
        let y = Literal.defaultofDynamic typeof<DateTimeOffset> :?> DateTimeOffset
        Assert.IsType<DateTimeOffset>(y)

    [<Fact>]
    member this.``timespan test``() =
        let x = TimeSpan.Zero
        let y = Literal.defaultofDynamic typeof<TimeSpan> :?> TimeSpan
        Should.equal x y

    [<Fact>]
    member this.``option test``() =
        let x = None
        let y = Literal.defaultofDynamic typeof<int option> :?> int option
        Should.equal x y

    [<Fact>]
    member this.``list test``() =
        let x = []
        let y = Literal.defaultofDynamic typeof<int list> :?> int list
        Should.equal x y

    [<Fact>]
    member this.``set test``() =
        let x = Set.empty
        let y = Literal.defaultofDynamic typeof<int Set> :?> int Set
        Should.equal x y

    [<Fact>]
    member this.``map test``() =
        let x = Map.empty
        let y = Literal.defaultofDynamic typeof<Map<int,int>> :?> Map<int,int>
        Should.equal x y


