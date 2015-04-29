module defIntegral

open System.Threading

let areaOfTrapezium (f : double -> double) a b =
  (f(a) + f(b)) * abs(b - a) / 2.0 

let findArea (f : double -> double) leftEndpoint rightEndpoint step =
  let numberOfSteps = int ((rightEndpoint - leftEndpoint) / step)
  let mutable res = areaOfTrapezium f (leftEndpoint + step * double numberOfSteps) (rightEndpoint) 
  for i = 1 to numberOfSteps do
    let area = areaOfTrapezium f (leftEndpoint + step * double (i - 1)) (leftEndpoint + step * double i)  
    res <- res + area
  res

let calculateDefIntegral (f : double -> double) leftEndpoint rightEndpoint step threadNumber = 
  if threadNumber < 1 then 
    invalidArg "threadNumber" "Thread number must be a natural number"
  if step <= 0.0 then 
    invalidArg "step" "Step must be greater than zero"
  if leftEndpoint > rightEndpoint then 
    invalidArg "interval" "Left endpoint must not be greater than right endpoint"  
  let intervalForThread = (rightEndpoint - leftEndpoint) / double threadNumber
  let res = ref 0.0
  let threadArray = Array.init threadNumber (fun i ->
    new Thread(ThreadStart(fun _ ->
      let threadRes = 
        findArea f (leftEndpoint + double i * intervalForThread) (leftEndpoint + (double i + 1.0) * intervalForThread) step
      Monitor.Enter(res)
      res := !res + threadRes
      Monitor.Exit(res)
    )
  ))
  for t in threadArray do
    t.Start()
  for t in threadArray do
    t.Join()
  res.Value
