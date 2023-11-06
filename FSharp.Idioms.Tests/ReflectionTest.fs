namespace FSharp.Idioms

open System
open System.Collections.Generic

open Xunit
open Xunit.Abstractions

type ReflectionTest(output:ITestOutputHelper) =

    [<Fact>]
    member _.``IsPrimitive test`` () =
        Assert.True(typeof<bool>.IsPrimitive)
        Assert.True(typeof<byte>.IsPrimitive)
        Assert.True(typeof<sbyte>.IsPrimitive)
        Assert.True(typeof<char>.IsPrimitive)
        Assert.True(typeof<int16>.IsPrimitive)
        Assert.True(typeof<int32>.IsPrimitive)
        Assert.True(typeof<int64>.IsPrimitive)
        Assert.True(typeof<nativeint>.IsPrimitive)
        Assert.True(typeof<uint16>.IsPrimitive)
        Assert.True(typeof<uint32>.IsPrimitive)
        Assert.True(typeof<uint64>.IsPrimitive)
        Assert.True(typeof<unativeint>.IsPrimitive)
        Assert.True(typeof<float>.IsPrimitive)
        Assert.True(typeof<float32>.IsPrimitive)

        Assert.False(typeof<decimal>.IsPrimitive)
        Assert.False(typeof<bigint>.IsPrimitive)
        Assert.False(typeof<string>.IsPrimitive)

        Assert.False(typeof<DateTimeOffset>.IsPrimitive)
        Assert.False(typeof<TimeSpan>.IsPrimitive)

        Assert.False(typeof<DateTimeKind>.IsPrimitive)
        Assert.False(typeof<Type>.IsPrimitive)

        Assert.False(typeof<unit>.IsPrimitive)
        Assert.False(typeof<DBNull>.IsPrimitive)

        Assert.False(typeof<Nullable<int>>.IsPrimitive)

        Assert.False(typeof<Guid>.IsPrimitive)


