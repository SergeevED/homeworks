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

let rec sumTree (binTree : Tree<int>) : Option<int> = 
  match binTree with
  | Void -> None
  | Node(l, r, k) ->
    let btree = foldTree (fun lacc acc racc -> lacc + acc + racc) 0 binTree
    Some btree

let rec minTree (binTree : Tree<'A>) : Option<'A> = 
  match binTree with
  | Void -> None
  | Node(l, r, k) ->
    let btree = foldTree (fun lacc acc racc -> 
      if lacc < acc then
        if lacc < racc then lacc
        else racc
      else
        if acc < racc then acc
        else racc) k binTree
    Some btree

let rec copyTree binTree = foldTree (fun lacc acc racc -> Node(lacc, racc, acc)) Void binTree

[<EntryPoint>]
let main args = 
  let tree = (insert (insert (insert (insert Void 5) 3) 7) 9)
  printf "tree "
  printTree tree
  printf "\n"

  let mappedTree = mapTree (fun x -> x + 1) tree
  printf "mapped tree (incremention) "
  printTree mappedTree
  printf "\n"

  let sum = sumTree tree
  match sum with
  | None -> printf "None\n"
  | Some a ->  printf "sum of elements %d\n" a

  let min = minTree tree
  match min with
  | None -> printf "None\n"
  | Some a -> printf "minimal element %d\n" a

  let copiedTree = copyTree tree
  printf "copied tree " 
  printTree copiedTree
  printf "\n"

  0