﻿namespace FSharp.Idioms.DefaultValues

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open FSharp.Idioms

type Person = { name : string; age : int }

type RecordTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``record test``() =
        let x = { name = ""; age = 0 }
        let y = Literal.defaultValueDynamic typeof<Person> :?> Person
        Should.equal x y 

    [<Fact>]
    member this.``anonymous record test``() =
        let x = {| name = ""; age = 0 |}
        let y = Literal.defaultValueDynamic typeof<{| name : string; age : int |}> :?> {| name : string; age : int |}
        Should.equal x y 