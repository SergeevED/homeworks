module hw
open NUnit.Framework
open System.IO

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
    | ' ' | '\n' | '\r' -> 
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
  if stack.Size() = 0 
    then Some 0
    else
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
    if not isCorrect 
      then None 
      else 
        if (result.Size() <> 1) 
          then None
          else Some(result.Pop() )
      
let  firstOperatorAfterSecond op1 op2 =
  (isLeftAssoc op1) && (precedence op1 <= precedence op2)
  || not (isLeftAssoc op1) && (precedence op1 < precedence op2)

type Program = Continue | EndCorrect | EndIncorrect 

let parseString (str : string) = 
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
    Some outputStack

type StackMachine() =
  class
    abstract ScanStringFromFile : string -> Option<string>
    default this.ScanStringFromFile (fileName : string) = 
      try
        use stream = new StreamReader(fileName)
        Some (stream.ReadToEnd() )
      with
        | _ -> None

    abstract PrintStackToFile : string -> Stack<string> -> bool
    default this.PrintStackToFile (fileName : string) (stack : Stack<string>) =
      try
        use stream = new StreamWriter(fileName)
        while stack.Size() <> 0 do
          stream.WriteLine (stack.Pop() )
        true
      with
        | _ -> false

    member this.Parse (inputFile : string) (outputFile : string) =
      match this.ScanStringFromFile inputFile with
      | None -> false
      | Some inputStr ->
        match (parseString inputStr) with
        | None -> false
        | Some outputStack -> (this.PrintStackToFile outputFile outputStack)

    member this.StackCalculator (inputFile : string) =
      match this.ScanStringFromFile inputFile with
      | None -> None
      | Some inputStr ->
        let list = Array.toList (inputStr.ToCharArray() )
        let inputStack = Stack<char>(list)
        let stack = Stack<string>([])
        let mutable program = Continue
        while program = Continue do
          let mutable token = scanToken inputStack
          match token with
          | (_, false) -> program <- EndIncorrect
          | (None, true) -> program <- EndCorrect
          | (Some t, true) -> stack.Push t
        if program = EndIncorrect 
          then None
          else 
            stack.Reverse()
            calculate stack

  end

  type TestStackMachine() =
    inherit StackMachine()
    let internalStack = Stack<string>([])                         // stores assigned for printing to file tokens
    override this.ScanStringFromFile (str : string) = Some str    
    override this.PrintStackToFile str stack = 
      while stack.Size() <> 0 do
        internalStack.Push (stack.Pop() )
      true
    member this.StackToString() =
      let mutable str = ""
      internalStack.Reverse()
      while internalStack.Size() <> 0 do
        str <- str + (internalStack.Pop() ) + " "
      str


[<Test>]
let TestParseEmptyString() =
  let str = ""
  let machine = TestStackMachine()
  ignore(machine.Parse str "")
  Assert.AreEqual("", "")

[<TestCase("1+2%*3")>]
[<TestCase("1+2*4^3^2-&1")>]
[<TestCase("!1-2")>]
[<TestCase("1+3-2#")>]
[<TestCase("@")>]
let TestParseIncorrectSymbol str =
  let machine = TestStackMachine()
  Assert.AreEqual(false, machine.Parse str "")

[<TestCase("1+2*(10%3)", Result = "1 2 10 3 % * + ")>]
[<TestCase(" 1 + 2 - 3 * 4 / 5 % 6", Result = "1 2 + 3 4 * 5 / 6 % - ")>]
[<TestCase("(-5)+(-6)*((-2)*3)", Result = "-5 -6 -2 3 * * + ")>]
[<TestCase("(\n\r1\n\r+\n\r3\n\r)\n\r*\n\r5\n\r", Result = "1 3 + 5 * ")>]
[<TestCase("1+2*4^3^2", Result = "1 2 4 3 2 ^ ^ * + ")>]
[<TestCase("100 + 200 - 400 * 30 / 50 % 39", Result = "100 200 + 400 30 * 50 / 39 % - ")>]
let TestParse str =
  let machine = TestStackMachine()
  ignore(machine.Parse str "")
  machine.StackToString()
  
[<Test>]
let TestCalculateEmptyString() =
  let str = ""
  let machine = TestStackMachine()
  Assert.AreEqual(Some 0, machine.StackCalculator str)

[<TestCase("1 2%+ * 3")>]
[<TestCase("1 2 3 -& *")>]
[<TestCase("!1 2 /")>]
[<TestCase("1 3 - 2#")>]
[<TestCase("@")>]
let TestCalculateIncorrectSymbol str =
  let machine = TestStackMachine()
  Assert.AreEqual(None, machine.StackCalculator str)

[<TestCase("1 0 /")>]
[<TestCase("1 0 %")>]
let DivisionByZero str =
  let machine = TestStackMachine()
  Assert.AreEqual(None, machine.StackCalculator str)

[<TestCase("0 0 ^")>]
[<TestCase("1 -1 ^")>]
[<TestCase("1 2 + ^")>]
let IncorrectPow str =
  let machine = TestStackMachine()
  Assert.AreEqual(None, machine.StackCalculator str)

[<TestCase("1-")>]
[<TestCase("*1-3")>]
[<TestCase("5*/3")>]
[<TestCase("1 2 + 3")>]
let IncorrectOrder str =
  let machine = TestStackMachine()
  Assert.AreEqual(None, machine.StackCalculator str) 

[<TestCase("100 200 + 400 30 * 50 / 39 % -", Result = 294)>]
[<TestCase("1234567 123 * 98765432 - 1 +12345 / 555555 +", Result = 559855)>]
[<TestCase("1 2 4 3 2 ^ ^ * +", Result = 524289)>]
let TestExpression str =
  let machine = TestStackMachine()
  Option.get (machine.StackCalculator str)

    
[<EntryPoint>]
let main args =  

  0
