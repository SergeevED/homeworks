module InterpretateTest

open Interpreter

open NUnit.Framework


[<TestCase("read\nx\n", Result = [||])>] 
[<TestCase("write\n1\n", Result = [|"1"|])>] 
[<TestCase(";\n:=\nx\n1\nwrite\nx\n", Result = [|"1"|])>]
[<TestCase(";\n:=\nx\n1\n;\n:=\nx\n+\n*\nx\n2\n3\nwrite\nx\n", Result = [|"5"|])>]
[<TestCase(";\nread\na\n;\nread\nb\n;\nread\nc\n;\nwrite\na\n;\nwrite\nb\nwrite\nc\n", Result = [|"0";"1";"2"|])>]
[<TestCase(";\n:=\na\n0\n;\n:=\nb\n1\n;\n:=\nc\n2\n;\nwrite\na\n;\nwrite\nb\nwrite\nc\n", Result = [|"0";"1";"2"|])>]
[<TestCase(";\n:=\nx\n1\nif\nx\nwrite\n+\nx\n1\nwrite\nx\n", Result = [|"2"|])>] 
[<TestCase(";\n:=\nx\n0\nif\nx\nwrite\n+\nx\n1\nwrite\nx\n", Result = [|"0"|])>] 
[<TestCase(";\n:=\nx\n5\nwhile\nx\n;\n:=\nx\n-\nx\n1\nwrite\nx\n", Result = [|"4";"3";"2";"1";"0"|])>] 
let TestInterpreterCorrectInput program =
  let interpr = Interpreter()
  List.toArray (interpr.Interpretate program [0;1;2] false)

[<TestCase(":=\nx\nwrite\n1\n", Result = [|"Lexical error: Unexpected token. Expression was expected."|])>] 
[<TestCase(":=\nwrite\n1\n", Result = [|"Lexical error: Unexpected token. Variable was expected."|])>]
[<TestCase(":=\nx\n/\n1\n0\n", Result = [|"Incorrect operation: Division by zero"|])>] 
[<TestCase(":=\nx\n%\n1\n0\n", Result = [|"Incorrect operation: Division by zero"|])>] 
[<TestCase(":=\nx\n", Result = [|"Unexpected end of program: Expression was expected"|])>] 
[<TestCase("if\n1\n", Result = [|"Unexpected end of program: Lexeme was expected"|])>]
[<TestCase(":=\n", Result = [|"Unexpected end of program: Variable was expected"|])>]
[<TestCase("write\nx\n", Result = [|"Unknown identifier: x"|])>]
[<TestCase(";\nread\nx\nread\ny\n", Result = [|"Incorrect input data: Unexpected end of input list"|])>]
let TestInterpreterIncorrectInput program =
  let interpr = Interpreter()
  List.toArray (interpr.Interpretate program [1] false)