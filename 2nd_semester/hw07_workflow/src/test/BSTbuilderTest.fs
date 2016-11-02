module BSTbuilderTest

open BSTbuilder

open NUnit.Framework

[<Test>]
let TestBSTMap01() =
  let tree = Node(Void, Node(Void, Node(Void, Void, 3), 2), 1)
  let resTree = bstMap (fun x -> x + 1) tree 
  let ans = Node(Void, Node(Void, Node(Void, Void, 4), 3), 2)
  Assert.AreEqual (resTree, ans)

[<Test>]
let TestBSTMap02() =
  let tree = Void
  let resTree = bstMap (fun x -> x + 1) tree 
  let ans = Void
  Assert.AreEqual (resTree, ans)

[<Test>]
let TestBSTFilter01() =
  let tree = Node(Node(Void, Void, 1), Node(Void, Void, 3), 2)
  let resTree = bstFilter (fun x -> (x % 2) = 1) tree 
  let ans = Node(Void, Node(Void, Void, 3), 1)
  Assert.AreEqual (resTree, ans)

[<Test>]
let TestBSTFilter02() =
  let tree = Void
  let resTree = bstFilter (fun x -> (x % 2) = 1) tree 
  let ans = Void
  Assert.AreEqual (resTree, ans)

[<Test>]
let TestBSTApplyBinOp01() =
  let tree1 = Node(Node(Void, Void, 1), Node(Void, Void, 2), 3)
  let tree2 = Node(Node(Void, Void, 5), Void, 10) 
  let resTree = bstApplyBinOp (*) tree1 tree2
  let ans = 
    Node(Void, Node(Void, Node(Void, Node(Void, Node(Node(Void, Void, 20), Void, 30), 15), 10), 10), 5)
  Assert.AreEqual (resTree, ans)

[<Test>]
let TestBSTApplyBinOp02() =
  let tree1 = Void
  let tree2 = Node(Node(Void, Void, 1), Node(Void, Void, 3), 2)
  let resTree = bstApplyBinOp (*) tree1 tree2
  let ans = Void
  Assert.AreEqual (resTree, ans)
