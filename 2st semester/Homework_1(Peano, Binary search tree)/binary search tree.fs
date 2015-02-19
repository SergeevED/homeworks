type tree = Void | Node of tree * tree * int

let rec insert binTree x =
  match binTree with
  |Void -> Node(Void, Void, x)
  |Node(l, r, k) ->
    if x<k then Node(insert l x, r, k)
    elif x>k then Node(l, insert r x, k)
    else Node(l, r, k)

let rec remove binTree x =
  let rec takeSmallestLeaf binTree =
    match binTree with
    |Void -> Void
    |Node(l, r, k) ->
      if l = Void then binTree 
      else takeSmallestLeaf l  
  match binTree with
  |Void -> Void
  |Node(l, r, k) when x < k -> Node(remove l x, r, k)
  |Node(l, r, k) when x > k -> Node(l, remove r x, k)
  |Node(l, r, k) when x = k ->
    if l = Void then r
    elif r = Void then l
    else 
      let smallestLeaf = takeSmallestLeaf r
      match smallestLeaf with
      |Void -> Void
      |Node(ls, rs, ks) -> Node(l, remove r ks, ks) 

let rec printLCR (binTree : tree) = 
  match binTree with
  |Void -> ()
  |Node(l, r, k) -> 
    (printLCR l)
    printf "%i " k
    (printLCR r)
  ()

let rec printLRC (binTree : tree) = 
  match binTree with
  |Void -> ()
  |Node(l, r, k) -> 
    (printLRC l)
    (printLRC r)
    printf "%i " k
  ()

let rec printCLR (binTree : tree) = 
  match binTree with
  |Void -> ()
  |Node(l, r, k) -> 
    printf "%i " k
    (printCLR l)
    (printCLR r)
  ()