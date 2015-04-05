type peano = Zero | S of peano

let rec plus a b =
  match a with
  | Zero -> b
  | S a  -> S (plus a b)
  
let rec minus a b =
  match b with
  | Zero -> a
  | S b -> 
    match a with
      | Zero -> Zero
      | S a  -> (minus a b)

let rec peanoIntoInt a =
  match a with
  | Zero -> 0
  | S a  -> (peanoIntoInt a) + 1  
  
let rec multiply a b =
  match b with
  | Zero -> Zero
  | S b  -> (plus a (multiply a b))

let rec raising a b =
  match b with
  | Zero -> S Zero
  | S b  -> (multiply a (raising a b))