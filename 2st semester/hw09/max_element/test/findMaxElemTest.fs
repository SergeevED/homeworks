module findMaxElemTest

open NUnit.Framework

open findMaxElem

[<TestCase([|1;3;5;4;2|], 1, Result = 5)>]
[<TestCase([|1;3;5;4;2|], 2, Result = 5)>]
[<TestCase([|1;3;5;4;2|], 3, Result = 5)>]
[<TestCase([|1;3;5;4;2|], 4, Result = 5)>]
[<TestCase([|1;3;5;4;2|], 5, Result = 5)>]
[<TestCase([|1;3;5;4;2|], 6, Result = 5)>]
[<TestCase([|0;5;10;15;20|], 1, Result = 20)>]
[<TestCase([|0;5;10;15;20|], 2, Result = 20)>]
[<TestCase([|20;15;10;5;0|], 1, Result = 20)>]
[<TestCase([|20;15;10;5;0|], 2, Result = 20)>]
[<TestCase([|-1;-12;-5;0;-7;-4|], 2, Result = 0)>]
[<TestCase([|441;540;-2101;4332;-9671;3731;1218;4047;7928;8418;-1430;-6185;29;
  -3745;222;3441;-5168;-9048;-1734;2565;2544;-8978;9091;7210;-5707;-6627;3359; 
  -2759;-6233;1399;-4653;-5644;-281;-8223;-3869;-2654;6916;8601;8218;5189;-4814;
  200;1954;2168;8623;9270;6337;-2119;-5984;-7030|], 8, Result = 9270)>]
let TestGetMax arr n =
  int (getMax arr n)
