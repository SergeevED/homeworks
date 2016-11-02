module ModularBuilderTest

open ModularBuilder

open NUnit.Framework

[<Test>]
let TestModularBuilderDiv() =
  let d = ModularBuilder(9) 
  let t = 
    d {
      let! a = 5 * 3 + 9
      let! b = a |> divBy 2
      return b
  }
  Assert.AreEqual (Some 3, t)

[<Test>]
let TestModularBuilderMod() =
  let d = ModularBuilder(11) 
  let t = 
    d {
      let! a = 5 * 3 + 9
      let! b = a |> modBy 5
      return b
  }
  Assert.AreEqual (Some 2, t)

[<Test>]
let TestModularReturn() =
  let d = ModularBuilder(5) 
  let t = 
    d {
      return 6
  }
  Assert.AreEqual (Some 1, t)

[<Test>]
let TestModularDivByZero() =
  let d = ModularBuilder(5) 
  let t = 
    d {
      return! 1 |> divBy 0
  }
  Assert.AreEqual (None, t)

[<Test>]
let TestModularModByZero() =
  let d = ModularBuilder(5) 
  let t = 
    d {
      return! 1 |> modBy 0
  }
  Assert.AreEqual (None, t)
