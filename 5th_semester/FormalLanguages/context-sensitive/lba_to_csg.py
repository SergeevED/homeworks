# LBA to context-sensitive grammar converter
# Usage: python lba_to_csg.py <fair-lba> <out-file>
import sys

fout = open(sys.argv[2], 'w+')


def pr(prods):
    for p in prods:
        fout.write("{}\n".format(p))


terminals = "01"
tape_symbols = "01234567"
transitions = [x.strip("\n").split()
               for x in open(sys.argv[1], 'r').readlines()
               if not (x.startswith(";") or x.startswith("\n"))]
init_state = "s100"
all_states = set([tr[0] for tr in transitions] + [tr[4] for tr in transitions])
finite_states = {"HALT-ACCEPT", "HALT-REJECT"}
non_finite_states = all_states - finite_states
axiom = 'S'  # start non-terminal


# boundary markers are '&' and '#', respectively


def exists_tr(state_before, value_before, state_after, value_after, shift):
    return len([transition for transition in transitions
                if transition == [state_before, value_before, value_after, state_after, shift]]) == 1


# initial
pr(["{0} -> [{1},&,{2},{2},#]".format(axiom, init_state, t)
    for t in terminals])

# movement over one-character word on tape
pr(["[{0},&,{2},{3},#] -> [&,{1},{2},{3},#]".format(q, p, x, a)
    for q in non_finite_states
    for p in non_finite_states
    for x in tape_symbols
    for a in terminals])

fout.close()
