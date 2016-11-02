tm_code_file = open('TM.txt', encoding='utf8')
grammar_code_file = open('Grammar.txt', 'w', encoding='utf8')

tm_tape_alphabet = ['0', '1', 'B', 'Z', 'U', '%', '#', '@', '&', '$']

tm_input_alphabet = ['0', '1', 'B']

grammar_symbols = ['[{},{}]'.format(input, tape) for input in tm_input_alphabet for tape in tm_tape_alphabet]


# Returns grammar rules which product input Turing machine tape (blank symbols, initial state right before
# observed symbol and entry symbols) from the axiom.
def generate_initial_rules():
    return ["S -> [B,B]S\n",
            "S -> S[B,B]\n",
            "S -> init\n"
            "init -> init[0,0]\n",
            "init -> init[1,1]\n",
            "init -> s0\n"]


# Returns grammar rules produced by one Turing machine transition rule.
def generate_rules(tm_rule):
    cur_state, cur_letter, next_letter, direction, next_state = tm_rule

    if direction == 'l':
        rules = ["{}{}[{},{}] -> {}{}[{},{}]\n".format(
            gr_symbol, cur_state, input_symbol, cur_letter, next_state, gr_symbol, input_symbol, next_letter)
                 for gr_symbol in grammar_symbols for input_symbol in tm_input_alphabet]
    elif direction == 'r':
        rules = ["{}[{},{}] -> [{},{}]{}\n".format(
            cur_state, input_symbol, cur_letter, input_symbol, next_letter, next_state)
                 for input_symbol in tm_input_alphabet]
    else:
        raise ValueError('Incorrect Turing machine direction', direction)

    return rules


# Returns grammar rules which transform Turing machine tape into entry symbols. These rules are applied
# when Turing machine is in accepting state.
def generate_finishing_rules():
    rules = []
    for symbol in grammar_symbols:
        tm_input_symbol = symbol[1]
        new_symbols = 'F' if tm_input_symbol == 'B' else 'F' + tm_input_symbol + 'F'
        rules.append("F{} -> {}\n".format(symbol, new_symbols))
        rules.append("{}F -> {}\n".format(symbol, new_symbols))

    rules.append("F -> \n")
    return rules


grammar = []

for line in tm_code_file:
    if not line or line == '\n':
        continue

    line = line.replace('_', 'B')
    line = line.replace('HALT-ACCEPT', 'F')

    grammar_rules = generate_rules(line.split())

    for rule in grammar_rules:
        grammar.append(rule)

grammar_code_file.writelines(generate_initial_rules() + grammar + generate_finishing_rules())

tm_code_file.close()
grammar_code_file.close()
