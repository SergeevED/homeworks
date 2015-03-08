module fold

// List.iter : ('T -> unit) -> 'T list -> unit 

let rec fold f acc list =
  match list with
  | [] -> acc
  | elem :: list -> fold f (f acc elem) list

let rec foldBack f list acc =
  match list with
  | [] -> acc
  | elem :: list -> f elem (foldBack f list acc)

let reverseList list = fold (fun acc elem -> elem :: acc) [] list

let filterFold f list = foldBack (fun elem acc -> if (f elem) = true then elem :: acc else acc) list []

let mapFold f list = foldBack (fun elem acc -> (f elem) :: acc) list []

let horner x list = 
  match list with
  | [] -> 0
  | elem :: list -> fold (fun acc elem -> acc * x + elem ) elem list

[<EntryPoint>]
let main args =
  printf "List.iter : ('T -> unit) -> 'T list -> unit\n"

  let list = [ 1; 2; 3; 4; 5; 6; 7 ]
  printf "list "
  list |> List.iter (fun x -> printf "%d " x)
  printf "\n"

  let reversedList = reverseList list
  printf "reversed list "
  reversedList |> List.iter (fun x -> printf "%d " x)
  printf "\n"

  let filteredList = filterFold (fun elem -> if elem % 2 = 0 then true else false) list
  printf "filtered list (even) "
  filteredList |> List.iter (fun x -> printf "%d " x)
  printf "\n"

  let mappedList = mapFold (fun elem -> elem + 1) list
  printf "mapped list (incremention) "
  mappedList |> List.iter (fun x -> printf "%d " x)
  printf "\n"

  let x = 2
  let coef = [ 0; 3; 7; -5]
  let y = horner x coef
  printf "x = %d, 0*x^3 + 3*x^2 + 7*x -5 = %d\n" x y
  
  0