module ModularBuilder

let divBy a b = 
  if a = 0 then None else Some(b / a)
  
let modBy a b = 
  if a = 0 then None else Some(b % a)

type ModularBuilder (n : int) =
  member this.Return x = Some (x % n)
  member this.ReturnFrom x = 
    match x with
    | None -> None
    | Some x -> Some (x % n)
  member this.Bind(m , f) = 
    match m with
    | None -> None
    | Some x -> f (x % n) 
  member this.Bind(x , f) =
    f (x % n)
