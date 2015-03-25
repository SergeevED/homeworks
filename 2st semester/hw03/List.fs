type IList<'A when 'A : equality> =
  interface
    abstract Length     : unit -> int
    abstract Pop        : unit -> Option<'A>
    abstract PrintList  : unit -> unit
    abstract InsertHead : 'A -> unit
    abstract InsertTail : 'A -> unit
    abstract InsertAt   : 'A -> int -> bool
    abstract RemoveHead : unit -> unit
    abstract RemoveTail : unit -> unit
    abstract RemoveAt   : int -> bool
    abstract TryFind    : ('A -> bool) -> Option<'A>
    abstract Concat     : IList<'A> -> unit
  end

type List<'A> = Void | Node of 'A * List<'A>

type ATDList<'A when 'A : equality>(L : List<'A>) =
  class
    let mutable thisList = L

    interface IList<'A> with
    
      member this.Length() = 
        let rec f list length =
          match list with
          | Void -> length
          | Node(_, next) -> f next (length+1)
        f thisList 0

      member this.Pop() =
        match thisList with
        | Void -> None
        | Node(a, _) -> 
          (this :> IList<'A>).RemoveHead()
          Some a

      member this.PrintList() = 
        let rec f list =
          match list with
          | Void -> ()
          | Node(a, next) -> 
            printf "%A " a
            f next
        f thisList
        printf "\n"

      member this.InsertHead elem = 
        thisList <- Node(elem, thisList)

      member this.InsertTail elem  =
        let rec f list =
          match list with
          | Void -> Node(elem, Void)
          | Node(a, next) -> Node(a, f next)
        thisList <- f thisList

      member this.InsertAt elem index = 
        if (index < 1) || ( index > (this :> IList<'A>).Length() + 1 ) then false 
          else 
            let rec f list i =
              match list with
              | Void -> Node(elem, Void)
              | Node(a, next) -> if i = index then Node(elem, list) else Node(a, f next (i+1) )
            thisList <- (f thisList 1)
            true
        
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

      member this.RemoveAt index =
        if (index < 1) || ( index > (this :> IList<'A>).Length() ) then false       
          else 
            let rec f list i =
              match list, i with
              | Void, _ -> Void
              | Node(_, next), i when (i = index) -> next
              | Node(a, next), i -> Node(a, f next (i+1))
            thisList <- f thisList 1
            true

      member this.TryFind f1 =
        let rec f list =
          match list with
          | Void -> None
          | Node(a, next) ->
            if (f1 a) then Some(a) else (f next)
        f thisList

      member this.Concat anotherList =
        let rec f list = 
          match list with   
          | Node(x, next) -> Node(x, f next)
          | Void ->
            match anotherList.Pop() with
            | None -> Void
            | Some a -> Node(a, f list)
        thisList <- f thisList
           
  end


type ArrayList<'A when 'A : equality>(L : 'A[]) =
  class
    let mutable thisList = L

    interface IList<'A> with

      member this.Length() = Array.length thisList

      member this.Pop() =
        match thisList with
        | [||] -> None
        | _ -> 
          let temp = thisList.[0]
          (this :> IList<'A>).RemoveHead()
          Some temp

      member this.PrintList() = 
        Array.iter (fun x -> printf "%A " x) thisList
        printf "\n"

      member this.InsertHead elem = thisList <- Array.append [|elem|] thisList 

      member this.InsertTail elem = thisList <- Array.append thisList [|elem|]

      member this.InsertAt elem index =
        let length = Array.length thisList
        if (index < 1) || (index > length + 1) then false          
          else 
            if index = 1 then 
              (this :> IList<'A>).InsertHead elem
              true
            elif index = length + 1 then 
              (this :> IList<'A>).InsertTail elem
              true
            else
              let temp = Array.zeroCreate (length+1)
              Array.blit thisList 0 temp 0 (index-1)
              temp.[index-1] <- elem
              Array.blit thisList (index-1) temp (index) (length-index+1)
              thisList <- temp
              true

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

      member this.RemoveAt index =
        let length = Array.length thisList
        if (index < 1) || (index >= length) then false
        elif index = 1 then
          (this :> IList<'A>).RemoveHead()
          true
        elif index = length then
          (this :> IList<'A>).RemoveTail()
          true
        else
          let temp = Array.zeroCreate (length-1)
          Array.blit thisList 0 temp 0 (index-1)
          Array.blit thisList index temp (index-1) (length-index)
          thisList <- temp
          true
    
      member this.TryFind f1 = Array.tryFind f1 thisList

      member this.Concat anotherList = 
        let rec f (anotherList : IList<'A>) =
          match anotherList.Pop() with
          | None -> ()
          | Some a -> 
            thisList <- Array.append thisList [|a|]
            f anotherList
        f anotherList
  end

[<EntryPoint>]
let main args =
  let list1 = Node(1, Node(3, Node(4, Node(5, Void) ) ) )
  let abList = new ATDList<int>(list1) :> IList<int>
  printf "ATD list: " 
  abList.PrintList()
  printf "Insert 0 at head: "
  abList.InsertHead 0
  abList.PrintList()
  printf "Insert 6 at tail: "
  abList.InsertTail 6
  abList.PrintList()
  printf "Insert 2 at third node: "
  ignore( (abList.InsertAt 2 3) )
  abList.PrintList()
  printf "Remove node from head: "
  abList.RemoveHead()
  abList.PrintList()
  printf "Remove node from tail: "
  abList.RemoveTail()
  abList.PrintList()
  printf "Remove third node : "
  ignore( (abList.RemoveAt 3) )
  abList.PrintList()
  printf "Find node divisible by 5 : "
  let x = abList.TryFind (fun x -> x % 5 = 0 )
  match x with
  | None   -> printf "Not found\n"
  | Some x -> printf "%d\n" x
  let list2 = Node(10, Node(11, Void) )
  let anList = new ATDList<int>(list2) :> IList<int>
  printf "Another ATD list: " 
  anList.PrintList()
  printf "Concat lists: "
  abList.Concat anList
  abList.PrintList()
  printf "\n"

  let array1 = [|1 ; 3 ; 4 ; 5|]
  let arList = new ArrayList<int>(array1) :> IList<int>
  printf "Array list: " 
  arList.PrintList()
  printf "Insert 0 at head: "
  arList.InsertHead 0
  arList.PrintList()
  printf "Insert 6 at tail: "
  arList.InsertTail 6
  arList.PrintList()
  printf "Insert 2 at third node: "
  ignore(arList.InsertAt 2 3)
  arList.PrintList()
  printf "Remove node from head: "
  arList.RemoveHead()
  arList.PrintList()
  printf "Remove node from tail: "
  arList.RemoveTail()
  arList.PrintList()
  printf "Remove third node : "
  ignore( arList.RemoveAt 3 ) 
  arList.PrintList()
  printf "Find node divisible by 5 : "
  let x = arList.TryFind (fun x -> x % 5 = 0 )
  match x with
  | None   -> printf "Not found\n"
  | Some x -> printf "%d\n" x
  let array2 = [|10 ; 11|]
  let arList2 = new ArrayList<int>(array2) :> IList<int>
  printf "Another array list: " 
  arList2.PrintList()
  printf "Concat lists: "
  arList.Concat arList2
  arList.PrintList()

  0