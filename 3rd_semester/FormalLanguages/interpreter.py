grammar = []
input = '[B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B]s0[1,1][0,0][1,1][1,1][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B][B,B]'
log = []


def find_rule():
    for rule in grammar:
        x = input.find(rule[0])
        if x != -1:
            return (rule, x)
    return None


for line in open('Grammar.txt'):
    l, r = line.split(" -> ")
    grammar.append((l.strip(), r.strip()))

while True:
    log.append(input + '\n')
    r = find_rule()
    if r is None:
        break
    else:
        (rule, index) = r
        input = input.replace(rule[0], rule[1], 1)
        log.append('Apply {} -> {}:\n'.format(rule[0], rule[1]))

open('rules_for_1011.txt', 'w').writelines(log)
