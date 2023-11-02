module FSharp.Idioms.BFS

// 朴素求无向图的邻接点
let adjacency s (graph:list<char*char>) =
    graph
    |> List.choose(fun (x,y) -> 
        if x = s then
            Some y
        elif y = s then
            Some x
        else None
        )

// 从边集合求顶点集合
let getVertices (graph:list<char*char>) =
    graph
    |> List.collect(fun(x,y)->[x;y])
    |> List.distinct

type Color = Black|White|Gray

type Vertex = {
    tag:char
    color:Color
    d:int
    pi:char
}

open System

// 初始化无向图
let initializeVertices (vertices:list<char>) =
    vertices
    |> List.map(fun c -> {
        tag = c
        color = White
        d = Int32.MaxValue
        pi = '\000'
    })

let getVertex (vertices:#seq<Vertex>) (c:char) =
    vertices
    |> Seq.find(fun cc -> cc.tag = c)

let setVertex (vertices:list<Vertex>) (v:Vertex) =
    vertices
    |> List.map(fun t -> if t.tag = v.tag then v else t)

let bfs (s:char) (graph:list<char*char>) =
    
    let rec loopAdj (vertices:list<Vertex>) (Q:Queue<char>) (u:Vertex) (adjs:list<char>) =
        match adjs with
        | [] -> vertices,Q
        | v::tail ->
            let v = getVertex vertices v
            if v.color = White then
                let vertices =
                    setVertex vertices {
                        v with
                            color = Gray
                            d = u.d+1
                            pi = u.tag
                    }
                let Q = Q |> Queue.enqueue v.tag
                loopAdj vertices Q u tail
            else loopAdj vertices Q u tail

    let rec loop (vertices:list<Vertex>) (Q:Queue<char>) =
        if Q |> Queue.isEmpty then
            vertices
        else
            let u,Q = Q |> Queue.dequeue
            let adjs = adjacency u graph
            //Console.WriteLine(Literal.stringify adjs)
            let u = getVertex vertices u
            let vertices,Q = loopAdj vertices Q u adjs
            let vertices =
                setVertex vertices {
                    u with color = Black
                }
            loop vertices Q


    let vertices =
        graph
        |> getVertices
        |> initializeVertices
        |> setVertex <| {
            tag = s
            color = Gray
            d = 0
            pi = '\000'
        }

    let Q = Queue.empty |> Queue.enqueue s
    loop vertices Q


let test() =
    let s = 's'
    let r = 'r'
    let t = 't'
    let u = 'u'
    let v = 'v'
    let w = 'w'
    let x = 'x'
    let y = 'y'
    
    // 无向图22-3
    let graph = [
        r,v
        r,s
        s,w
        t,w
        w,x
        t,x
        t,u
        u,x
        u,y
        x,y
        ]
    
    let x = bfs s graph
    Console.WriteLine(Literal.stringify x)
    ()
