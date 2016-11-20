MT.txt --- transition rules of Turing machine which checks whether input binary number is prime

Grammar.txt --- production rules of type 0 grammar which produces language of prime numbers

rules_for_1011.txt --- derivation of (binary) prime number 1011

converter.py --- produces type 0 production grammar rules by TM transition rules

interpreter.py --- produces derivation of given number using grammar production rules

Steps:

1) python converter.py
2) python interpreter.py input.txt grammar.txt rules_for_1011.txt

Note: derivation starts not from the start terminal, but from some further step at which all 'starting' symbols and blank symbols are already generated (see `input.txt`). Derivation of that state is simple from following generation rules:

S -> [B,B]S
S -> S[B,B]
S -> init
init -> init[0,0]
init -> init[1,1]
init -> s0

The automatic derivation starts from this point in order to avoid "forking", which is present during the skipped step: the grammar can produce endless chains of [0,0] and [1,1] for "start words".
