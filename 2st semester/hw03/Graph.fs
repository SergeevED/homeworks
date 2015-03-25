type IGraph<'A when 'A : equality> =
  interface
    abstract Print : unit -> unit
    abstract GetSuccessors : int -> List<'A>
    abstract GetPredecessors : int -> List<'A>
  end

type MatrixGraph<'A when 'A : equality>(V : 'A[] , M : bool[,]) =
  class
    let size = V.Length
    let vertices = V 
    let adjMatrix = M
    let isMarked = Array.zeroCreate<bool> size
    do if (size <> Array2D.length1 M) || (size <> Array2D.length2 M) then failwith "Wrong argument"
    interface IGraph<'A> with   
      member this.Print() = 
        Array.iter (fun x -> printf "%A " x) vertices
        printf "\n"
      member this.GetSuccessors index = 
        if (index >= size || index < 0) 
        then failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f i j list =
            isMarked.[i] <- true
            if j >= size 
              then list
              else
                match adjMatrix.[i,j] , isMarked.[j] with
                | true , false -> vertices.[j] :: (f j 0 list) @ (f i (j+1) list)
                | _ , _ -> f i (j+1) list
          f index 0 []
      member this.GetPredecessors index = 
        if (index >= size || index < 0) 
        then failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f i j list =
            isMarked.[i] <- true
            if j >= size 
              then list
              else
                match adjMatrix.[j,i] , isMarked.[j] with
                | true , false -> vertices.[j] :: (f j 0 list) @ (f i (j+1) list)
                | _ , _ -> f i (j+1) list
          f index 0 []
  end

type ListGraph<'A when 'A : equality>(V : 'A[] , L : list<int>[]) =
  class
    let size = V.Length
    let vertices = V 
    let adjList = L
    let isMarked = Array.zeroCreate<bool> size
    do if (size <> Array.length L) then failwith "Wrong argument"
    do for list in L do for x in list do if (x < 0) || (x > size) then failwith "Wrong argument"
    interface IGraph<'A> with
      member this.Print() = 
        Array.iter (fun x -> printf "%A " x) vertices
        printf "\n"
      member this.GetSuccessors index = 
        if (index >= size || index < 0) 
        then failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f list  =
            match list with
            | [] -> []
            | a :: list when isMarked.[a] -> f list
            | a :: list ->
              isMarked.[a] <- true
              vertices.[a] :: (f adjList.[a]) @ (f list)
          isMarked.[index] <- true
          f adjList.[index]
      member this.GetPredecessors index = 
        if (index >= size || index < 0) 
          then failwith "Wrong argument" 
          else
            for i = 0 to size - 1 do isMarked.[i] <- false
            let rec f i j list =
              if i >= size 
                then []
                else 
                  match List.tryFind ((=)j) adjList.[i] with
                  | Some _ when isMarked.[i] -> f (i+1) j list 
                  | Some _  -> 
                    isMarked.[i] <- true 
                    vertices.[i] :: (f 0 i []) @ (f (i+1) j list) 
                  | None -> f (i+1) j list
            isMarked.[index] <- true  
            f 0 index []
  end

type IMarkedGraph<'A, 'B when 'A : equality> =
  interface
    inherit IGraph<'A>
    abstract Mark     : int -> 'B -> unit
    abstract GetMark  : int -> 'B
    abstract Unmark   : int -> unit
  end

type Computer = {
  number : int; 
  os : string;
  infectedChance : int;
  mutable isInfected : bool;
  }

type LAN(V : Computer[] , M : bool[,]) =
  class
    inherit MatrixGraph<Computer>(V , M)
    let size = V.Length
    let vertices = V 
    let adjMatrix = M
    let isMarked = Array.zeroCreate<bool> size
    member this.TryInfect() =  
      let random = System.Random()
      let rec f i j list =
        match i, j with
        | i, _ when i >= size -> list
        | _, j when j >= size -> f (i+1) 0 list
        | i, j ->
          if (adjMatrix.[i,j] = true) && (vertices.[i].isInfected) && (random.Next(100) > 100 - vertices.[j].infectedChance) 
            then j :: (f i (j+1) list) 
            else f i (j+1) list
      let infected = f 0 0 []
      List.iter (fun x -> vertices.[x].isInfected <- true) infected 
    member this.Show() =
      for i = 0 to size - 1 do 
        printf "Computer %d : " vertices.[i].number
        if vertices.[i].isInfected then printf "infected\n" else printf "not infected\n" 
    end

[<EntryPoint>]
let main args =
  printf "Graph: 0 -> 1 -> 2 ; 1 -> 3 -> 2 ; 3 -> 4 (That`s single graph)\n"
  let vertices = [| 0 ; 1 ; 2 ; 3 ; 4 |]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[1,3] <- true; adjMatrix.[3,2] <- true; adjMatrix.[3,4] <- true;
  let mGraph = (new MatrixGraph<int>(vertices, adjMatrix)) :> IGraph<int>
  printf "Graph with adjacency matrix : "
  mGraph.Print()
  printf "%A\n" adjMatrix
  printf "Successors of '1' : "
  printf "%A\n" (mGraph.GetSuccessors 1)
  printf "Predecessors of '4' : "
  printf "%A\n" (mGraph.GetPredecessors 4)

  let adjLists = [| [1] ; [2;3] ; [] ; [2;4] ; [] |]
  let lGraph = (new ListGraph<int>(vertices, adjLists)) :> IGraph<int>
  printf "Graph with adjacency lists : "
  lGraph.Print()
  printf "%A\n" adjLists
  printf "Successors of '1' : "
  printf "%A\n" (lGraph.GetSuccessors 1)
  printf "Predecessors of '4' : "
  printf "%A\n" (lGraph.GetPredecessors 4)

  printf "\n"
  printf "LAN\n"
  let lan = [|
               { number = 1; os = "Windows 98"; infectedChance = 25; isInfected = false} 
               { number = 2; os = "Linux";      infectedChance = 20; isInfected = false} 
               { number = 3; os = "FreeBSD";    infectedChance = 15; isInfected = false} 
               { number = 4; os = "Windows 95"; infectedChance = 30; isInfected = true} 
               { number = 5; os = "MenuetOS";   infectedChance = 10;  isInfected = false} 
            |]
  let m3 = Array2D.zeroCreate<bool> 5 5
  m3.[0,1] <- true; m3.[0,3] <- true; m3.[1,0] <- true; m3.[1,4] <- true; m3.[4,1] <- true;
  m3.[4,3] <- true; m3.[3,0] <- true; m3.[3,4] <- true; m3.[3,2] <- true; m3.[2,3] <- true;
  printf "LAN: 3 <-> 4 <-> 1 <-> 2 <-> 5 <-> 4\n"
  let J = new LAN(lan, m3)
  Array.iter (fun x -> printf "%A\n" x) lan 
  for i = 1 to 5 do 
    printf "Check %d\n" i 
    J.Show()
    J.TryInfect()
    
  0