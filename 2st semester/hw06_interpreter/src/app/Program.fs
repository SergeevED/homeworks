module Program

open Interpreter


[<EntryPoint>]
let main args =  
  let interpr = Interpreter()
  printf "
  Example of program:
  read(x);
  read(y);
  while(x) {
    write(y);
    x := x - 1
  }\n"  

  let program = ";\nread\nx\n;\nread\ny\nwhile\nx\n;\nwrite\ny\n:=\nx\n-\nx\n1\n"
  let mutable z = [""]
  let mutable c = "run"
  while c <> "exit" do
    printf "Enter 'run' to run this program or 'exit' to exit\n" 
    c <- System.Console.ReadLine()
    if c = "run" then z <- interpr.Interpretate program [] true

  0 