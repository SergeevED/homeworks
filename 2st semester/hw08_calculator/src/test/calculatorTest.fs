module calculatorTest

open calculator

open NUnit.Framework
open System.Drawing
open System.Windows.Forms

[<TestCase("1", '+', "0", Result = "1")>]
[<TestCase("0123", '+', "00456", Result = "579")>]
[<TestCase("123.456", '+', "456.12", Result = "579.576")>]
[<TestCase("1.2", '+', "3", Result = "4.2")>]
[<TestCase("1", '-', "0", Result = "1")>]
[<TestCase("0", '-', "1", Result = "-1")>]
[<TestCase("00456", '-', "0123", Result = "333")>]
[<TestCase("0123", '-', "00456", Result = "-333")>]
[<TestCase("123.456", '-', "456.12", Result = "-332.664")>]
[<TestCase("456.12", '-', "123.456", Result = "332.664")>]
[<TestCase("1", '-', "0.90", Result = "0.1")>]
[<TestCase("1", '*', "0", Result = "0")>]
[<TestCase("0", '*', "1", Result = "0")>]
[<TestCase("00456", '*', "0123", Result = "56088")>]
[<TestCase("123.456", '*', "456.12", Result = "56310.75072")>]
[<TestCase("9999999999", '*', "8888888888", Result = "8.88888888711111E+19")>]
[<TestCase("1", '/', "0", Result = "Division by zero")>]
[<TestCase("0", '/', "1", Result = "0")>]
[<TestCase("232.05", '/', "45.5", Result = "5.1")>]
let TestBinaryOperators operand1 operator operand2 =
  calculator.Clear()
  programInput.Text <- "0"
  for c in operand1 do 
    if c = '.' then decimalMarkAction () else digitAction ((int c) - (int '0')) ()
  binOpAction operator ()
  for c in operand2 do 
    if c = '.' then decimalMarkAction () else digitAction ((int c) - (int '0')) ()
  equalMarkAction ()
  programInput.Text

[<TestCase("8", "1/x", Result = "0.125")>]
[<TestCase("0", "1/x", Result = "Division")>]
[<TestCase("0.25", "1/x", Result = "4")>]
[<TestCase("NaN", "1/x", Result = "NaN")>]
[<TestCase("1.1", "-x", Result = "-1.1")>]
[<TestCase("0", "-x", Result = "0")>]
[<TestCase("NaN", "-x", Result = "NaN")>]
[<TestCase("0", "sin x", Result = "0")>]
[<TestCase("0.5", "sin x", Result = "0.479425")>]
[<TestCase("-2", "sin x", Result = "-0.90929")>]
[<TestCase("1000", "sin x", Result = "0.826879")>]
[<TestCase("NaN", "sin x", Result = "NaN")>]
[<TestCase("0", "cos x", Result = "1")>]
[<TestCase("0.5", "cos x", Result = "0.877582")>]
[<TestCase("-2", "cos x", Result = "-0.41614")>]
[<TestCase("1000", "cos x", Result = "0.562379")>]
[<TestCase("NaN", "cos x", Result = "NaN")>]
[<TestCase("0", "e^x", Result = "1")>]
[<TestCase("0.5", "e^x", Result = "1.648721")>]
[<TestCase("1", "e^x", Result = "2.718281")>]
[<TestCase("-2", "e^x", Result = "0.135335")>]
[<TestCase("NaN", "e^x", Result = "NaN")>]
[<TestCase("0", "x^2", Result = "0")>]
[<TestCase("0.5", "x^2", Result = "0.25")>]
[<TestCase("1", "x^2", Result = "1")>]
[<TestCase("-2", "x^2", Result = "4")>]
[<TestCase("1.1", "x^2", Result = "1.21")>]
[<TestCase("NaN", "x^2", Result = "NaN")>]
[<TestCase("0", "arcsin x", Result = "0")>]
[<TestCase("0.5", "arcsin x", Result = "0.523598")>]
[<TestCase("1", "arcsin x", Result = "1.570796")>]
[<TestCase("-0.4", "arcsin x", Result = "-0.41151")>]
[<TestCase("1.1", "arcsin x", Result = "NaN")>]
[<TestCase("NaN", "arcsin x", Result = "NaN")>]
[<TestCase("0", "arccos x", Result = "1.570796")>]
[<TestCase("0.5", "arccos x", Result = "1.047197")>]
[<TestCase("1", "arccos x", Result = "0")>]
[<TestCase("-0.4", "arccos x", Result = "1.982313")>]
[<TestCase("1.1", "arccos x", Result = "NaN")>]
[<TestCase("NaN", "arccos x", Result = "NaN")>]
[<TestCase("0", "ln x", Result = "NaN")>]
[<TestCase("0.5", "ln x", Result = "-0.69314")>]
[<TestCase("1", "ln x", Result = "0")>]
[<TestCase("10000", "ln x", Result = "9.210340")>]
[<TestCase("-1", "ln x", Result = "NaN")>]
[<TestCase("NaN", "ln x", Result = "NaN")>]
[<TestCase("0", "sqrt x", Result = "0")>]
[<TestCase("0.5", "sqrt x", Result = "0.707106")>]
[<TestCase("123.456", "sqrt x", Result = "11.11107")>]
[<TestCase("1", "sqrt x", Result = "1")>]
[<TestCase("1000", "sqrt x", Result = "31.62277")>]
[<TestCase("-1", "sqrt x", Result = "NaN")>]
[<TestCase("NaN", "sqrt x", Result = "NaN")>]
let TestUnaryOperators operand operation =
  calculator.Clear()
  programInput.Text <- operand
  match operation with
  | "1/x"      -> applyUnaryOperator (1.0 |> (/)) (0.0 |> (<>)) "Division by zero" ()
  | "-x"       -> applyUnaryOperator (-1.0 |> (*)) (fun _ -> true) "" ()
  | "sin x"    -> applyUnaryOperator (sin) (fun _ -> true) "" ()
  | "cos x"    -> applyUnaryOperator (cos) (fun _ -> true) "" ()
  | "e^x"      -> applyUnaryOperator (exp) (fun _ -> true) "" ()
  | "x^2"      -> applyUnaryOperator (fun x -> x ** 2.0) (fun _ -> true) "" ()
  | "arcsin x" -> applyUnaryOperator (asin) (fun x -> x >= -1.0 && x <= 1.0) "NaN" ()
  | "arccos x" -> applyUnaryOperator (acos) (fun x -> x >= -1.0 && x <= 1.0) "NaN" ()
  | "ln x"     -> applyUnaryOperator (log) (fun x -> x > 0.0) "NaN" ()
  | "sqrt x"   -> applyUnaryOperator (sqrt) (fun x -> x >= 0.0) "NaN" ()
  | _          -> ()
  (programInput.Text).Substring(0, (min 8 programInput.Text.Length)) 
  
[<TestCase("1", 3, Result = "0")>]
[<TestCase("NaN", 3, Result = "0")>]
[<TestCase("1", 1, Result = "0")>]
[<TestCase("1.1", 2, Result = "1")>]
let TestBackspace str x =
  calculator.Clear()
  programInput.Text <- str
  for i = 1 to x do backspaceAction ()
  programInput.Text 

