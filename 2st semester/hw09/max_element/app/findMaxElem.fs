module findMaxElem

open System.Threading

let maxInRange (arr : 'T []) l r =
  let mutable res = arr.[0]
  for i in l .. r do
    if arr.[i] > res then res <- arr.[i]
  res

let getMax (arr : 'T array) threadNumber = 
  if threadNumber < 1 then 
    invalidArg "threadNumber" "Thread number must be a natural number"
  if arr.Length = 0 then 
    invalidArg "arr" "Empty array has no maximum elements"
  let threadNumber = min arr.Length threadNumber   
  let step = arr.Length / threadNumber
  let res = ref (maxInRange arr (arr.Length - step) (arr.Length - 1))
  let threadArray = Array.init threadNumber (fun i ->
    new Thread(ThreadStart(fun _ ->
      let threadRes = maxInRange arr (i * step) ((i + 1) * step - 1)
      Monitor.Enter(res)
      if threadRes > !res then res := threadRes
      Monitor.Exit(res)
    )
  ))
  for t in threadArray do
    t.Start()
  for t in threadArray do
    t.Join()
  res.Value
