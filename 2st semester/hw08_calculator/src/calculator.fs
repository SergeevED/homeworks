module calculator

open System.Drawing
open System.Windows.Forms

open windowParametres

let operators =
  ['+', (+); '-', (-); '*', (*); '/', (/)] |> Map.ofList

let isOperator c = 
  (string c).Length = 1 && operators.ContainsKey((string c).[0])

let isNumber str =
  str |> String.forall (fun x -> (System.Char.IsDigit x) || x = '.' || x = '-' || x = '+' || x = 'E') 

let standardize (strDouble : string) =
  match strDouble with
  | strDouble when strDouble.EndsWith "E"  -> strDouble + "+0"
  | strDouble when strDouble.EndsWith "E+" -> strDouble + "0"
  | strDouble when strDouble.EndsWith "E-" -> strDouble + "0"
  | _ -> strDouble

type Calculator() =
  class
    let mutable (operand1 : Option<double>) = None
    let mutable (operator : Option<char>)   = None
    let mutable (operand2 : Option<double>) = None
    member this.FirstOperand
      with get ()       = operand1
      and  set (optVal) = operand1 <- optVal
    member this.Operator
      with get ()        = operator
      and  set (optOper) = operator <- optOper
    member this.SecondOperand
      with get ()       = operand2
      and  set (optVal) = operand2 <- optVal
    member this.Clear() =
      operand1 <- None
      operator <- None
      operand2 <- None
    member this.ExecuteOperation () =  
      if operator = Some '/' && operand2 = Some 0.0
      then
        this.Clear()
        "Division by zero"
      else
        let result = operators.[Option.get(operator)] (Option.get operand1) (Option.get operand2)
        this.Clear()
        string result
  end

let calculator = Calculator()

let programInput =
  let textBox = new TextBox() 
  textBox.Text     <- "0"
  textBox.Location <- System.Drawing.Point(0, 0)
  textBox.ReadOnly <- true 
  textBox.Width    <- windowWidth
  textBox

let digitAction digit args =
  if (isOperator programInput.Text)
  then
    calculator.Operator <- Some(char programInput.Text)
    programInput.Text <- string digit
  else
    if programInput.Text = "0" || not (isNumber programInput.Text)
      then programInput.Text <- string digit
      else programInput.Text <- programInput.Text + (string digit)

let digitButton digit =
  let but = new Button()
  but.Text <- string digit
  if digit <> 0 
  then 
    but.Size <- Size(buttonWidth, buttonHeigth)
    let buttomX = interval + (buttonWidth + interval) * ((digit - 1) % 3)
    let buttomY = (buttonHeigth + interval) * (((digit - 1) / 3) + 2)
    but.Location <- System.Drawing.Point(buttomX, windowHeight - buttomY)
  else 
    but.Size <- Size(2 * buttonWidth + interval, buttonHeigth)
    but.Location <- System.Drawing.Point(interval, windowHeight - interval - buttonHeigth)
  but.Click.Add (digitAction digit)
  but

let binOpAction operator args =
  if calculator.FirstOperand = None && isNumber programInput.Text
  then 
    calculator.FirstOperand <- programInput.Text |> standardize |> double |> Some
    calculator.Operator <- Some(operator)
    programInput.Text   <- string operator

let binOpButtom operator =
  let but = new Button()
  but.Text <- string operator
  but.Size <- Size(buttonWidth, buttonHeigth)
  match operator with
  | '+' -> 
    but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - interval - buttonHeigth)
  | '-' -> 
    but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - 2*interval - 2*buttonHeigth)
  | '*' -> 
    but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - 3*interval - 3*buttonHeigth)
  | '/' -> 
    but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - 4*interval - 4*buttonHeigth)
  | _ -> failwith ""
  but.Click.Add (binOpAction operator)
  but

let decimalMarkAction args =
  if not (String.exists (fun x -> x = '.') programInput.Text)  
    then programInput.Text <- programInput.Text + "." 

let decimalMarkButton = 
  let but = new Button()
  but.Text <- "."
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(3 * interval + 2 * buttonWidth, windowHeight - interval - buttonHeigth)
  but.Click.Add(decimalMarkAction)
  but

let equalMarkAction args =
  if calculator.FirstOperand <> None && calculator.Operator <> None && (isNumber programInput.Text)
  then
    calculator.SecondOperand <- Some(double programInput.Text)
    programInput.Text <- calculator.ExecuteOperation()

let equalMarkButton = 
  let but = new Button()
  but.Text <- "="
  but.Size <- Size(buttonWidth, 2 * buttonHeigth + interval)
  but.Location <- System.Drawing.Point(5*interval + 4*buttonWidth, windowHeight - 2*interval - 2*buttonHeigth)
  but.Click.Add (equalMarkAction)
  but

let applyOperationToStr (f : double -> double) str =
  str |> double |> f |> string

let applyUnaryOperator operator isApplicable errorMessage args =
  if isNumber programInput.Text 
  then
    programInput.Text <- standardize programInput.Text
    if double(programInput.Text) |> isApplicable
    then 
      programInput.Text <- applyOperationToStr operator programInput.Text
    else
      programInput.Text <- errorMessage
      calculator.Clear()
  
let multiplicativeInverseButton = 
  let but = new Button()
  but.Text <- "1/x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(5*interval + 4*buttonWidth, windowHeight - 3*interval - 3*buttonHeigth)
  but.Click.Add(applyUnaryOperator (1.0 |> (/)) (0.0 |> (<>)) "Division by zero")
  but

let summationInverseButton = 
  let but = new Button()
  but.Text <- "-x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(5*interval + 4*buttonWidth, windowHeight - 4*interval - 4*buttonHeigth)
  but.Click.Add(applyUnaryOperator (-1.0 |> (*)) (fun _ -> true) "") 
  but

let sinButton = 
  let but = new Button()
  but.Text <- "sin x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(2*interval + buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (sin) (fun _ -> true) "")
  but

let arcsinButton = 
  let but = new Button()
  but.Font <- new Font(but.Font.Name, font / 1.5F, but.Font.Style, but.Font.Unit)
  but.Text <- "arcsin x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(2*interval + buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (asin) (fun x -> x >= -1.0 && x <= 1.0) "NaN")
  but

let cosButton = 
  let but = new Button()
  but.Text <- "cos x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(3*interval + 2*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (cos) (fun _ -> true) "")
  but

let arccosButton = 
  let but = new Button()
  but.Font <- new Font(but.Font.Name, font / 1.5F, but.Font.Style, but.Font.Unit)
  but.Text <- "arccos x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(3*interval + 2*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (acos) (fun x -> x >= -1.0 && x <= 1.0) "NaN")
  but

let exponentButton = 
  let but = new Button()
  but.Text <- "e^x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (exp) (fun _ -> true) "")
  but

let naturalLogarithmButton = 
  let but = new Button()
  but.Text <- "ln x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(4*interval + 3*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (log) (fun x -> x > 0.0) "NaN")
  but

let squareButton = 
  let but = new Button()
  but.Text <- "x^2"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(5*interval + 4*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (fun x -> x ** 2.0) (fun _ -> true) "")
  but

let squareRootButton = 
  let but = new Button()
  but.Text <- "sqrt x"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(5*interval + 4*buttonWidth, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add(applyUnaryOperator (sqrt) (fun x -> x >= 0.0) "NaN")
  but

type Regime = Dir | Inv
let mutable reg = Dir

let dirButtons = [sinButton; cosButton; exponentButton; squareButton]
let invButtons = [arcsinButton; arccosButton; naturalLogarithmButton; squareRootButton]

let inverseButton = 
  let but = new Button()
  but.Text <- "Inv"
  but.Size <- Size(buttonWidth, buttonHeigth)
  but.Location <- System.Drawing.Point(interval, windowHeight - 5*interval - 5*buttonHeigth)
  but.Click.Add (fun _ -> 
    if reg = Dir then
      for button in dirButtons do button.Visible <- false
      for button in invButtons do button.Visible <- true
      reg <- Inv
    else
      for button in dirButtons do button.Visible <- true
      for button in invButtons do button.Visible <- false
      reg <- Dir)
  but

let backspaceAction args =
  let length = String.length (programInput.Text)
  match length with
  | 0 | 1 -> programInput.Text <- "0"
  | _     -> programInput.Text <- (programInput.Text).Substring(0, (length - 1))

let backspaceButton = 
  let but = new Button()
  but.Text <- "backspace"
  but.Size <- Size(2*buttonWidth + interval, buttonHeigth)
  but.Location <- System.Drawing.Point(interval, windowHeight - 6*interval - 6*buttonHeigth)
  but.Click.Add (backspaceAction)
  but

let mainForm =
  let form = new Form()
  form.BackColor     <- Color.Silver
  form.Cursor        <- Cursors.Hand
  form.ClientSize    <- Size(windowWidth, windowHeight)
  form.Text          <- "Scientific calculator"
  form.Visible       <- true
  form.Font          <- new Font(form.Font.Name, font, form.Font.Style, form.Font.Unit)
  
  for button in invButtons do button.Visible <- false
  form.Controls.Add(programInput)
  for i = 0 to 9 do form.Controls.Add(digitButton i)
  for c in [|'+';'-';'*';'/'|] do form.Controls.Add(binOpButtom c)
  let buttons = [
    decimalMarkButton; equalMarkButton; multiplicativeInverseButton;
    summationInverseButton; sinButton; arcsinButton; cosButton; 
    arccosButton; exponentButton; naturalLogarithmButton; 
    squareButton; squareRootButton; inverseButton; backspaceButton ]
  for button in buttons do form.Controls.Add(button)
  form

[<EntryPoint>]
let main argv = 
  Application.Run() 
  0
