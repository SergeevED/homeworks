module tree

type Tree<'A> = Void | Node of Tree<'A> * Tree<'A> * 'A

type Option<'A> = None | Some of 'A

let rec insert binTree x =
  match binTree with
  | Void -> Node(Void, Void, x)
  | Node(l, r, k) ->
    match compare x k with
    | a when a < 0 -> Node(insert l x, r, k)
    | a when a > 0 -> Node(l, insert r x, k)
    | _ -> Node(l, r, k)

let rec remove binTree x =
  let rec takeSmallestLeaf binTree =
    match binTree with
    | Void -> Void
    | Node(l, r, k) ->
      if l = Void then binTree 
      else takeSmallestLeaf l  
  match binTree with
  | Void -> Void
  | Node(l, r, k) ->
    if x < k then Node(remove l x, r, k)
    elif x > k then Node(l, remove r x, k)
    else
      if l = Void then r
      elif r = Void then l
      else 
        let smallestLeaf = takeSmallestLeaf r
        match smallestLeaf with
        | Void -> Void
        | Node(ls, rs, ks) -> Node(l, remove r ks, ks)

let rec printLCR binTree = 
  match binTree with
  | Void -> ()
  | Node(l, r, k) -> 
    (printLCR l)
    printf "%A " k
    (printLCR r)
  ()

let rec printTree binTree =
  match binTree with
  | Void -> ()
  | Node(l, r, k) ->
    if l <> Void then
      printf "{" 
      printTree l
      printf "}"
    else
      printf "Void"
    printf ", %A, " k
    if r <> Void then
      printf "{"
      printTree r
      printf "}"
    else
      printf "Void"
  ()

let rec mapTree f binTree =
  match binTree with
  | Void -> Void
  | Node(l, r, k) -> Node(mapTree f l, mapTree f r, f k)

let rec foldTree f acc binTree =
  match binTree with
  | Void -> acc
  | Node(l, r, k) -> f (foldTree f acc l) k (foldTree f acc r)

let rec sumTree binTree = foldTree (fun lacc acc racc -> lacc + acc + racc) 0 binTree

let minElem lacc acc racc =
  match lacc with
  | None -> Some acc
  | _ -> lacc

let rec minTree binTree = foldTree minElem None binTree

let rec copyTree binTree = foldTree (fun lacc acc racc -> Node(lacc, racc, acc)) Void binTree

[<EntryPoint>]
let main args = 
  let tree = (insert (insert (insert (insert Void 5) 3) 8) 10)
  printf "tree "
  printTree tree
  printf "\n"

  let mappedTree = mapTree (fun x -> x + 1) tree
  printf "mapped tree (incremention) "
  printTree mappedTree
  printf "\n"

  let sum = sumTree tree
  printf "sum of elements %d\n" sum

  let min = minTree tree
  match min with
  | Some a -> printf "minimal element %d\n" a
  | _ -> printf "Empty\n"

  let copiedTree = copyTree tree
  printf "copied tree " 
  printTree copiedTree
  printf "\n"

  0