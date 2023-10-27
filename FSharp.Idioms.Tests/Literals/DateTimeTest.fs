namespace FSharp.Idioms.Literals

open Xunit
open Xunit.Abstractions
open System
open System.Reflection

type DateTimeTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``get time zone info object``() =
        let lcl = TimeZoneInfo.Local
        output.WriteLine(sprintf "%A" lcl)
        //Assert.Equal("[1;2;3]",res)
    [<Fact>]
    member this.``time span properties``() =
        let tspan = TimeZoneInfo.Local.BaseUtcOffset

        let props = typeof<TimeSpan>.GetProperties(BindingFlags.Public|||BindingFlags.Instance)
        output.WriteLine(sprintf "%A" props)

    [<Fact>]
    member this.``date time offset properties``() =
        let dto = DateTimeOffset.Now

        let props = typeof<DateTimeOffset>.GetProperties(BindingFlags.Public|||BindingFlags.Instance)
        output.WriteLine(sprintf "%A" props)

    [<Fact>]
    member this.``time zone info properties``() =
        let lcl = TimeZoneInfo.Local

        let props = typeof<TimeZoneInfo>.GetProperties(BindingFlags.Public|||BindingFlags.Instance)
        output.WriteLine(sprintf "%A" props)
        //Assert.Equal("[1;2;3]",res)


    [<Fact>]
    member this.``how to initialize a DateTimeOffset object``() =
        

        let localTime = DateTime(2007, 07, 12, 06, 32, 00);

        //let timeSpan =
        //    match localTime.Kind with
        //    | DateTimeKind.Utc -> TimeSpan.Zero
        //    | _ -> TimeZoneInfo.Local.GetUtcOffset(localTime)

        let dateAndOffset = DateTimeOffset(localTime);
        output.WriteLine(sprintf "%A" dateAndOffset.Offset);
        // The code produces the following output:
        //    7/12/2007 6:32:00 AM -07:00

    [<Fact>]
    member this.``uint64 Length``() =
        let mins =
            (10UL,Array.zeroCreate 18)
            ||> Array.scan(fun acc _ -> acc * 10UL)
        
        //Assert.Equal(mins.Length,19)
        output.WriteLine(
            mins
            |> Array.toList
            |> ParenRender.instanceToString 0 typeof<uint64 list>
        )

        let len ul =
            let i = Array.tryFindIndex(fun mn -> ul < mn) mins
            if i.IsNone then 20 else i.Value+1

        Assert.Equal(len 0UL,1)
        Assert.Equal(len 19UL,2)
        Assert.Equal(len UInt64.MaxValue,20)
