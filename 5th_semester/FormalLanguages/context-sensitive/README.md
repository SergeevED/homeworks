### Context-sensitive grammar for prime numbers language

Alphabet: `{'0', '1'}`

Steps:

1. Convert LBA with syntax sugar to fair LBA

       python to_fair_lba.py lba.txt fair_lba.txt

2. You may test the LBA [on a website](http://morphett.info/turing/turing.html).

       Add `0 * * * s100` at the top of the input field.

3. Translate LBA to a CSG

       python lba_to_csg.py fair_lba.txt grammar.txt

4. Generate derivation of binary prime number 1011

       python ../interpreter.py input.txt grammar.txt rules_for_1011.txt


Note: derivation starts not from the start terminal, but from some further step at which all 'starting' symbols are already generated (see `input.txt`). Derivation of that state is simple. Example for `1011`:

       S
       -> [s100,&,1,1]D
       -> [s100,&,1,1][0,0]D
       -> [s100,&,1,1][0,0][1,1]D
       -> [s100,&,1,1][0,0][1,1][1,1,#]

See parts 4.1-4.3 of `lba_to_csg.py` for the rules used at this point. The automatic derivation starts from this point in order to avoid "forking", which is present during the skipped step: the grammar can produce endless chains of [0,0] and [1,1] for "start words".
