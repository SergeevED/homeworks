module hw
open NUnit.Framework

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
        if (index < 1) || (index > length) then false
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


let listToArr mList = 
  let f() = 
    match (mList :> IList<'A>).Pop() with
    | None -> 
      failwith "Wrong result in testing"
    | Some x -> x
  let length = mList.Length()
  Array.map (fun x -> f() ) [|1..length|]


[<Test>]
let ArrayListPopEmptyList() = 
  let array = [||]
  let mList = new ArrayList<obj>(array) :> IList<obj>
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ArrayListInsertHeadEmptyList() = 
  let array = [||]
  let mList = new ArrayList<int>(array) :> IList<int>
  mList.InsertHead 1
  Assert.AreEqual(Some 1, mList.Pop())

[<TestCase([|1; 2; 3; 4; 5|], 100, Result = [|100; 1; 2; 3; 4; 5|])>]
[<TestCase([|'a'; 'b'; 'c'|], 'x', Result = [|'x'; 'a'; 'b'; 'c'|])>]
[<TestCase([| false; false; false |], true, Result = [| true; false; false; false |])>]
let ArrayListInsertHead array x =
  let mList = new ArrayList<'A>(array) :> IList<'A>
  mList.InsertHead x
  listToArr mList 

[<Test>]
let ArrayListInsertTailEmptyList() = 
  let array = [||]
  let mList = new ArrayList<int>(array) :> IList<int>
  mList.InsertTail 1
  Assert.AreEqual([|1|], listToArr mList)

[<TestCase([|1; 2; 3; 4; 5|], 100, Result = [|1; 2; 3; 4; 5; 100|])>]
[<TestCase([|'a'; 'b'; 'c'|], 'x', Result = [|'a'; 'b'; 'c'; 'x'|])>]
[<TestCase([| false; false; false |], true, Result = [| false; false; false; true|])>]
let ArrayListInsertTail array x =
  let mList = new ArrayList<'A>(array) :> IList<'A>
  mList.InsertTail x
  listToArr mList

[<Test>]
let ArrayListInsertAtEmptyList() = 
  let array = [||]
  let mList = new ArrayList<int>(array) :> IList<int>
  let b = mList.InsertAt 9 1
  Assert.AreEqual( (Some 9, true) , (mList.Pop(), b) )

[<TestCase([|1; 2; 3; 4; 5|], 100, -1, Result = false)>]
[<TestCase([|1; 2; 3; 4; 5|], 100, 0, Result = false)>]
[<TestCase([|'a'; 'b'; 'c'|], 'x', 5, Result = false)>]
let ArrayListInsertAtWrongIndex array x index = 
  let mList = new ArrayList<'A>(array) :> IList<'A>
  mList.InsertAt x index 
  
[<TestCase([|1; 3|], 2, 2, Result = [|1; 2; 3|])>]
[<TestCase([|1; 2|], 3, 3, Result = [|1; 2; 3|])>]
[<TestCase([|2; 3|], 1, 1, Result = [|1; 2; 3|])>]
[<TestCase([|'a'; 'c'|], 'b', 2, Result = [|'a'; 'b'; 'c'|])>]
let ArrayListInsertAt array x index = 
  let mList = new ArrayList<'A>(array) :> IList<'A>
  let b = mList.InsertAt x index
  listToArr mList 
 
[<Test>]
let ArrayListInsertLongList() = 
  let array = [|
     1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
     1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
     1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
     1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
  |]
  let mList = new ArrayList<int>(array) :> IList<int>
  let b = mList.InsertAt 100 21
  let ans = [|
    1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;100
    1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
    1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
    1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20;
  |]
  Assert.AreEqual( ans , listToArr mList )

[<Test>]
let ArrayListRemoveHeadEmptyList() = 
  let array = [||]
  let mList = new ArrayList<int>(array) :> IList<int>
  mList.RemoveHead()
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ArrayListRemoveHeadList() = 
  let mList = new ArrayList<int>([|1;2;3;4;5;6;7;8;9;0|]) :> IList<int>
  mList.RemoveHead()
  Assert.AreEqual([|2;3;4;5;6;7;8;9;0|], listToArr mList)

[<Test>]
let ArrayListRemoveTailEmptyList() = 
  let array = [||]
  let mList = new ArrayList<obj>(array) :> IList<obj>
  mList.RemoveTail()
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ArrayListRemoveTailList() = 
  let mList = new ArrayList<int>([|1;2;3;4;5;6;7;8;9;0|]) :> IList<int>
  mList.RemoveTail()
  Assert.AreEqual([|1;2;3;4;5;6;7;8;9|], listToArr mList)

[<Test>]
let ArrayListRemoveAtEmptyList() = 
  let array = [||]
  let mList = new ArrayList<int>(array) :> IList<int>
  let b = mList.RemoveAt 1
  Assert.AreEqual(false, b )

[<Test>]
let ArrayListRemoveAtWrongIndex() = 
  let mList = new ArrayList<int>([|1;2;3|]) :> IList<int>
  let b = mList.RemoveAt 4 
  Assert.AreEqual(false, b)

[<Test>]
let ArrayListRemoveAtList() = 
  let mList = new ArrayList<char>([|'a';'b';'c';'d';'e';'f'|]) :> IList<char>
  let b1 = mList.RemoveAt 1
  let b2 = mList.RemoveAt 5
  let b3 = mList.RemoveAt 3
  Assert.AreEqual(true, b1)
  Assert.AreEqual(true, b2)
  Assert.AreEqual(true ,b3)
  Assert.AreEqual([|'b';'c';'e'|] , listToArr mList)

[<Test>]
let ArrayListFindEmptyList() = 
  let array = [||]
  let mList = new ArrayList<obj>(array) :> IList<obj>
  let ans = mList.TryFind (fun x -> true) 
  Assert.AreEqual(None, ans)

[<Test>]
let ArrayListFindNothing() = 
  let array = [|1; 2; 3; 4; 5|]
  let mList = new ArrayList<int>(array) :> IList<int>
  let ans = mList.TryFind (fun x -> if x > 10 then true else false) 
  Assert.AreEqual(None, ans)

[<Test>]
let ArrayListTryFind01() =
  let array = [|1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20|]
  let mList = new ArrayList<int>(array) :> IList<int>
  let ans = mList.TryFind (fun x -> if (x % 2) = 0 then true else false)
  Assert.AreEqual(Some 2, ans)

[<Test>]
let ArrayListConcatEmptyLists() =
  let mList1 = new ArrayList<obj>([||]) :> IList<obj>
  let mList2 = new ArrayList<obj>([||]) :> IList<obj>
  mList1.Concat mList2
  Assert.AreEqual(None, mList1.Pop())

[<Test>]
let ArrayListConcatLists() =
  let mList1 = new ArrayList<char>([|'a';'b'|]) :> IList<char>
  let mList2 = new ArrayList<char>([|'c';'d'|]) :> IList<char>
  mList1.Concat mList2
  Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

[<Test>]
let ArrayConcatWithATDList() =
  let mList1 = new ArrayList<char>( [|'a';'b'|] ) :> IList<char>
  let mList2 = new ATDList<char>( Node('c', Node('d', Void)) ) :> IList<char>
  mList1.Concat mList2
  Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

[<Test>]
let ATDListPopEmptyList() = 
  let list = Void 
  let mList = new ATDList<obj>(list) :> IList<obj>
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ATDListInsertHeadEmptyList() = 
  let list = Void
  let mList = new ATDList<obj>(list) :> IList<obj>
  mList.InsertHead 1
  Assert.AreEqual([|1|], listToArr mList)

[<Test>]
let ATDListInsertHead() =
  let list = Node(1, Node(2, Node(3, Void)))
  let mList = new ATDList<int>(list) :> IList<int>
  mList.InsertHead 0
  Assert.AreEqual([|0; 1; 2; 3|], listToArr mList)

[<Test>]
let ATDListInsertTailEmptyList() = 
  let list = Void
  let mList = new ATDList<int>(list) :> IList<int>
  mList.InsertTail 1
  Assert.AreEqual([|1|], listToArr mList)

[<Test>]
let ATDListInsertTail() =
  let list = Node('a', Node('b', Node('c', Void)))
  let mList = new ATDList<char>(list) :> IList<char>
  mList.InsertTail 'z'
  Assert.AreEqual([|'a';'b';'c';'z'|], listToArr mList)

[<Test>]
let ATDListInsertAtEmptyList() = 
  let list = Void
  let mList = new ATDList<int>(list) :> IList<int>
  let b = mList.InsertAt 9 1
  Assert.AreEqual( (Some 9, true) , (mList.Pop(), b) )

[<Test>]
let ATDListInsertAtWrongIndex() = 
  let list = Node(1, Node(2, Node(3, Void)))
  let mList = new ATDList<int>(list) :> IList<int>
  let b = mList.InsertAt 5 -1
  Assert.AreEqual( false, b )
  
[<Test>]
let ATDListInsertAt() =
  let list = Node('b', Node('c', Node('d', Void)))
  let mList = new ATDList<char>(list) :> IList<char>
  let b1 = mList.InsertAt 'a' 1
  let b2 = mList.InsertAt 'e' 5
  let b3 = mList.InsertAt 'z' 3
  Assert.AreEqual( (true, true, true), (b1,b2,b3))
  Assert.AreEqual([|'a';'b';'z';'c';'d';'e'|], listToArr mList)

[<Test>]
let ATDListRemoveHeadEmptyList() = 
  let list = Void
  let mList = new ATDList<obj>(list) :> IList<obj>
  mList.RemoveHead()
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ATDListRemoveHeadList() = 
  let list = Node(1, Node(2, Node(3, Void))) 
  let mList = new ATDList<int>(list) :> IList<int>
  mList.RemoveHead()
  Assert.AreEqual([|2;3|], listToArr mList)

[<Test>]
let ATDListRemoveTailEmptyList() = 
  let list = Void
  let mList = new ATDList<obj>(list) :> IList<obj>
  mList.RemoveTail()
  Assert.AreEqual(None, mList.Pop())

[<Test>]
let ATDListRemoveTail() = 
  let list = Node(1, Node(2, Node(3, Void)))
  let mList = new ATDList<int>(list) :> IList<int>
  mList.RemoveTail()
  Assert.AreEqual([|1;2|], listToArr mList)

[<Test>]
let ATDListRemoveAtEmptyList() = 
  let list = Void
  let mList = new ATDList<obj>(list) :> IList<obj>
  let b = mList.RemoveAt 1
  Assert.AreEqual(false, b)

[<Test>]
let ATDListRemoveAtWrongIndex() = 
  let list = Node(1, Node(2, Node(3, Node(4, Void))))
  let mList = new ATDList<int>(list) :> IList<int>
  let b = mList.RemoveAt 5 
  Assert.AreEqual(false, b)
  Assert.AreEqual([|1;2;3;4|], listToArr mList)

[<Test>]
let ATDListRemoveAt() = 
  let list = Node('a', Node('b', Node('c', Node('d', Node('e', Node('f', Void))))))
  let mList = new ATDList<char>(list) :> IList<char>
  let b1 = mList.RemoveAt 1
  let b2 = mList.RemoveAt 5
  let b3 = mList.RemoveAt 3
  Assert.AreEqual(true, b1)
  Assert.AreEqual(true, b2)
  Assert.AreEqual(true ,b3)
  Assert.AreEqual([|'b';'c';'e'|] , listToArr mList)

[<Test>]
let ATDListFindEmptyList() = 
  let list = Void
  let mList = new ATDList<obj>(list) :> IList<obj>
  let ans = mList.TryFind (fun x -> true) 
  Assert.AreEqual(None, ans)

[<Test>]
let ATDListFindNothing() = 
  let list = Node(1, Node(2, Node(3, Node(4, Void))))
  let mList = new ATDList<int>(list) :> IList<int>
  let ans = mList.TryFind (fun x -> if x > 10 then true else false) 
  Assert.AreEqual(None, ans)

[<Test>]
let ATDListTryFind01() =
  let list = Node(1, Node(2, Node(3, Node(4, Void))))
  let mList = new ATDList<int>(list) :> IList<int>
  let ans = mList.TryFind (fun x -> if (x % 2) = 0 then true else false)
  Assert.AreEqual(Some 2, ans)

[<Test>]
let ATDListConcatEmptyLists() =
  let mList1 = new ATDList<obj>(Void) :> IList<obj>
  let mList2 = new ATDList<obj>(Void) :> IList<obj>
  mList1.Concat mList2
  Assert.AreEqual(None, mList1.Pop())

[<Test>]
let ATDListConcatLists() =
  let mList1 = new ATDList<char>( Node('a', Node('b', Void)) ) :> IList<char>
  let mList2 = new ATDList<char>( Node('c', Node('d', Void)) ) :> IList<char>
  mList1.Concat mList2
  Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

[<Test>]
let ATDConcatWithArrayList() =
  let mList1 = new ATDList<char>( Node('a', Node('b', Void)) ) :> IList<char>
  let mList2 = new ArrayList<char>( [|'c';'d'|] ) :> IList<char>
  mList1.Concat mList2
  Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)



[<EntryPoint>]
let main argv =

  0