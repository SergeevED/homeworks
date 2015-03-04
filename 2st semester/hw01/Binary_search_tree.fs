type tree = Void | Node of tree * tree * int

let rec insert binTree x =
  match binTree with
  | Void -> Node(Void, Void, x)
  | Node(l, r, k) ->
    if x < k then Node(insert l x, r, k)
    elif x > k then Node(l, insert r x, k)
    else Node(l, r, k)

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

let rec printLCR (binTree : tree) = 
  match binTree with
  | Void -> ()
  | Node(l, r, k) -> 
    (printLCR l)
    printf "%i " k
    (printLCR r)
  ()

let rec printLRC (binTree : tree) = 
  match binTree with
  | Void -> ()
  | Node(l, r, k) -> 
    (printLRC l)
    (printLRC r)
    printf "%i " k
  ()

let rec printCLR (binTree : tree) = 
  match binTree with
  | Void -> ()
  | Node(l, r, k) -> 
    printf "%i " k
    (printCLR l)
    (printCLR r)
  ()

let rec printTree (binTree : tree) =
  match binTree with
  | Void -> ()
  | Node(l, r, k) ->
    if l <> Void then
      printf "{" 
      printTree l
      printf "}"
    else
      printf "Void"
    printf ", %i, " k
    if r <> Void then
      printf "{"
      printTree r
      printf "}"
    else
      printf "Void"
  ()

[<EntryPoint>]
let main args = 
  printf "adding 5, 3, 7, 9\n"
  let t = (insert (insert (insert (insert Void 5) 3) 7) 9)
  printTree t
  printf "\n"
  printf "deleting 9, 5\n"
  let t = (remove (remove t 9) 5)
  printTree t
  printf "\n"
  printf "adding 8, 2, 6\n"
  let t = (insert (insert (insert t 8) 2) 6)
  printTree t
  printf "\n"
  printf "LCR: "
  printLCR t
  printf "\n"
  printf "CLR: "
  printCLR t
  printf "\n"
  printf "LRC: "
  printLRC t
  printf "\n"
  0