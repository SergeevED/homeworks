module BuildTreeTest

open Interpreter

open NUnit.Framework


[<Test>] 
let TestBuidTreeReadWrite() =
  let interpr = Interpreter()
  let st = Stack([";";"read";"x";"write";"+";"x";"1"])
  let ans = Seq(Read "x", Write (BinOp ('+', Var "x", Num 1)))
  Assert.AreEqual (ans, interpr.BuildSyntaxTree st)

[<Test>]
let TestBuildTreeAssign() =       
  let interpr = Interpreter()   
  let st = Stack([":=";"x";"1"])
  let ans = Assign("x", Num 1)
  Assert.AreEqual (ans, interpr.BuildSyntaxTree st)

[<Test>]
let TestBuildTreeIfThenElse() =  
  let interpr = Interpreter()          
  let st = Stack(["if";"x";":=";"y";"+";"x";"1";":=";"y";"-";"x";"1"])
  let ans = If(Var "x", Assign("y", BinOp('+', Var "x", Num 1)), Assign("y", BinOp('-', Var "x", Num 1)))
  Assert.AreEqual (ans, interpr.BuildSyntaxTree st)

[<Test>]
let TestBuildTreeWhile() =
  let interpr = Interpreter()   
  let st = Stack(["while";"x";";";":=";"y";"+";"x";"1";":=";"y";"-";"x";"1"])
  let ans = While(Var "x", Seq(Assign("y", BinOp('+', Var "x", Num 1)), Assign("y", BinOp('-', Var "x", Num 1))))
  Assert.AreEqual (ans, interpr.BuildSyntaxTree st)

[<Test>]
let TestBuildTreeSeq() =
  let interpr = Interpreter()   
  let st = Stack([";";":=";"x";"1";";";"write";"y";"read";"b"])
  let ans = Seq(Assign("x", Num 1), Seq(Write(Var "y"), Read("b")))
  Assert.AreEqual (ans, interpr.BuildSyntaxTree st)

[<TestCase([|"read"|], Result = "Variable was expected")>]
[<TestCase([|"write"|], Result = "Expression was expected")>]
[<TestCase([|"if"|], Result = "Expression was expected")>]
[<TestCase([|"if";"x"|], Result = "Lexeme was expected")>]
[<TestCase([|"if";"x";"write";"1"|], Result = "Lexeme was expected")>]
[<TestCase([|"while"|], Result = "Expression was expected")>]
[<TestCase([|"while";"x"|], Result = "Lexeme was expected")>]
[<TestCase([|":="|], Result = "Variable was expected")>]
[<TestCase([|":=";"x"|], Result = "Expression was expected")>]
[<TestCase([|":=";"x";"+";"1"|], Result = "Expression was expected")>]
[<TestCase([|";";"write";"1"|], Result = "Lexeme was expected")>]
[<TestCase([|"+";"1";"x"|], Result = "Unexpected token. Lexeme was expected.")>]
[<TestCase([|"write";"read";|], Result = "Unexpected token. Expression was expected.")>]
[<TestCase([|"while";":=";"x";"y";"write";"0"|], Result = "Unexpected token. Expression was expected.")>]
let TestBuildTreeIncorrectInput arr =
  let interpr = Interpreter()   
  let st = Stack(Array.toList(arr))
  try 
    string(interpr.BuildSyntaxTree st)
  with
    | LexicalError message | UnexpectedEndOfProgram message -> message
