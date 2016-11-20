# produces derivation of given number using grammar production rules
# Usage: python interpreter.py <input> <grammar> <rules_for_1011>

import sys

grammar = []
input = open(sys.argv[1], 'r').readline()
log = open(sys.argv[3], 'w+')


def find_rule():
    for rule in grammar:
        if input.find(rule[0]) != -1:
            return rule
    return None


for line in open(sys.argv[2], 'r'):
    if line.find('->') != -1:
        l, r = line.split(" -> ")
        grammar.append((l.strip(), r.strip()))

while True:
    log.write(input + '\n')
    rule = find_rule()
    if rule is None:
        break
    else:
        input = input.replace(rule[0], rule[1], 1)
        log.write('Apply {} -> {}:\n'.format(rule[0], rule[1]))

#open(sys.argv[3], 'w').writelines(log)
