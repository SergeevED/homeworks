module hw
open NUnit.Framework

type Computer = {
  number : int; 
  os : string;
  infectedChance : int;
  mutable isInfected : bool;
}

type LAN(V : Computer[] , M : bool[,]) =
  class
    let size = V.Length
    let vertices = V 
    let adjMatrix = M
    let isMarked = Array.zeroCreate<bool> size
    member this.TryInfect() =  
      let random = System.Random()
      let rec f i j list =
        match i, j with
        | i, _ when i >= size -> list
        | _, j when j >= size -> f (i+1) 0 list
        | i, j ->
          if (adjMatrix.[i,j] = true) && (vertices.[i].isInfected) && 
          (this.GetRandom() > 100 - vertices.[j].infectedChance) 
            then j :: (f i (j+1) list) 
            else f i (j+1) list
      let infected = f 0 0 []
      List.iter (fun x -> vertices.[x].isInfected <- true) infected 
    member this.Show() =
      for i = 0 to size - 1 do 
        printf "Computer %d : " vertices.[i].number
        if vertices.[i].isInfected then printf "infected\n" else printf "not infected\n" 
    member this.GetInfected() =
       Array.filter (fun x -> x.isInfected) vertices
    abstract GetRandom: unit -> int
    default this.GetRandom() =
      let random = System.Random() 
      random.Next(100)
    end


[<Test>]
let LAN01() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 100; isInfected = false} 
              { number = 2; os = "2"; infectedChance = 100; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 100; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 100; isInfected = true} 
              { number = 5; os = "5"; infectedChance = 100; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 true
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 0
  }
  for i = 1 to 10 do lan.TryInfect()
  let ans = [| { number = 4; os = "4"; infectedChance = 100; isInfected = true} |]
  Assert.AreEqual(ans, lan.GetInfected() ) 

[<Test>]
let LAN02() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 0; isInfected = false} 
              { number = 2; os = "2"; infectedChance = 0; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 0; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 0; isInfected = true} 
              { number = 5; os = "5"; infectedChance = 0; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 true
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 10 do lan.TryInfect()
  let ans = [| { number = 4; os = "4"; infectedChance = 0; isInfected = true} |]
  Assert.AreEqual(ans, lan.GetInfected() )

[<Test>]
let LAN03() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 100; isInfected = false} 
              { number = 2; os = "2"; infectedChance = 100; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 100; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 100; isInfected = false} 
              { number = 5; os = "5"; infectedChance = 100; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 true
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 10 do lan.TryInfect()
  Assert.AreEqual([||], lan.GetInfected() )

[<Test>]
let LAN04() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 1; isInfected = true} 
              { number = 2; os = "2"; infectedChance = 1; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 1; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 1; isInfected = false} 
              { number = 5; os = "5"; infectedChance = 1; isInfected = false} 
              { number = 6; os = "6"; infectedChance = 1; isInfected = false} 
              { number = 7; os = "7"; infectedChance = 1; isInfected = false} 
              { number = 8; os = "8"; infectedChance = 1; isInfected = false} 
              { number = 9; os = "9"; infectedChance = 1; isInfected = false} 
              { number = 10; os = "10"; infectedChance = 1; isInfected = false} 
           |]
  let matrix = Array2D.create 10 10 false
  matrix.[0,1] <- true; matrix.[1,2] <- true; matrix.[2,3] <- true; 
  matrix.[3,4] <- true; matrix.[4,5] <- true; matrix.[5,6] <- true;
  matrix.[6,7] <- true; matrix.[7,8] <- true; matrix.[8,9] <- true;
  matrix.[9,0] <- true;
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 9 do lan.TryInfect()
  Assert.AreEqual(l, lan.GetInfected() )

[<Test>]
let LAN05() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 100; isInfected = true} 
              { number = 2; os = "2"; infectedChance = 100; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 100; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 100; isInfected = false} 
              { number = 5; os = "5"; infectedChance = 100; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 false
  matrix.[1,2] <- true; matrix.[2,3] <- true; matrix.[3,4] <- true; 
  matrix.[4,1] <- true;
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 10 do lan.TryInfect()
  let ans = [| { number = 1; os = "1"; infectedChance = 100; isInfected = true} |] 
  Assert.AreEqual(ans, lan.GetInfected() )

[<Test>]
let LAN06() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 100; isInfected = true} 
              { number = 2; os = "2"; infectedChance = 100; isInfected = false} 
              { number = 3; os = "3"; infectedChance = 100; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 100; isInfected = false} 
              { number = 5; os = "5"; infectedChance = 100; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 false
  matrix.[0,1] <- true; matrix.[0,3] <- true; matrix.[1,2] <- true; 
  matrix.[2,3] <- true; matrix.[3,4] <- true; matrix.[4,1] <- true;
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  lan.TryInfect()
  let ans = [| 
    { number = 1; os = "1"; infectedChance = 100; isInfected = true}; 
    { number = 2; os = "2"; infectedChance = 100; isInfected = true};
    { number = 4; os = "4"; infectedChance = 100; isInfected = true}
  |] 
  Assert.AreEqual(ans, lan.GetInfected() )

[<Test>]
let LAN07() =
  let l = [|
              { number = 1; os = "1"; infectedChance = 100; isInfected = false} 
              { number = 2; os = "2"; infectedChance = 100; isInfected = true} 
              { number = 3; os = "3"; infectedChance = 100; isInfected = false} 
              { number = 4; os = "4"; infectedChance = 100; isInfected = false} 
              { number = 5; os = "5"; infectedChance = 100; isInfected = false} 
           |]
  let matrix = Array2D.create 5 5 false
  matrix.[0,1] <- true; matrix.[0,2] <- true; matrix.[0,3] <- true; 
  matrix.[0,4] <- true; matrix.[1,2] <- true; matrix.[2,3] <- true;
  matrix.[3,4] <- true; matrix.[4,1] <- true;
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 10 do lan.TryInfect()
  let ans = [| 
    { number = 2; os = "2"; infectedChance = 100; isInfected = true}; 
    { number = 3; os = "3"; infectedChance = 100; isInfected = true}; 
    { number = 4; os = "4"; infectedChance = 100; isInfected = true}; 
    { number = 5; os = "5"; infectedChance = 100; isInfected = true}; 
  |] 
  Assert.AreEqual(ans, lan.GetInfected() )

[<Test>]
let LAN08() =
  let l = [||]          
  let matrix = Array2D.create 0 0 false
  let lan = { new LAN(l, matrix) with
      override this.GetRandom() = 100
  }
  for i = 1 to 10 do lan.TryInfect()
  Assert.AreEqual([||], lan.GetInfected() )



[<EntryPoint>]
let main argv =

  0