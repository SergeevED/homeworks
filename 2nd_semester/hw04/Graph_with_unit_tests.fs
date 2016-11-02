module hw
open NUnit.Framework

type IGraph<'A> = 
  interface
    abstract Size : unit -> int
    abstract hasEdge : 'A -> 'A -> bool
    abstract GetVertex : unit -> 'A array
  end 

type MatrixGraph<'A when 'A: equality> (V : 'A array, M : bool[,]) =
  class
    let vertices = V
    let matrix = M
    interface IGraph<'A> with
      member this.GetVertex() = vertices
      member this.Size() = vertices.Length
      member this.hasEdge vertex1 vertex2 =
        let i = Array.findIndex (fun x -> x = vertex1) vertices
        let j = Array.findIndex (fun x -> x = vertex2) vertices
        matrix.[i,j]
  end

type ListGraph<'A when 'A: equality> (V : 'A array, L : ('A list) array) =
  class
    let vertices = V
    let list = L
    interface IGraph<'A> with
      member this.GetVertex() = vertices
      member this.Size() = vertices.Length
      member this.hasEdge vertex1 vertex2 =
        let index = Array.findIndex (fun x -> x = vertex1) vertices
        List.exists (fun x -> x = vertex2) list.[index]
  end


let GetSuccessors (graph : IGraph<'A>) vertex =
  match Array.tryFindIndex (fun x -> x = vertex) (graph.GetVertex()) with
  | None -> []
  | _ ->
    let visited = [|for i in 0 .. graph.Size() - 1 -> false|]
    let rec f (graph : IGraph<'A>) (visited : bool array) vertex =
      let mutable result = []
      let index = Array.findIndex (fun x -> x = vertex) (graph.GetVertex())
      visited.[index] <- true 
      for i in 0 .. graph.Size() - 1 do
        if not visited.[i] && (graph.hasEdge vertex (graph.GetVertex().[i]))
          then
            visited.[i] <- true
            result <- List.append result [graph.GetVertex().[i];]
            result <- List.append result (f graph visited (graph.GetVertex().[i]))        
      result
    f graph visited vertex


let GetPredecessors (graph : IGraph<'A>) vertex =
  match Array.tryFindIndex (fun x -> x = vertex) (graph.GetVertex()) with
  | None -> []
  | _ ->
    let visited = [|for i in 0 .. graph.Size() - 1 -> false|]
    let rec f (graph : IGraph<'A>) (visited : bool array) vertex =
      let mutable result = []
      let index = Array.findIndex (fun x -> x = vertex) (graph.GetVertex())
      visited.[index] <- true 
      for i in 0 .. graph.Size() - 1 do
        if not visited.[i] && (graph.hasEdge (graph.GetVertex().[i]) vertex)
          then
            visited.[i] <- true
            result <- List.append result [graph.GetVertex().[i];]
            result <- List.append result (f graph visited (graph.GetVertex().[i]))        
      result
    f graph visited vertex

[<Test>]
let MGraphGetSuccessorsNonExistentVertex() = 
  let vertices = [| 0; 1; 2; 3; 4|]
  let adjMatrix = Array2D.create 5 5 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors mGraph 5)

[<Test>]
let MGraphGetSuccessorsEmptyGraph() = 
  let vertices = [||]
  let adjMatrix = Array2D.create 0 0 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors mGraph 0)

[<Test>]
let MGraphGetSuccessorsVertexWithNoSuccessors() = 
  let vertices = [| 0; 1; 2; 3; 4|]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true; adjMatrix.[3,1] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors mGraph 0)

[<Test>]
let MGraphGetSuccessorsAllVerticesAreConnectedCyclically() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.zeroCreate<bool> 10 10
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true;
  adjMatrix.[3,4] <- true; adjMatrix.[4,5] <- true; adjMatrix.[5,6] <- true;
  adjMatrix.[6,7] <- true; adjMatrix.[7,8] <- true; adjMatrix.[8,9] <- true;
  adjMatrix.[9,0] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetSuccessors mGraph 0))

[<Test>]
let MGraphGetSuccessorsAllVerticesAreConnectedToEachOther() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.create 10 10 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetSuccessors mGraph 0))

[<Test>]
let MGraphGetSuccessorsComplicatedGraph01() = 
  let vertices = [| 0 ; 1 ; 2 ; 3 ; 4 |]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[1,3] <- true;
  adjMatrix.[3,2] <- true; adjMatrix.[3,4] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([2; 3; 4], List.sort (GetSuccessors mGraph 1))

[<Test>]
let MGraphGetSuccessorsComplicatedGraph02() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.zeroCreate<bool> 10 10
  adjMatrix.[9,2] <- true; adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true;
  adjMatrix.[3,5] <- true; adjMatrix.[3,8] <- true; adjMatrix.[5,6] <- true;
  adjMatrix.[5,7] <- true; adjMatrix.[7,2] <- true; 
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([3; 5; 6; 7; 8], List.sort (GetSuccessors mGraph 2))

[<Test>]
let MGraphGetSuccessorsComplicatedGraph03() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15|]
  let adjMatrix = Array2D.zeroCreate<bool> 16 16
  adjMatrix.[9,1] <- true; adjMatrix.[9,2] <- true; adjMatrix.[9,3] <- true;
  adjMatrix.[9,4] <- true; adjMatrix.[9,5] <- true; adjMatrix.[1,0] <- true;
  adjMatrix.[2,0] <- true; adjMatrix.[3,0] <- true; adjMatrix.[4,0] <- true;
  adjMatrix.[5,0] <- true; adjMatrix.[10,9] <- true; adjMatrix.[10,6] <- true;
  adjMatrix.[6,7] <- true; adjMatrix.[6,8] <- true; adjMatrix.[6,9] <- true;
  adjMatrix.[9,6] <- true; adjMatrix.[6,11] <- true; adjMatrix.[6,12] <- true;
  adjMatrix.[6,13] <- true; adjMatrix.[7,9] <- true; adjMatrix.[8,9] <- true;
  adjMatrix.[14,10] <- true; adjMatrix.[15,14] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 11; 12; 13], List.sort (GetSuccessors mGraph 10))


[<Test>]
let MGraphGetPredecessorsNonExistentVertex() = 
  let vertices = [| 0; 1; 2; 3; 4|]
  let adjMatrix = Array2D.create 5 5 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetPredecessors mGraph 5)

[<Test>]
let MGraphGetPredecessorsEmptyGraph() = 
  let vertices = [||]
  let adjMatrix = Array2D.create 0 0 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetPredecessors mGraph 0)

[<Test>]
let MGraphGetPredecessorsVertexWithNoPredecessors() = 
  let vertices = [| 0; 1; 2; 3; 4|]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true; adjMatrix.[3,1] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([], GetPredecessors mGraph 0)

[<Test>]
let MGraphGetPredecessorsAllVerticesAreConnectedCyclically() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.zeroCreate<bool> 10 10
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true;
  adjMatrix.[3,4] <- true; adjMatrix.[4,5] <- true; adjMatrix.[5,6] <- true;
  adjMatrix.[6,7] <- true; adjMatrix.[7,8] <- true; adjMatrix.[8,9] <- true;
  adjMatrix.[9,0] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetPredecessors mGraph 0))

[<Test>]
let MGraphGetPredecessorsAllVerticesAreConnectedToEachOther() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.create 10 10 true
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetPredecessors mGraph 0))

[<Test>]
let MGraphGetPredecessorsComplicatedGraph01() = 
  let vertices = [| 0 ; 1 ; 2 ; 3 ; 4 |]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[1,3] <- true;
  adjMatrix.[3,2] <- true; adjMatrix.[3,4] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([0; 1; 3], List.sort (GetPredecessors mGraph 2))

[<Test>]
let MGraphGetPredecessorsComplicatedGraph02() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjMatrix = Array2D.zeroCreate<bool> 10 10
  adjMatrix.[9,2] <- true; adjMatrix.[1,2] <- true; adjMatrix.[2,3] <- true;
  adjMatrix.[3,5] <- true; adjMatrix.[3,8] <- true; adjMatrix.[5,6] <- true;
  adjMatrix.[5,7] <- true; adjMatrix.[7,2] <- true; 
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 7; 9], List.sort (GetPredecessors mGraph 5))

[<Test>]
let MGraphGetPredecessorsComplicatedGraph03() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15|]
  let adjMatrix = Array2D.zeroCreate<bool> 16 16
  adjMatrix.[0,5] <- true; adjMatrix.[1,5] <- true; adjMatrix.[2,5] <- true;
  adjMatrix.[3,5] <- true; adjMatrix.[4,5] <- true; adjMatrix.[6,7] <- true;
  adjMatrix.[7,5] <- true; adjMatrix.[8,6] <- true; adjMatrix.[9,6] <- true;
  adjMatrix.[10,6] <- true; adjMatrix.[5,11] <- true; adjMatrix.[1,12] <- true;
  adjMatrix.[12,13] <- true; adjMatrix.[5,14] <- true; adjMatrix.[5,15] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  Assert.AreEqual([0; 1; 2; 3; 4; 6; 7; 8; 9; 10], List.sort (GetPredecessors mGraph 5))


[<Test>]
let LGraphGetSuccessorsNonExistentVertex() = 
  let vertices = [| 0; 1; 2; 3; 4|]
  let adjLists = [| [1]; [2]; [3]; [4]; [0] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors lGraph 5)

[<Test>]
let LGraphGetSuccessorsEmptyGraph() = 
  let vertices = [||]
  let adjLists = [||]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors lGraph 0)

[<Test>]
let LGraphGetSuccessorsVertexWithNoSuccessors() = 
  let vertices = [|0; 1; 2; 3; 4|]
  let adjLists = [|[]; [2]; [3]; [4]; [1]|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([], GetSuccessors lGraph 0)

[<Test>]
let LGraphGetSuccessorsAllVerticesAreConnectedCyclically() = 
  let vertices = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [|[1]; [2]; [3]; [4]; [5]; [6]; [7]; [8]; [9]; [0]|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetSuccessors lGraph 0))

[<Test>]
let LGraphGetSuccessorsAllVerticesAreConnectedToEachOther() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [| 
     [1;2;3;4;5;6;7;8;9]; [0;2;3;4;5;6;7;8;9]; [0;1;3;4;5;6;7;8;9]; [0;1;2;4;5;6;7;8;9];
     [0;1;2;3;5;6;7;8;9]; [0;1;2;3;4;6;7;8;9]; [0;1;2;3;4;5;7;8;9]; [0;1;2;3;4;5;6;8;9];
     [0;1;2;3;4;5;6;7;9]; [0;1;2;3;4;5;6;7;8] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetSuccessors lGraph 0))

[<Test>]
let LGraphGetSuccessorsComplicatedGraph01() = 
  let vertices = [| 0 ; 1 ; 2 ; 3 ; 4 |]
  let adjLists = [| [1]; [2;3]; []; [2;4]; [] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([2; 3; 4], List.sort (GetSuccessors lGraph 1))

[<Test>]
let LGraphGetSuccessorsComplicatedGraph02() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [| []; [2]; [3]; [5;8]; []; [6;7]; []; [2]; []; [2] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([3; 5; 6; 7; 8], List.sort (GetSuccessors lGraph 2))

[<Test>]
let LGraphGetSuccessorsComplicatedGraph03() = 
  let vertices = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15|]
  let adjLists = [| 
     []; [0]; [0]; [0]; [0]; [0]; [7;8;9;11;12;13]; []; []; [1;2;3;4;5;6]; [6;9]; [];
     []; []; [10]; [14]|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 11; 12; 13], List.sort (GetSuccessors lGraph 10))


[<Test>]
let LGraphPredecessorsNonExistentVertex() = 
 let vertices = [| 0; 1; 2; 3; 4|]
 let adjLists = [| [1]; [2]; [3]; [4]; [0] |]
 let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
 Assert.AreEqual([], GetPredecessors lGraph 5)

[<Test>]
let LGraphPredecessorsEmptyGraph() = 
  let vertices = [||]
  let adjLists = [||]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([], GetPredecessors lGraph 0)

[<Test>]
let LGraphGetPredecessorsVertexWithNoPredecessors() = 
  let vertices = [|0; 1; 2; 3; 4|]
  let adjLists = [|[]; [2]; [3]; [4]; [1]|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([], GetPredecessors lGraph 0)

[<Test>]
let LGraphGetPredecessorsAllVerticesAreConnectedCyclically() = 
  let vertices = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [|[1]; [2]; [3]; [4]; [5]; [6]; [7]; [8]; [9]; [0]|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetPredecessors lGraph 0))

[<Test>]
let LGraphGetPredecessorsAllVerticesAreConnectedToEachOther() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [| 
     [1;2;3;4;5;6;7;8;9]; [0;2;3;4;5;6;7;8;9]; [0;1;3;4;5;6;7;8;9]; [0;1;2;4;5;6;7;8;9];
     [0;1;2;3;5;6;7;8;9]; [0;1;2;3;4;6;7;8;9]; [0;1;2;3;4;5;7;8;9]; [0;1;2;3;4;5;6;8;9];
     [0;1;2;3;4;5;6;7;9]; [0;1;2;3;4;5;6;7;8] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 4; 5; 6; 7; 8; 9], List.sort (GetPredecessors lGraph 0))

[<Test>]
let LGraphGetPredecessorsComplicatedGraph01() = 
  let vertices = [| 0 ; 1 ; 2 ; 3 ; 4 |]
  let adjLists = [| [1]; [2;3]; []; [2;4]; [] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([0; 1; 3], List.sort (GetPredecessors lGraph 2))

[<Test>]
let LGraphGetPredecessorsComplicatedGraph02() = 
  let vertices = [| 0; 1; 2; 3; 4; 5; 6; 7; 8; 9|]
  let adjLists = [| []; [2]; [3]; [5;8]; []; [6;7]; []; [2]; []; [2] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([1; 2; 3; 7; 9], List.sort (GetPredecessors lGraph 5))

[<Test>]
let LGraphGetPredecessorsComplicatedGraph03() = 
  let vertices = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15|]
  let adjLists = [| 
     [5]; [5;12]; [5]; [5]; [5]; [11;14;15]; [7]; [5]; [6]; [6]; [6]; [];
     [13]; []; []; []|]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  Assert.AreEqual([0; 1; 2; 3; 4; 6; 7; 8; 9; 10], List.sort (GetPredecessors lGraph 5))



[<EntryPoint>]
let main argv =

  0