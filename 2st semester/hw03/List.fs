type IList<'A when 'A : equality> =
  interface
    abstract InsertHead : 'A -> unit
    abstract InsertTail : 'A -> unit
    abstract InsertAt   : 'A -> int -> unit
    abstract RemoveHead : unit -> unit
    abstract RemoveTail : unit -> unit
    abstract RemoveNode   : 'A -> unit
    abstract TryFind    : ('A -> bool) -> Option<'A>
    abstract Concat     : IList<'A> -> unit
  end

type List<'A> = Void | Node of 'A * List<'A>

type AbstractList<'A when 'A : equality>(L : List<'A>) =
  class
    let mutable thisList = L

    member this.GetList() = thisList

    member this.PrintList() = 
      let rec f list =
        match list with
        | Void -> ()
        | Node(a, next) -> 
          printf "%A " a
          f next
      f thisList
      printf "\n"

    interface IList<'A> with

      member this.InsertHead(elem) = 
        thisList <- Node(elem, thisList)

      member this.InsertTail(elem) =
        let rec f list =
          match list with
          | Void -> Node(elem, Void)
          | Node(a, next) -> Node(a, f next)
        thisList <- f thisList

      member this.InsertAt elem index =   //if index > length of list then inserts element at tail
        if index < 1 
          then 
            failwith "Wrong argument" 
          else 
            let rec f list i =
              match list with
              | Void -> Node(elem, Void)
              | Node(a, next) -> if i = index then Node(elem, list) else Node(a, f next (i+1) )
            thisList <- (f thisList 1)
        
      member this.RemoveHead() =
        match thisList with
        | Void -> ()
        | Node(a, next) -> thisList <- next

      member this.RemoveTail() =
        let rec f list =
          match list with
          | Void -> Void
          | Node(_, Void) -> Void
          | Node(a, next) -> Node(a, f next)
        thisList <- f thisList

      member this.RemoveNode(elem) =
        let rec f list =
          match list with
          | Void -> Void
          | Node(a, next) -> if a = elem then next else Node(a, f next)
        thisList <- f thisList

      member this.TryFind(f1) =
        let rec f list =
          match list with
          | Void -> None
          | Node(a, next) ->
            if (f1 a) then Some(a) else (f next)
        f thisList

      member this.Concat(anotherList) =
        let rec f list =
          match list with
          | Void -> (anotherList :?> AbstractList<'A>).GetList()
          | Node(a, next) -> Node(a, f next)
        thisList <- f thisList
  end


type ArrayList<'A when 'A : equality>(L : 'A[]) =
  class
    let mutable thisList = L

    member this.GetList() = thisList

    member this.PrintList() = 
      Array.iter (fun x -> printf "%A " x) thisList
      printf "\n"

    interface IList<'A> with

      member this.InsertHead elem = thisList <- Array.append [|elem|] thisList 

      member this.InsertTail elem = thisList <- Array.append thisList [|elem|]

      member this.InsertAt elem index =           //if index > length of list then inserts element at tail
        if index < 1 
          then 
            failwith "Wrong argument" 
          else 
            let length = Array.length thisList
            if index = 1 then (this :> IList<'A>).InsertHead elem
            elif index >= length then (this :> IList<'A>).InsertTail elem
            else
              let temp = Array.zeroCreate (length+1)
              Array.blit thisList 0 temp 0 (index-1)
              temp.[index-1] <- elem
              Array.blit thisList (index-1) temp (index) (length-index+1)
              thisList <- temp

      member this.RemoveHead() =
        match Array.length thisList with
        | 0 -> ()
        | 1 -> thisList <- [||] 
        | n ->
          let temp = Array.zeroCreate (n-1)
          Array.blit thisList 1 temp 0 (n-1)
          thisList <- temp

      member this.RemoveTail() =
        match Array.length thisList with
        | 0 -> ()
        | 1 -> thisList <- [||] 
        | n ->
          let temp = Array.zeroCreate (n-1)
          Array.blit thisList 0 temp 0 (n-1)
          thisList <- temp

      member this.RemoveNode(elem) =
        match Array.length thisList with
        | 0 -> ()
        | 1 -> if thisList.[0] = elem then thisList <- [||] else () 
        | length ->
          match Array.tryFindIndex ((=)elem) thisList with
          | None -> ()
          | Some i ->
            if i = 0 then 
              (this :> IList<'A>).RemoveHead()
            elif i = (length-1) then 
              (this :> IList<'A>).RemoveTail()
            else 
              let temp = Array.zeroCreate (length-1)
              Array.blit thisList 0 temp 0 i
              Array.blit thisList (i+1) temp i (length-i-1)
              thisList <- temp
    
      member this.TryFind(f1) = Array.tryFind f1 thisList

      member this.Concat(anotherList) = thisList <- Array.append thisList ( (anotherList :?> ArrayList<'A>).GetList() )
  end

[<EntryPoint>]
let main args =
  let list1 = Node(1, Node(3, Node(4, Node(5, Void) ) ) )
  let abList = new AbstractList<int>(list1)
  printf "Abstract list: " 
  abList.PrintList()
  printf "Insert 0 at head: "
  (abList :> IList<int>).InsertHead 0
  abList.PrintList()
  printf "Insert 6 at tail: "
  (abList :> IList<int>).InsertTail 6
  abList.PrintList()
  printf "Insert 2 at third node: "
  (abList :> IList<int>).InsertAt 2 3
  abList.PrintList()
  printf "Remove node from head: "
  (abList :> IList<int>).RemoveHead()
  abList.PrintList()
  printf "Remove node from tail: "
  (abList :> IList<int>).RemoveTail()
  abList.PrintList()
  printf "Remove third node : "
  (abList :> IList<int>).RemoveNode 3
  abList.PrintList()
  printf "Find node divisible by 5 : "
  let x = (abList :> IList<int>).TryFind (fun x -> x % 5 = 0 )
  match x with
  | None   -> printf "Not found\n"
  | Some x -> printf "%d\n" x
  let list2 = Node(10, Node(11, Void) )
  let anList = new AbstractList<int>(list2)
  printf "Another abstract list: " 
  anList.PrintList()
  printf "Concat lists: "
  (abList :> IList<int>).Concat anList
  abList.PrintList()
  printf "\n"

  let array1 = [|1 ; 3 ; 4 ; 5|]
  let arList = new ArrayList<int>(array1)
  printf "Array list: " 
  arList.PrintList()
  printf "Insert 0 at head: "
  (arList :> IList<int>).InsertHead 0
  arList.PrintList()
  printf "Insert 6 at tail: "
  (arList :> IList<int>).InsertTail 6
  arList.PrintList()
  printf "Insert 2 at third node: "
  (arList :> IList<int>).InsertAt 2 3
  arList.PrintList()
  printf "Remove node from head: "
  (arList :> IList<int>).RemoveHead()
  arList.PrintList()
  printf "Remove node from tail: "
  (arList :> IList<int>).RemoveTail()
  arList.PrintList()
  printf "Remove third node : "
  (arList :> IList<int>).RemoveNode 3
  arList.PrintList()
  printf "Find node divisible by 5 : "
  let x = (arList :> IList<int>).TryFind (fun x -> x % 5 = 0 )
  match x with
  | None   -> printf "Not found\n"
  | Some x -> printf "%d\n" x
  let array2 = [|10 ; 11|]
  let arList2 = new ArrayList<int>(array2)
  printf "Another abstract list: " 
  arList2.PrintList()
  printf "Concat lists: "
  (arList :> IList<int>).Concat arList2
  arList.PrintList()

  0