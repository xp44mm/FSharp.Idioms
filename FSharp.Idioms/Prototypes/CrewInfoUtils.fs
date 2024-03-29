﻿module FSharp.Idioms.Prototypes.CrewInfoUtils

open FSharp.Reflection
open FSharp.Idioms.Literal

open System
open System.Collections.Generic
open System.Text.RegularExpressions
open System.Reflection

/// 第一行的声明
let generateClassDecl (crew:CrewInfo) =
    let declParams =
        [
            match crew.prototype with
            | Some prototype -> yield $"prototype:{prototype.typeName}"
            | None -> ()

            yield!
                crew.fields
                |> List.map(fun (name,fieldType) ->
                    $"{name}:{stringifyTypeDynamic fieldType}"
                )

        ]
        |> String.concat ","
    $"type {crew.typeName}({declParams}) ="

/// 获得构造函数的
/// inherit语句需要递归生成
let generateClassCtor (crew:CrewInfo) =
    let otherArgs =
        crew.fields
        |> List.map fst

    let fullArgs =
        [
            match crew.prototype with
            | Some crew1 -> yield "prototype"
            | None -> ()
            yield! otherArgs
        ]
        |> String.concat ","
    $"{crew.typeName}({fullArgs})"

/// 获得构造函数的
/// inherit语句需要递归生成
let rec generateClassInherit (crew:CrewInfo) =
    let fullArgs =
        [
            match crew.prototype with
            | Some protoCrew -> yield generateClassInherit protoCrew
            | None -> ()
            yield!
                crew.fields
                |> List.map(fun (nm,_) -> "prototype." + nm)
        ]
        |> String.concat ","
    $"{crew.typeName}({fullArgs})"

/// 获得构造函数的
/// inherit语句需要递归生成
let generateClassMembers (crew:CrewInfo) =
    crew.fields
    |> List.map(fun (name,tp) -> $"    member _.{name} = {name}")
    |> String.concat "\r\n"

let generateClassDefinition (crew:CrewInfo) =
    [
        yield $"// {generateClassCtor crew}"
        yield generateClassDecl crew
        match crew.prototype with
        | Some prototype ->
            yield $"    inherit {generateClassInherit prototype}"
        | None -> ()
        yield generateClassMembers crew
    ]
    |> String.concat "\r\n"

let filterCrewInfos foldPath fileName (crewInfos:array<string*CrewInfo>) =
    crewInfos
    |> Array.filter(fun(ns,_)->
        let arr = ns.Split [|'.'|]
        let path = arr.[arr.Length-2]
        let name = arr.[arr.Length-1]
        foldPath = path && fileName = name)
    |> Array.map(snd)

let getFile foldPath fileName (crewInfos:array<string*CrewInfo>) =
    let fileCrewInfos =
        crewInfos
        |> Array.filter(fun(ns,_)->
            let arr = ns.Split [|'.'|]
            let path = arr.[arr.Length-2]
            let name = arr.[arr.Length-1]
            foldPath = path && fileName = name)
        |> Array.map(snd)

    [
        $"namespace FslexFsyacc.{foldPath}.{fileName}"
        "open FslexFsyacc.Runtime"
        "open FslexFsyacc.Yacc.ItemCoreCrews"
        ""
        yield!
            fileCrewInfos
            |> Seq.map(fun crew -> $"{generateClassDefinition crew}\n")
    ]
    |> String.concat "\r\n"

