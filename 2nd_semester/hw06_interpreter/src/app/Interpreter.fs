module Interpreter

open System.IO

exception LexicalError of string
exception UnexpectedEndOfProgram of string
exception UnknownIdentifier of string 
exception IncorrectOperation of string 
exception IncorrectInputData of string

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

type expression = 
  | Num   of int
  | Var   of string
  | BinOp of char * expression * expression

type lexeme = 
  | Read   of string
  | Write  of expression
  | Assign of string * expression
  | Seq    of lexeme * lexeme
  | If     of expression * lexeme * lexeme
  | While  of expression * lexeme

let isOperator c = 
  match c with
  | '+' | '-' | '*' | '/' | '%'| '^' -> true
  | _ -> false

let isCorrectName str =
  (str <> "read") && (str <> "write") && (str <> "if") && (str <> "while") && 
  (String.exists (System.Char.IsLetter) str)

let rec scanExpr (stack : Stack<string>) =
  match stack.Top() with
  | None -> raise (UnexpectedEndOfProgram("Expression was expected"))
  | Some _ ->
    match stack.Pop() with
    | num when (String.forall (System.Char.IsDigit) num) -> Num(int num)
    | var when (isCorrectName var) -> Var(var)
    | binOp when (String.length binOp = 1)
      && (isOperator (char binOp)) -> BinOp(char binOp, scanExpr stack, scanExpr stack) 
    | " " | "\n" | "\r" -> scanExpr stack  
    | _ -> raise (LexicalError("Unexpected token. Expression was expected."))
  
let rec scanVar (stack : Stack<string>) =
  match stack.Top() with
  | None -> raise (UnexpectedEndOfProgram("Variable was expected"))
  | Some _ ->
    match stack.Pop() with
    | var when (isCorrectName var) -> var
    | " " | "\n" | "\r" -> scanVar stack
    | _ -> raise (LexicalError("Unexpected token. Variable was expected."))
 
let rec getVariableValue varList (variable : string) =
  match varList with
  | (v, value) :: tail when v = variable -> Some value
  | (v, value) :: tail when not (v = variable) -> getVariableValue tail variable
  | _ -> None

let calculateExpression expression varList =
  try
    let rec calc expr =
      match expr with
      | Num(n) -> n
      | Var(x) -> 
        match (getVariableValue varList x) with
        | None -> raise (UnknownIdentifier(x))
        | Some x -> x
      | BinOp(operator, expr1, expr2) -> 
        match operator with
        | '+' -> calc(expr1) + calc(expr2)
        | '-' -> calc(expr1) - calc(expr2)
        | '*' -> calc(expr1) * calc(expr2)
        | '/' -> calc(expr1) / calc(expr2)
        | '%' -> calc(expr1) % calc(expr2)
        | '^' -> pown (calc expr1) (calc expr2)
        | _   -> raise (LexicalError("Unexpected token. Opeator was expected."))
    calc expression
  with
    | :? System.DivideByZeroException -> raise (IncorrectOperation("Division by zero"))

type IScanVarious = 
  interface
    abstract ScanVar : unit -> Option<int>
  end 

type ScanFromConsole() =
  class
    interface IScanVarious with
      member this.ScanVar() = 
        let rec f() =
          printf "Enter integer value of variable\n"
          match System.Console.ReadLine() with
          | "" -> f()
          | num when (String.forall (System.Char.IsDigit) num) -> Some (int num)
          | _ -> None
        f()  
  end

type ScanFromList(list : list<int>) = 
  class
    let mutable inputList = list
    interface IScanVarious with
      member this.ScanVar() = 
        match inputList with 
          | a :: tail ->
            inputList <- tail
            Some a
          | [] -> None  
  end

type Interpreter() =
  class
    let mutable (inputList : list<int>) = []
    let mutable (varList : list<string*int>) = []
    let mutable (outputList : list<string>) = []

    member this.Parse (str : string) =
      let stack = Stack([])
      let mutable list = Array.toList (str.ToCharArray() )
      let mutable token = ""
      while list <> [] do
        match list.Head with
          | '\n'  ->
            list <- list.Tail
            stack.Push token
            token <- "" 
          | a -> 
            token <- token + string(a)
            list <- list.Tail
      if token <> "" then stack.Push token
      stack.Reverse()
      stack

    member this.BuildSyntaxTree stack =
      let rec scanLexeme (stack : Stack<string>) = 
        match stack.Top() with
        | None -> raise (UnexpectedEndOfProgram("Lexeme was expected"))
        | Some _ ->
          match stack.Pop() with
          | "read" -> Read(scanVar stack)
          | "write" -> Write(scanExpr stack) 
          | ":=" -> Assign(scanVar stack, scanExpr stack)
          | "if" -> If(scanExpr stack, scanLexeme stack, scanLexeme stack)
          | "while" -> While(scanExpr stack, scanLexeme stack)  
          | ";" -> Seq(scanLexeme stack, scanLexeme stack)
          | "" | " " | "\n" | "\r" -> scanLexeme stack
          | _ -> raise (LexicalError("Unexpected token. Lexeme was expected."))
      scanLexeme stack

    member this.Execute syntaxTree (scanVarious : IScanVarious) = 

      let rec perform tree =
        match tree with
        | Read(var) ->
          match scanVarious.ScanVar() with
          | None -> raise (IncorrectInputData("Unexpected end of input list"))
          | Some value -> varList <- (var, value) :: varList
        | Write(expr) -> outputList <- (string(calculateExpression expr varList)) :: outputList
        | Assign(str, expr) -> 
          let exprVal = calculateExpression expr varList
          let rec f list =
            match list with
            | [] -> [(str, exprVal)]
            | (var, value) :: tail ->
              if var = str 
                then (var, exprVal) :: tail
                else (var, value) :: (f tail)
          varList <- List.rev(f varList)
        | Seq(tree1, tree2) -> 
          perform tree1
          perform tree2
        | If(expr, thenTree, elseTree) ->
          if (calculateExpression expr varList) <> 0 
            then perform thenTree
            else perform elseTree 
        | While(expr, doTree) ->
          if (calculateExpression expr varList) <> 0 
            then 
              perform doTree
              perform tree

      perform syntaxTree 

    member this.Interpretate program inputData outputThroughConsole =    
      inputList <- []
      varList <- []
      outputList <- []
      try
        let syntaxTree = this.BuildSyntaxTree (this.Parse program)
        match outputThroughConsole with
        | true ->
          let scan = new ScanFromConsole()
          this.Execute syntaxTree scan
          outputList <- List.rev outputList
          List.iter (printf "%s ") outputList
          printf "\n"
        | false -> 
          inputList <- inputData 
          let scan = new ScanFromList(inputList)
          this.Execute syntaxTree scan
          outputList <- List.rev outputList
        outputList
      with
        | LexicalError message -> 
          if outputThroughConsole then printf "%s\n" ("Lexical error: " + message)
          ["Lexical error: " + message]
        | UnexpectedEndOfProgram message -> 
          if outputThroughConsole then printf "%s\n" ("Unexpected end of program: " + message)
          ["Unexpected end of program: " + message]
        | UnknownIdentifier message -> 
          if outputThroughConsole then printf "%s\n" ("Unknown identifier: " + message)
          ["Unknown identifier: " + message]
        | IncorrectOperation message -> 
          if outputThroughConsole then printf "%s\n" ("Incorrect operation: " + message)
          ["Incorrect operation: " + message]
        | IncorrectInputData message -> 
          if outputThroughConsole then printf "%s\n" ("Incorrect input data")
          ["Incorrect input data: " + message]

    member this.InterpretateFile (inputFile : string) inputData outputThroughConsole =
      try
        use stream = new StreamReader(inputFile)
        let program = stream.ReadToEnd()
        this.Interpretate program inputData outputThroughConsole
      with
        | _ -> ["Error in opening" + inputFile]

  end  