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
