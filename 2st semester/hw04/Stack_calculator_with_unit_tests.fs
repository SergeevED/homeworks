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
      if this.Size() = 0 
      then None
      else
        let head = this.Top()
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
  | 1 -> if (System.Char.IsDigit (c.Chars(0))) then true else false
  | x -> if (System.Char.IsDigit (c.Chars(1))) then true else false 

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
        (Some (System.Char.ToString (Option.get (temp ))), true)  
    | ' ' -> 
      ignore( stack.Pop() )
      scanToken stack
    | x when (System.Char.IsDigit x) ->
      let rec scanNumber strNumber =
        match stack.Top() with
        | None -> strNumber
        | x when (System.Char.IsDigit (Option.get x)) ->
          let temp = System.Char.ToString (Option.get ( stack.Pop() ))
          strNumber + (scanNumber temp)
        | _ -> strNumber
      let x = Option.get ( stack.Pop() )
      (Some (scanNumber (System.Char.ToString x)), true)
    | x when (System.Char.IsLetter x) ->
      let rec scanLetter strNumber =
        match stack.Top() with
        | None -> strNumber
        | x when (System.Char.IsLetter (Option.get x)) ->
          let temp = System.Char.ToString (Option.get ( stack.Pop() ))
          strNumber + (scanLetter temp)
        | _ -> strNumber
      let x = Option.get ( stack.Pop() )
      (Some (scanLetter (System.Char.ToString x)), true)
    | x when (x = '(' || x = ')') -> (Some (System.Char.ToString (Option.get ( stack.Pop() ))) , true)
    | _ -> (None, false) 
    
let calculate (stack : Stack<string>) =
  let result = Stack<int>([])
  let mutable isCorrect = true
  while (stack.Size() > 0) && (isCorrect) do
    let token = stack.Pop()
    match token with
    | Some(value) when (isNumber (value)) -> result.Push (int32 (value))
    | Some(operator) when (isOperator (operator.Chars(0)) ) && ((String.length operator = 1)) ->
      if result.Size() < 2 
        then isCorrect <- false
        else
          let operand2 = result.Pop()
          let operand1 = result.Pop()
          match operator with
          | "+" -> result.Push ((int32(Option.get(operand1))) + (int32(Option.get(operand2)))) 
          | "-" -> result.Push ((int32(Option.get(operand1))) - (int32(Option.get(operand2))))
          | "*" -> result.Push ((int32(Option.get(operand1))) * (int32(Option.get(operand2))))
          | "%" -> 
            if (operand2 = Some 0)
              then isCorrect <- false
              else result.Push ((int32(Option.get(operand1))) % (int32(Option.get(operand2))))
          | "/" ->
            if (operand2 = Some 0)
              then isCorrect <- false
              else result.Push ((int32(Option.get(operand1))) / (int32(Option.get(operand2))))
          | "^" ->
            match operand1, operand2 with
            | _ , Some x when x < 0 -> isCorrect <- false
            | Some 0 , Some 0 -> isCorrect <- false
            | Some val1 , Some val2 when val1 = 0 -> result.Push 0
            | Some x , Some 0 -> result.Push 1
            | Some val1 , Some val2 -> result.Push (pown val1 val2)
            | _ , _ -> isCorrect <- false
          | _ -> isCorrect <- false
    | _ -> isCorrect <- false      
  if isCorrect = false 
    then None 
    else 
      if not (stack.Size() = 0) 
      then None
      else ( result.Pop() )

type Program = Continue | EndCorrect | EndIncorrect 

let stackCalc (str : string) variableList = 
  let list = Array.toList ( str.ToCharArray() )
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
          while (not (tempStack.Size() = 0)) &&
          (isLeftAssoc (x.Chars(0)) && (precedence (x.Chars(0)) <= precedence (Option.get (tempStack.Top() )))
          || not (isLeftAssoc (x.Chars(0) )) && (precedence (x.Chars(0)) < precedence (Option.get (tempStack.Top() )))) do
            outputStack.Push (string (Option.get (tempStack.Pop() )))
          tempStack.Push (x.Chars(0))
    | (Some x, true) when x.Chars(0) = '(' -> 
      if not operatorWasScanned
        then program <- EndIncorrect
        else tempStack.Push (x.Chars(0))
    | (Some x, true) when x.Chars(0) = ')' ->
      if operatorWasScanned
        then program <- EndIncorrect
        else
          while (not (tempStack.Size() = 0)) && (not (tempStack.Top() = Some '('))  do
            outputStack.Push (string (Option.get (tempStack.Pop() )))
          if tempStack.Size() = 0 
            then program <- EndIncorrect
            else ignore (tempStack.Pop() )
    | (None, true) -> program <- EndCorrect
    | _ -> program <- EndIncorrect
  match program with
  | EndIncorrect -> None
  | _ ->
    while not (tempStack.Size() = 0) do 
      outputStack.Push (string (Option.get (tempStack.Pop() )))
    outputStack.Reverse() 
    calculate outputStack


[<Test>]
let DivisionByZero() =
  let str1 = "1/0"
  let str2 = "1%0"
  let values = [("x",0)]
  Assert.AreEqual(None, stackCalc str1 [])
  Assert.AreEqual(None, stackCalc str2 [])
  Assert.AreEqual(None, stackCalc str1 values)

[<Test>]
let IncorrectPow() =
  let str1 = "0^0"
  let str2 = "1^(-1)"
  let str3 = "1+2^"
  let str4 = "2^x"
  let values = [("x",-1)]
  Assert.AreEqual(None, stackCalc str1 [])
  Assert.AreEqual(None, stackCalc str2 [])
  Assert.AreEqual(None, stackCalc str3 [])
  Assert.AreEqual(None, stackCalc str4 values)

[<Test>]
let IncorrectOrder() =
  let str1 = "1-"
  let str2 = "*1-3"
  let str3 = "5*/3"
  let str4 = "1 2 + 3"
  let str5 = "1+(1-3)"
  let str6 = "1-2+3x%5"
  let values = [("x",-1)]
  Assert.AreEqual(None, stackCalc str1 [])
  Assert.AreEqual(None, stackCalc str2 [])
  Assert.AreEqual(None, stackCalc str3 [])
  Assert.AreEqual(None, stackCalc str4 [])
  Assert.AreEqual(None, stackCalc str5 [])
  Assert.AreEqual(None, stackCalc str6 values)

[<Test>]
let IncorrectSymbols() =
  let str1 = "123*2$"
  let str2 = "12+!2"
  Assert.AreEqual(None, stackCalc str1 [])
  Assert.AreEqual(None, stackCalc str2 [])

[<Test>]
let TestBracketsIncorrect() =
  let str1 = "1*(2-)*2"
  let str2 = "1*(2-3)2+1"
  let str3 = "1*(2-3)x+1"
  let str4 = "1+2*(1"
  let str5 = "1*2+3)*2"
  let values = [("x",-1)]
  Assert.AreEqual(None, stackCalc str1 [])
  Assert.AreEqual(None, stackCalc str2 [])
  Assert.AreEqual(None, stackCalc str3 values)
  Assert.AreEqual(None, stackCalc str4 [])
  Assert.AreEqual(None, stackCalc str5 [])

[<Test>]
let TestValues() =
  let str1 = "a+b-c*d/e%f"
  let values1 = [("a",10);("b",8);("c",9);("d",10);("e",2);("f",4)]
  let str2 = "aaaaaaaaaaaaaaa * bbbbbbbbbbbbbbb"
  let values2 = [("aaaaaaaaaaaaaaa",10);("bbbbbbbbbbbbbbb",8)]
  Assert.AreEqual(Some 17, stackCalc str1 values1)
  Assert.AreEqual(Some 80, stackCalc str2 values2)

[<Test>]
let TestUnaryMinus() =
  let str1 = "(-5)+(-6)*((-2)*3)"
  Assert.AreEqual(Some 31, stackCalc str1 [])

[<Test>]
let Test01() =
  let str1 = "100 + 200 - 400 * 30 / 50 % 39"
  Assert.AreEqual(Some 294, stackCalc str1 [])

[<Test>]
let Test02() =
  let str1 = "(1234567 *123 - 98765432+1)/12345+555555"
  let values1 = [("x",12345)]
  Assert.AreEqual(Some 559855, stackCalc str1 values1)

[<Test>]
let Test03() =
  let str1 = "1+2*4^3^2"
  Assert.AreEqual(Some 524289, stackCalc str1 [])


[<EntryPoint>]
let main args =  
  0
  