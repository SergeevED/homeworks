type IGraph<'A when 'A : equality> =
  interface
    abstract Print : List<int> -> unit
    abstract GetSuccByIndex : int -> List<'A>
    abstract GetPredByIndex : int -> List<'A>
    abstract GetSuccByValue : 'A -> List<'A>
    abstract GetPredByValue : 'A -> List<'A>
  end

type MatrixGraph<'A when 'A : equality>(V : 'A[] , M : bool[,]) =
  class
    let size = V.Length
    let vertices = V 
    let adjMatrix = M
    let isMarked = Array.zeroCreate<bool> size
    do if (size <> Array2D.length1 M) || (size <> Array2D.length2 M) then failwith "Wrong argument"
    interface IGraph<'A> with   
      member this.Print(list) = 
        List.iter (fun x -> if (x < size) then printf "%A ; " vertices.[x] else failwith "Bounds violation") list
        printf "\n"
      member this.GetSuccByIndex index = 
        if (index >= size || index < 0) 
        then 
          failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f i j list =
            isMarked.[i] <- true
            if j >= size 
              then 
                list
              else
                match adjMatrix.[i,j] , isMarked.[j] with
                | true , false -> vertices.[j] :: (f j 0 list) @ (f i (j+1) list)
                | _ , _ -> f i (j+1) list
          f index 0 []
      member this.GetPredByIndex index = 
        if (index >= size || index < 0) 
        then 
          failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f i j list =
            isMarked.[i] <- true
            if j >= size 
              then 
                list
              else
                match adjMatrix.[j,i] , isMarked.[j] with
                | true , false -> vertices.[j] :: (f j 0 list) @ (f i (j+1) list)
                | _ , _ -> f i (j+1) list
          f index 0 []
      member this.GetSuccByValue elem =
        match Array.tryFindIndex ((=)elem) vertices with 
        | Some x -> (this :> IGraph<'A>).GetSuccByIndex x
        | None -> []
      member this.GetPredByValue elem =
        match Array.tryFindIndex ((=)elem) vertices with 
        | Some x -> (this :> IGraph<'A>).GetPredByIndex x
        | None -> []
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
      member this.Print(list) = 
        List.iter (fun x -> if (x < size) then printf "%A ; " vertices.[x] else failwith "Bounds violation") list
        printf "\n"
      member this.GetSuccByIndex index = 
        if (index >= size || index < 0) 
        then 
          failwith "Wrong argument" 
        else
          for i = 0 to size - 1 do isMarked.[i] <- false
          let rec f list  =
            match list with
            | [] -> []
            | a :: list -> 
              if isMarked.[a] 
                then 
                  f list 
                else 
                  isMarked.[a] <- true
                  vertices.[a] :: (f adjList.[a]) @ (f list) 
          isMarked.[index] <- true
          f adjList.[index]
      member this.GetPredByIndex index = 
        if (index >= size || index < 0) 
          then 
            failwith "Wrong argument" 
          else
            for i = 0 to size - 1 do isMarked.[i] <- false
            let rec f i j list =
              if i >= size 
                then 
                  []
                else 
                  match List.tryFind ((=)j) adjList.[i] with
                  | Some _ ->
                    if isMarked.[i] 
                      then
                        f (i+1) j list  
                      else
                        isMarked.[i] <- true 
                        vertices.[i] :: (f 0 i []) @ (f (i+1) j list) 
                  | None -> f (i+1) j list
            isMarked.[index] <- true  
            f 0 index []
      member this.GetSuccByValue elem =
        match Array.tryFindIndex ((=)elem) vertices with 
        | Some x -> (this :> IGraph<'A>).GetSuccByIndex x
        | None -> []
      member this.GetPredByValue elem =
        match Array.tryFindIndex ((=)elem) vertices with 
        | Some x -> (this :> IGraph<'A>).GetPredByIndex x
        | None -> []
  end

type ILabeledGraph<'A when 'A : equality> =
  interface
    inherit IGraph<'A>
    abstract GetPath : 'A -> 'A -> list<'A>           //returns path from one vertex to another vertex
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
        if i >= size
          then
            list
          else
            if j >= size
              then
                f (i+1) 0 list
              else
                 if (adjMatrix.[i,j] = true) && (random.Next(100) > 100 - vertices.[j].infectedChance) 
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
  printf "Graph: 10 -> 11 -> 12 ; 11 -> 13 -> 12 ; 13 -> 14 (That`s single graph)\n"
  let vertices = [| 10 ; 11 ; 12 ; 13 ; 14 |]
  let adjMatrix = Array2D.zeroCreate<bool> 5 5
  adjMatrix.[0,1] <- true; adjMatrix.[1,2] <- true; adjMatrix.[1,3] <- true; adjMatrix.[3,2] <- true; adjMatrix.[3,4] <- true;
  let mGraph = new MatrixGraph<int>(vertices, adjMatrix)
  printf "Graph with adjacency matrix : "
  (mGraph :> IGraph<int>).Print [ 0 ; 1 ; 2 ; 3 ; 4 ]
  printf "%A\n" adjMatrix
  printf "Successors of '11' : "
  printf "%A\n" ((mGraph :> IGraph<int>).GetSuccByValue 11)
  printf "Predecessors of '14' : "
  printf "%A\n" ((mGraph :> IGraph<int>).GetPredByValue 14)

  let adjLists = [| [1] ; [2;3] ; [] ; [2;4] ; [] |]
  let lGraph = new ListGraph<int>(vertices, adjLists)
  printf "Graph with adjacency lists : "
  (lGraph :> IGraph<int>).Print [ 0 ; 1 ; 2 ; 3 ; 4 ]
  printf "%A\n" adjLists
  printf "Successors of '11' : "
  printf "%A\n" ((lGraph :> IGraph<int>).GetSuccByValue 11)
  printf "Predecessors of '14' : "
  printf "%A\n" ((lGraph :> IGraph<int>).GetPredByValue 14)

  printf "\n"
  printf "LAN\n"
  let lan = [|
               { number = 1; os = "Windows 98"; infectedChance = 20; isInfected = false} 
               { number = 2; os = "Linux";      infectedChance = 15; isInfected = false} 
               { number = 3; os = "FreeBSD";    infectedChance = 10; isInfected = false} 
               { number = 4; os = "Windows 95"; infectedChance = 30; isInfected = true} 
               { number = 5; os = "MenuetOS";   infectedChance = 5;  isInfected = false} 
            |]
  let m3 = Array2D.zeroCreate<bool> 5 5
  for i = 0 to 4 do for j = 0 to 4 do Array2D.set m3 i j true
  let J = new LAN(lan, m3)
  Array.iter (fun x -> printf "%A\n" x) lan 
  (J :> IGraph<Computer>).Print([1])
  for i = 1 to 5 do 
    printf "Check %d\n" i 
    J.Show()
    J.TryInfect()
    
  0