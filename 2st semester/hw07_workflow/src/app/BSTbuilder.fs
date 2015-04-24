module BSTbuilder

type tree = Void | Node of tree * tree * int

let rec bstInsert tree x =
  match tree with
  | Void -> Node(Void, Void, x)
  | Node(l, r, k) ->
    if x < k 
      then Node(bstInsert l x, r, k)
      else Node(l, bstInsert r x, k)

let rec bstMerge tree1 tree2 =                          //merges two arbitrary trees into binary search tree 
  let rec f tree acc =
    match tree with
    | Void -> acc
    | Node(l, r, x) -> 
      let leftAndCenter = bstInsert (f l acc) x
      if r <> Void 
        then bstMerge leftAndCenter r 
        else leftAndCenter
  f tree2 (f tree1 Void)

let bstRebuild inputTree = bstMerge inputTree Void
 
type BSTBuilder () =                                    //not balanced
  member this.Return x = Node(Void, Void, x)
  member this.ReturnFrom x = bstRebuild x
  member this.Bind (m, f) =
    let rec f1 tree =
      match tree with
      | Void -> Void
      | Node (tree1, tree2, x) -> 
        bstMerge (bstMerge (f1 tree1) (f x)) (f1 tree2)
    f1 m
  member this.For (m, f) = this.Bind (m, f)
  member this.Combine(tree1, tree2) = bstMerge tree1 tree2
  member this.Delay f = f()

let bstMap f tree = 
  BSTBuilder() {
    for i in tree do 
    return f i
  }

let bstFilter f tree = 
  BSTBuilder() {
    for i in tree do 
    if (f i) then return i else return! Void
  }

let bstApplyBinOp binOp tree1 tree2 =
  BSTBuilder() {
    for i in tree1 do
    for j in tree2 do 
    return binOp i j
  } 
