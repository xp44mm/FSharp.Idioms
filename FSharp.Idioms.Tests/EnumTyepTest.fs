namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System.Reflection

type EnumTyepTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getEnumUnderlyingType``() =
        let x = typeof<BindingFlags> 
        let y = EnumType.getEnumUnderlyingType x
        Should.equal y typeof<int>

    [<Fact>]
    member this.``getNameValuePairs``() =
        let x = typeof<BindingFlags> 
        let y = EnumType.getNameValuePairs x
        //show y
        let res = [|
            "Default",0UL;
            "IgnoreCase",1UL;
            "DeclaredOnly",2UL;
            "Instance",4UL;
            "Static",8UL;
            "Public",16UL;
            "NonPublic",32UL;
            "FlattenHierarchy",64UL;
            "InvokeMethod",256UL;
            "CreateInstance",512UL;
            "GetField",1024UL;
            "SetField",2048UL;
            "GetProperty",4096UL;
            "SetProperty",8192UL;
            "PutDispProperty",16384UL;
            "PutRefDispProperty",32768UL;
            "ExactBinding",65536UL;
            "SuppressChangeType",131072UL;
            "OptionalParamBinding",262144UL;
            "IgnoreReturn",16777216UL;
            "DoNotWrapExceptions",33554432UL
            |]
        Should.equal y res

    [<Fact>]
    member this.``getValues``() =
        let x = typeof<BindingFlags> 
        let y = EnumType.getValues x
        //show y
        let res = Map.ofList ["CreateInstance",512UL;"DeclaredOnly",2UL;"Default",0UL;"DoNotWrapExceptions",33554432UL;"ExactBinding",65536UL;"FlattenHierarchy",64UL;"GetField",1024UL;"GetProperty",4096UL;"IgnoreCase",1UL;"IgnoreReturn",16777216UL;"Instance",4UL;"InvokeMethod",256UL;"NonPublic",32UL;"OptionalParamBinding",262144UL;"Public",16UL;"PutDispProperty",16384UL;"PutRefDispProperty",32768UL;"SetField",2048UL;"SetProperty",8192UL;"Static",8UL;"SuppressChangeType",131072UL]
        Should.equal y res

    [<Fact>]
    member this.``readFlags``() =
        let x = BindingFlags.Public ||| BindingFlags.NonPublic
        let read = EnumType.readFlags typeof<BindingFlags> 

        let y = read x

        Should.equal y [|"Public";"NonPublic";|]
