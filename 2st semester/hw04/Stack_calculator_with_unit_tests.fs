module hw
open NUnit.Framework

type Stack<'A>(V : list<'A>) =
  class
    let mutable list = V
    member this.Size() = List.length list
    member this.Reverse() = list <- (List.rev list)
    member this.Top() = 
      if this.Size() = 0 
      then None
      else Some (List.head list)
    member this.Push x = list <- x :: list
    member this.Pop() =
      let head = Option.get ( this.Top() )
      list <- List.tail list
      head
  end


let rec findVariableValue list (variable : string) =
  match list with
  | (v, value) :: tail when v = variable -> Some value
  | (v, value) :: tail when not (v = variable) -> findVariableValue tail variable
  | _ -> None


let precedence c =
  match c with
  | '^' -> 3
  | '*' | '/' | '%' -> 2
  | '+' | '-' -> 1
  | _ -> 0

let isLeftAssoc c =
  match c with
  | '+' | '-' | '*' | '/' | '%' -> true
  | '^' -> false
  | _ -> failwith "not an operator"

let isOperator c = 
  match c with
  | '+' | '-' | '*' | '/' | '%'| '^' -> true
  | _ -> false

let isNumber (c : string) =
  match ( String.length c ) with
  | 0 -> false
  | 1 -> System.Char.IsDigit (c.Chars(0))
  | x -> System.Char.IsDigit (c.Chars(1))

let rec scanToken (stack : Stack<char>) =
  match stack.Top() with
  | None -> (None, true)
  | Some x ->
    match x with
    | x when (isOperator x) -> 
      let temp = stack.Pop()
      if (x = '-') && (not (stack.Size() = 0)) && (System.Char.IsDigit (Option.get (stack.Top() )))
      then
        match (scanToken stack) with 
        | (Some strNumber, true) -> (Some("-" + strNumber), true)
        | _ -> failwith "ScanToken() wrong output"
      else
        (Some (System.Char.ToString temp), true)  
    | ' ' -> 
      ignore(stack.Pop() )
      scanToken stack
    | x when (System.Char.IsDigit x) ->
      let rec scanNumber strNumber =
        match stack.Top() with
        | None -> strNumber
        | x when (System.Char.IsDigit (Option.get x)) ->
          let temp = System.Char.ToString (stack.Pop() )
          strNumber + (scanNumber temp)
        | _ -> strNumber
      let x = stack.Pop() 
      (Some (scanNumber (System.Char.ToString x)), true)
    | x when (System.Char.IsLetter x) ->
      let rec scanLetter strNumber =
        match stack.Top() with
        | None -> strNumber
        | x when (System.Char.IsLetter (Option.get x)) ->
          let temp = System.Char.ToString (stack.Pop() )
          strNumber + (scanLetter temp)
        | _ -> strNumber
      let x = stack.Pop() 
      (Some (scanLetter (System.Char.ToString x)), true)
    | x when (x = '(' || x = ')') -> (Some (System.Char.ToString (stack.Pop() )) , true)
    | _ -> (None, false) 
    
let calculate (stack : Stack<string>) =
  let result = Stack<int>([])
  let mutable isCorrect = true
  while (stack.Size() > 0) && (isCorrect) do
    let token = stack.Pop()
    match token with
    | value when (isNumber (value)) -> result.Push (int32 (value))
    | operator when (isOperator (operator.Chars(0)) ) && ((String.length operator = 1)) ->
      if result.Size() < 2 
        then isCorrect <- false
        else
          let operand2 = result.Pop()
          let operand1 = result.Pop()
          match operator with
          | "+" -> result.Push (int32(operand1) + int32(operand2)) 
          | "-" -> result.Push (int32(operand1) - int32(operand2)) 
          | "*" -> result.Push (int32(operand1) * int32(operand2)) 
          | "%" -> 
            if (operand2 = 0)
              then isCorrect <- false
              else result.Push (int32(operand1) % int32(operand2)) 
          | "/" ->
            if (operand2 = 0)
              then isCorrect <- false
              else result.Push (int32(operand1) / int32(operand2)) 
          | "^" ->
            match operand1, operand2 with
            | _ , x when x < 0 -> isCorrect <- false
            | 0 , 0 -> isCorrect <- false
            | val1 , val2 when val1 = 0 -> result.Push 0
            | x , 0 -> result.Push 1
            | val1 , val2 -> result.Push (pown val1 val2)
            | _ , _ -> isCorrect <- false
          | _ -> isCorrect <- false
    | _ -> isCorrect <- false      
  if isCorrect = false 
    then None 
    else 
      if not (stack.Size() = 0) 
      then None
      else  Some(result.Pop() )

let  firstOperatorAfterSecond op1 op2 =
  (isLeftAssoc op1) && (precedence op1 <= precedence op2)
  || not (isLeftAssoc op1) && (precedence op1 < precedence op2)

type Program = Continue | EndCorrect | EndIncorrect 

let stackCalc (str : string) variableList = 
  let list = Array.toList (str.ToCharArray() )
  let mutable program = Continue
  let mutable operatorWasScanned = true          
  let outputStack = Stack<string>([])
  let inputStack = Stack<char>(list)
  let tempStack = Stack<char>([])
  while program = Continue do
    let mutable token = scanToken inputStack
    match token with
    | (_, false) -> program <- EndIncorrect
    | (Some x, true) when (isNumber x) ->
      if not operatorWasScanned 
        then program <- EndIncorrect
        else 
          operatorWasScanned <- false
          outputStack.Push x
    | (Some x, true) when (System.Char.IsLetter (x.Chars(0)) ) -> 
      if not operatorWasScanned 
        then program <- EndIncorrect
        else
          operatorWasScanned <- false
          match (findVariableValue variableList x) with
          | None -> program <- EndIncorrect
          | Some x -> outputStack.Push (string x)
    | (Some x, true) when (isOperator (x.Chars(0)) ) && ((String.length x) = 1) ->     
      if operatorWasScanned 
        then program <- EndIncorrect
        else
          operatorWasScanned <- true
          while (tempStack.Size() <> 0) 
          && (firstOperatorAfterSecond (x.Chars(0) ) (Option.get (tempStack.Top() ))) do
            outputStack.Push (string (tempStack.Pop() ))
          tempStack.Push (x.Chars(0))
    | (Some x, true) when x.Chars(0) = '(' -> 
      if not operatorWasScanned
        then program <- EndIncorrect
        else tempStack.Push (x.Chars(0))
    | (Some x, true) when x.Chars(0) = ')' ->
      if operatorWasScanned
        then program <- EndIncorrect
        else
          while (tempStack.Size() <> 0) && (tempStack.Top() <> Some '(')  do
            outputStack.Push (string (tempStack.Pop() ))
          if tempStack.Size() = 0 
            then program <- EndIncorrect
            else ignore (tempStack.Pop() )
    | (None, true) -> program <- EndCorrect
    | _ -> program <- EndIncorrect
  match program with
  | EndIncorrect -> None
  | _ ->
    while (tempStack.Size() <> 0) do 
      outputStack.Push (string (tempStack.Pop() ))
    outputStack.Reverse() 
    calculate outputStack


[<TestFixture>]
type TestStackCalculator () =

  let values1 = [("x",0)]
  let values2 = [("x",-1)]
  let values3 = [("x",12345)]

  [<TestCase("1/0")>]
  [<TestCase("1%0")>]
  [<TestCase("1/x")>]
  member this.DivisionByZero str =
    Assert.AreEqual(None, stackCalc str values1)


  [<TestCase("0^0")>]
  [<TestCase("1^(-1)")>]
  [<TestCase("1+2^")>]
  [<TestCase("2^x")>]
  member this.IncorrectPow str =
    Assert.AreEqual(None, stackCalc str values2)


  [<TestCase("1-")>]
  [<TestCase("*1-3")>]
  [<TestCase("5*/3")>]
  [<TestCase("1 2 + 3")>]
  [<TestCase("1+(1-3)")>]
  [<TestCase("1-2+3x%5")>]
  member this.IncorrectOrder str =
    Assert.AreEqual(None, stackCalc str values2)  


  [<TestCase("123*2$")>]
  [<TestCase("*12+!2")>]
  member this.IncorrectSymbols str =
    Assert.AreEqual(None, stackCalc str [])


  [<TestCase("1*(2-)*2")>]
  [<TestCase("1*(2-3)2+1")>]
  [<TestCase("1*(2-3)x+1")>]
  [<TestCase("1+2*(1")>]
  [<TestCase("1*2+3)*2")>]
  member this.TestBracketsIncorrect str =
    Assert.AreEqual(None, stackCalc str values2)


  [<Test>]
  member this.TestValues() =
    let str = "a+b-c*d/e%f"
    let values = [("a",10);("b",8);("c",9);("d",10);("e",2);("f",4)]
    Assert.AreEqual(Some 17, stackCalc str values)


  [<Test>]
  member this.TestUnaryMinus() =
    let str = "(-5)+(-6)*((-2)*3)"
    Assert.AreEqual(Some 31, stackCalc str [])


  [<TestCase("100 + 200 - 400 * 30 / 50 % 39", Result = 294)>]
  [<TestCase("(1234567 *123 - 98765432+1)/12345+555555", Result = 559855)>]
  [<TestCase("1+2*4^3^2", Result = 524289)>]
  member this.TestExpression str =
    Option.get (stackCalc str values3)


[<EntryPoint>]
let main args =  
  0