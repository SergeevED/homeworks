module defIntegralTest

open defIntegral

open NUnit.Framework

[<TestCase(0, 1, 1.E-07, 1, Result = "0.459697")>]
[<TestCase(0, 1, 1.E-07, 2, Result = "0.459697")>]
[<TestCase(3, 10, 1.E-07, 2, Result = "-0.15092")>]
[<TestCase(0, 1, 1.E-07, 32, Result = "0.459697")>]
let TestCalculateDefIntegralSmallStep leftEndpoint rightEndpoint step threadNumber =
  (string(calculateDefIntegral sin leftEndpoint rightEndpoint step threadNumber)).Substring(0, 8)

[<TestCase(-1, 0, 0.5, 1, Result = "-0.5")>]
[<TestCase(-1, 0, 0.5, 2, Result = "-0.5")>]
[<TestCase(-1, 0, 0.5, 32, Result = "-0.5")>]
[<TestCase(-1, 0, 1, 1, Result = "-0.5")>]
[<TestCase(-1, 0, 1, 2, Result = "-0.5")>]
[<TestCase(-1, 0, 1, 32, Result = "-0.5")>]
[<TestCase(-1, 0, 2, 1, Result = "-0.5")>]
[<TestCase(-1, 0, 2, 2, Result = "-0.5")>]
[<TestCase(-1, 0, 2, 32, Result = "-0.5")>]
let TestCalculateDefIntegralBigStep leftEndpoint rightEndpoint step threadNumber =
  (string(calculateDefIntegral id leftEndpoint rightEndpoint step threadNumber)).Substring(0, 4)

