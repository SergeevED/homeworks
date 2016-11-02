module ExprCalcTest

open Interpreter

open NUnit.Framework


[<Test>]
let TestPlus() =
  let expr = BinOp('+', BinOp('+', Num 1, Num 2), Num 3)
  Assert.AreEqual (6, calculateExpression expr [])

[<Test>]
let TestMinus() =
  let expr = BinOp('-', BinOp('-', Num 1, Num 2), Num 3)
  Assert.AreEqual (-4, calculateExpression expr [])

[<Test>]
let TestMultiplying() =
  let expr = BinOp('*', BinOp('*', Num 2, Num 3), Num 4)
  Assert.AreEqual (24, calculateExpression expr [])

[<Test>]
let TestDiv() =
  let expr = BinOp('/', BinOp('/', Num 100, Num 10), Num 2)
  Assert.AreEqual (5, calculateExpression expr [])

[<Test>]
let TestMod() =
  let expr = BinOp('%', BinOp('%', Num 115, Num 10), Num 2)
  Assert.AreEqual (1, calculateExpression expr [])

[<Test>]
let TestPow() =
  let expr = BinOp('^', BinOp('^', Num 2, Num 1), Num 3)
  Assert.AreEqual (8, calculateExpression expr [])

[<Test>]
let TestDivisionByZero() =
  let expr = BinOp('/', Num 1, Num 0)
  let ans =
    try 
      string(calculateExpression expr [])
    with
      | IncorrectOperation message -> message
  Assert.AreEqual ("Division by zero", ans)

[<Test>]
let TestVariables() =
  let varList = [("x",1);("y",2);("z",3)]
  let expr = BinOp('+', BinOp('-', Var "x", Var "y"), Var "z")
  Assert.AreEqual (2, calculateExpression expr varList)

[<Test>]
let TestComplicatedExpression() =
  let expr = BinOp('/', BinOp('*', BinOp('-', Num 5, Num 10), Num 20), BinOp('^', Num 2, Num 3))
  Assert.AreEqual (-12, calculateExpression expr [])