module FSharp.Idioms.DFS

open System
open FSharp.Literals

//有向图
let adjacency<'c when 'c:comparison> (u:'c) (graph:list<'c*'c>) =
    graph
    |> List.filter(fun(b,e)-> b = u)
    |> List.map snd

let getVertices<'c when 'c:comparison> (graph:list<'c*'c>) =
    let starts,ends =
        graph
        |> List.unzip
    starts @ ends
    |> List.distinct

type Color = Black|White|Gray

type Vertex<'c when 'c:comparison> = {
    tag:'c
    color:Color
    pi:'c
    d:int
    f:int
}

let getVertex (vertices:list<Vertex<'c>>) (c:'c) =
    vertices
    |> Seq.find(fun cc -> cc.tag = c)

let setVertex (vertices:list<Vertex<'c>>) (v:Vertex<'c>) =
    vertices
    |> List.map(fun t -> if t.tag = v.tag then v else t)

let initializeVertices(graph:list<'c*'c>) =
    getVertices graph
    |> List.map(fun v -> {
            tag= v
            pi= Literal.defaultValue<'c>
            color= White
            d = 0
            f = 0
        })

let dfs (graph:list<'c*'c>) =
    //访问一整颗树
    let rec visit (time:int) (vertices:list<Vertex<'c>>) (u:'c) =
        let time = time + 1

        let vertices =
            setVertex vertices { 
                getVertex vertices u with
                    color = Gray
                    d = time
                }

        let time,vertices = 
            let adjs = adjacency u graph
            visitAdj time vertices u adjs
        let time = time + 1

        let vertices =
            setVertex vertices { 
                getVertex vertices u with
                    color = Black
                    f = time
                }

        time,vertices

    //访问临近顶点的递归
    and visitAdj (time:int) (vertices:list<Vertex<'c>>) (u:'c) (adjacentVertices:'c list) =
        match adjacentVertices with
        | [] -> time,vertices
        | v::tail ->
            let vv = getVertex vertices v
            if vv.color = White then
                let vertices =
                    setVertex vertices { 
                        vv with pi = u
                        }
                visit time vertices v
            else
                visitAdj time vertices u tail
    
    //访问森林
    let rec loop (time:int) (vertices:list<Vertex<'c>>) =
        match vertices |> List.tryFind(fun (u:Vertex<'c>) -> u.color = White) with
        | None -> vertices
        | Some(u) -> 
            let time,vertices = visit time vertices u.tag
            loop time vertices

    let vertices = initializeVertices graph

    loop 0 vertices

let test() =
    let u = 'u'
    let v = 'v'
    let w = 'w'
    let x = 'x'
    let y = 'y'
    let z = 'z'
        
    let graph = 
        [
            u,v
            v,y
            y,x
            x,v
            u,x
            w,y
            w,z
            z,z
        ] |> List.sort
    
    let y = dfs graph |> List.sortBy(fun x -> x.d)
    Console.WriteLine(Literal.stringify y)