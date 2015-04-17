module ModularBuilder

let divBy a b = 
  if a = 0 then None else Some(b / a)
  
let modBy a b = 
  if a = 0 then None else Some(b % a)

let getResidue x n =                          //returns residue of the integer x modulo n
  match (x % n) with
  | x when x < 0  -> x + n
  | x -> x

type ModularBuilder (n : int) =
  member this.Return x = Some(getResidue x n)
  member this.ReturnFrom x = 
    match x with
    | None -> None
    | Some x -> Some(getResidue x n)
  member this.Bind(m , f) = 
    match m with
    | None -> None
    | Some x -> f (getResidue x n) 
  member this.Bind(x , f) =
    f (getResidue x n)
