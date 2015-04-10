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



[<TestFixture>]
type TestArrayList () =

  let listToArr mList = 
    let f() = 
      match (mList :> IList<'A>).Pop() with
      | None -> 
        failwith "Wrong result in testing"
      | Some x -> x
    let length = mList.Length()
    Array.map (fun x -> f() ) [|1..length|]

  [<Test>]
   member this.ArrayListPopEmptyList() = 
    let array = [||]
    let mList = new ArrayList<obj>(array) :> IList<obj>
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ArrayListInsertHeadEmptyList() = 
    let array = [||]
    let mList = new ArrayList<int>(array) :> IList<int>
    mList.InsertHead 1
    Assert.AreEqual(Some 1, mList.Pop())

  [<TestCase([|1; 2; 3; 4; 5|], 100, Result = [|100; 1; 2; 3; 4; 5|])>]
  [<TestCase([|'a'; 'b'; 'c'|], 'x', Result = [|'x'; 'a'; 'b'; 'c'|])>]
  [<TestCase([| false; false; false |], true, Result = [| true; false; false; false |])>]
  member this.ArrayListInsertHead array x =
    let mList = new ArrayList<'A>(array) :> IList<'A>
    mList.InsertHead x
    listToArr mList 

  [<Test>]
  member this.ArrayListInsertTailEmptyList() = 
    let array = [||]
    let mList = new ArrayList<int>(array) :> IList<int>
    mList.InsertTail 1
    Assert.AreEqual([|1|], listToArr mList)

  [<TestCase([|1; 2; 3; 4; 5|], 100, Result = [|1; 2; 3; 4; 5; 100|])>]
  [<TestCase([|'a'; 'b'; 'c'|], 'x', Result = [|'a'; 'b'; 'c'; 'x'|])>]
  [<TestCase([| false; false; false |], true, Result = [| false; false; false; true|])>]
  member this.ArrayListInsertTail array x =
    let mList = new ArrayList<'A>(array) :> IList<'A>
    mList.InsertTail x
    listToArr mList

  [<Test>]
  member this.ArrayListInsertAtEmptyList() = 
    let array = [||]
    let mList = new ArrayList<int>(array) :> IList<int>
    let b = mList.InsertAt 9 1
    Assert.AreEqual( (Some 9, true) , (mList.Pop(), b) )

  [<TestCase([|1; 2; 3; 4; 5|], 100, -1, Result = false)>]
  [<TestCase([|1; 2; 3; 4; 5|], 100, 0, Result = false)>]
  [<TestCase([|'a'; 'b'; 'c'|], 'x', 5, Result = false)>]
  member this.ArrayListInsertAtWrongIndex array x index = 
    let mList = new ArrayList<'A>(array) :> IList<'A>
    mList.InsertAt x index 
  
  [<TestCase([|1; 3|], 2, 2, Result = [|1; 2; 3|])>]
  [<TestCase([|1; 2|], 3, 3, Result = [|1; 2; 3|])>]
  [<TestCase([|2; 3|], 1, 1, Result = [|1; 2; 3|])>]
  [<TestCase([|'a'; 'c'|], 'b', 2, Result = [|'a'; 'b'; 'c'|])>]
  member this.ArrayListInsertAt array x index = 
    let mList = new ArrayList<'A>(array) :> IList<'A>
    let b = mList.InsertAt x index
    listToArr mList 
 
  [<Test>]
  member this.ArrayListInsertLongList() = 
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
  member this.ArrayListRemoveHeadEmptyList() = 
    let array = [||]
    let mList = new ArrayList<int>(array) :> IList<int>
    mList.RemoveHead()
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ArrayListRemoveHeadList() = 
    let mList = new ArrayList<int>([|1;2;3;4;5;6;7;8;9;0|]) :> IList<int>
    mList.RemoveHead()
    Assert.AreEqual([|2;3;4;5;6;7;8;9;0|], listToArr mList)

  [<Test>]
  member this.ArrayListRemoveTailEmptyList() = 
    let array = [||]
    let mList = new ArrayList<obj>(array) :> IList<obj>
    mList.RemoveTail()
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ArrayListRemoveTailList() = 
    let mList = new ArrayList<int>([|1;2;3;4;5;6;7;8;9;0|]) :> IList<int>
    mList.RemoveTail()
    Assert.AreEqual([|1;2;3;4;5;6;7;8;9|], listToArr mList)

  [<Test>]
  member this.ArrayListRemoveAtEmptyList() = 
    let array = [||]
    let mList = new ArrayList<int>(array) :> IList<int>
    let b = mList.RemoveAt 1
    Assert.AreEqual(false, b )

  [<Test>]
  member this.ArrayListRemoveAtWrongIndex() = 
    let mList = new ArrayList<int>([|1;2;3|]) :> IList<int>
    let b = mList.RemoveAt 4 
    Assert.AreEqual(false, b)

  [<Test>]
  member this.ArrayListRemoveAtList() = 
    let mList = new ArrayList<char>([|'a';'b';'c';'d';'e';'f'|]) :> IList<char>
    ignore (mList.RemoveAt 1)
    ignore (mList.RemoveAt 5)
    ignore (mList.RemoveAt 3)
    Assert.AreEqual([|'b';'c';'e'|], listToArr mList)

  [<Test>]
  member this.ArrayListFindEmptyList() = 
    let array = [||]
    let mList = new ArrayList<obj>(array) :> IList<obj>
    let ans = mList.TryFind (fun x -> true) 
    Assert.AreEqual(None, ans)

  [<Test>]
  member this.ArrayListFindNothing() = 
    let array = [|1; 2; 3; 4; 5|]
    let mList = new ArrayList<int>(array) :> IList<int>
    let ans = mList.TryFind (fun x -> if x > 10 then true else false) 
    Assert.AreEqual(None, ans)

  [<Test>]
  member this.ArrayListTryFind01() =
    let array = [|1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19;20|]
    let mList = new ArrayList<int>(array) :> IList<int>
    let ans = mList.TryFind (fun x -> if (x % 2) = 0 then true else false)
    Assert.AreEqual(Some 2, ans)

  [<Test>]
  member this.ArrayListConcatEmptyLists() =
    let mList1 = new ArrayList<obj>([||]) :> IList<obj>
    let mList2 = new ArrayList<obj>([||]) :> IList<obj>
    mList1.Concat mList2
    Assert.AreEqual(None, mList1.Pop())

  [<Test>]
  member this.ArrayListConcatLists() =
    let mList1 = new ArrayList<char>([|'a';'b'|]) :> IList<char>
    let mList2 = new ArrayList<char>([|'c';'d'|]) :> IList<char>
    mList1.Concat mList2
    Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

  [<Test>]
  member this.ArrayConcatWithATDList() =
    let mList1 = new ArrayList<char>( [|'a';'b'|] ) :> IList<char>
    let mList2 = new ATDList<char>( Node('c', Node('d', Void)) ) :> IList<char>
    mList1.Concat mList2
    Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

[<TestFixture>]
type TestATDList () =

  let listToArr mList = 
    let f() = 
      match (mList :> IList<'A>).Pop() with
      | None -> 
        failwith "Wrong result in testing"
      | Some x -> x
    let length = mList.Length()
    Array.map (fun x -> f() ) [|1..length|]

  [<Test>]
  member this.ATDListPopEmptyList() = 
    let list = Void 
    let mList = new ATDList<obj>(list) :> IList<obj>
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ATDListInsertHeadEmptyList() = 
    let list = Void
    let mList = new ATDList<obj>(list) :> IList<obj>
    mList.InsertHead 1
    Assert.AreEqual([|1|], listToArr mList)

  [<Test>]
  member this.ATDListInsertHead() =
    let list = Node(1, Node(2, Node(3, Void)))
    let mList = new ATDList<int>(list) :> IList<int>
    mList.InsertHead 0
    Assert.AreEqual([|0; 1; 2; 3|], listToArr mList)

  [<Test>]
  member this.ATDListInsertTailEmptyList() = 
    let list = Void
    let mList = new ATDList<int>(list) :> IList<int>
    mList.InsertTail 1
    Assert.AreEqual([|1|], listToArr mList)

  [<Test>]
  member this.ATDListInsertTail() =
    let list = Node('a', Node('b', Node('c', Void)))
    let mList = new ATDList<char>(list) :> IList<char>
    mList.InsertTail 'z'
    Assert.AreEqual([|'a';'b';'c';'z'|], listToArr mList)

  [<Test>]
  member this.ATDListInsertAtEmptyList() = 
    let list = Void
    let mList = new ATDList<int>(list) :> IList<int>
    let b = mList.InsertAt 9 1
    Assert.AreEqual( (Some 9, true) , (mList.Pop(), b) )

  [<Test>]
  member this.ATDListInsertAtWrongIndex() = 
    let list = Node(1, Node(2, Node(3, Void)))
    let mList = new ATDList<int>(list) :> IList<int>
    let b = mList.InsertAt 5 -1
    Assert.AreEqual( false, b )
  
  [<Test>]
  member this.ATDListInsertAt() =
    let list = Node('b', Node('c', Node('d', Void)))
    let mList = new ATDList<char>(list) :> IList<char>
    ignore (mList.InsertAt 'a' 1)
    ignore (mList.InsertAt 'e' 5)
    ignore (mList.InsertAt 'z' 3)
    Assert.AreEqual([|'a';'b';'z';'c';'d';'e'|], listToArr mList)

  [<Test>]
  member this.ATDListRemoveHeadEmptyList() = 
    let list = Void
    let mList = new ATDList<obj>(list) :> IList<obj>
    mList.RemoveHead()
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ATDListRemoveHeadList() = 
    let list = Node(1, Node(2, Node(3, Void))) 
    let mList = new ATDList<int>(list) :> IList<int>
    mList.RemoveHead()
    Assert.AreEqual([|2;3|], listToArr mList)

  [<Test>]
  member this.ATDListRemoveTailEmptyList() = 
    let list = Void
    let mList = new ATDList<obj>(list) :> IList<obj>
    mList.RemoveTail()
    Assert.AreEqual(None, mList.Pop())

  [<Test>]
  member this.ATDListRemoveTail() = 
    let list = Node(1, Node(2, Node(3, Void)))
    let mList = new ATDList<int>(list) :> IList<int>
    mList.RemoveTail()
    Assert.AreEqual([|1;2|], listToArr mList)

  [<Test>]
  member this.ATDListRemoveAtEmptyList() = 
    let list = Void
    let mList = new ATDList<obj>(list) :> IList<obj>
    let b = mList.RemoveAt 1
    Assert.AreEqual(false, b)

  [<Test>]
  member this.ATDListRemoveAtWrongIndex() = 
    let list = Node(1, Node(2, Node(3, Node(4, Void))))
    let mList = new ATDList<int>(list) :> IList<int>
    let b = mList.RemoveAt 5 
    Assert.AreEqual([|1;2;3;4|], listToArr mList)

  [<Test>]
  member this.ATDListRemoveAt() = 
    let list = Node('a', Node('b', Node('c', Node('d', Node('e', Node('f', Void))))))
    let mList = new ATDList<char>(list) :> IList<char>
    ignore (mList.RemoveAt 1)
    ignore (mList.RemoveAt 5)
    ignore (mList.RemoveAt 3)
    Assert.AreEqual([|'b';'c';'e'|] , listToArr mList)

  [<Test>]
  member this.ATDListFindEmptyList() = 
    let list = Void
    let mList = new ATDList<obj>(list) :> IList<obj>
    let ans = mList.TryFind (fun x -> true) 
    Assert.AreEqual(None, ans)

  [<Test>]
  member this.ATDListFindNothing() = 
    let list = Node(1, Node(2, Node(3, Node(4, Void))))
    let mList = new ATDList<int>(list) :> IList<int>
    let ans = mList.TryFind (fun x -> if x > 10 then true else false) 
    Assert.AreEqual(None, ans)

  [<Test>]
  member this.ATDListTryFind01() =
    let list = Node(1, Node(2, Node(3, Node(4, Void))))
    let mList = new ATDList<int>(list) :> IList<int>
    let ans = mList.TryFind (fun x -> if (x % 2) = 0 then true else false)
    Assert.AreEqual(Some 2, ans)

  [<Test>]
  member this.ATDListConcatEmptyLists() =
    let mList1 = new ATDList<obj>(Void) :> IList<obj>
    let mList2 = new ATDList<obj>(Void) :> IList<obj>
    mList1.Concat mList2
    Assert.AreEqual(None, mList1.Pop())

  [<Test>]
  member this.ATDListConcatLists() =
    let mList1 = new ATDList<char>( Node('a', Node('b', Void)) ) :> IList<char>
    let mList2 = new ATDList<char>( Node('c', Node('d', Void)) ) :> IList<char>
    mList1.Concat mList2
    Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)

  [<Test>]
  member this.ATDConcatWithArrayList() =
    let mList1 = new ATDList<char>( Node('a', Node('b', Void)) ) :> IList<char>
    let mList2 = new ArrayList<char>( [|'c';'d'|] ) :> IList<char>
    mList1.Concat mList2
    Assert.AreEqual([|'a';'b';'c';'d'|], listToArr mList1)



[<EntryPoint>]
let main argv =

  0