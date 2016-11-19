#!/usr/bin/python
# -*- coding: utf-8 -*-

# LBA to context-sensitive grammar converter
# Usage: python lba_to_csg.py <fair-lba> <out-file>
import sys
import time

start_time = time.time()

fout = open(sys.argv[2], 'w+')


# helpers for writing into the file

def pr1(s=""):
    fout.write("{}\n".format(s))


def pr(arr):
    for s in arr:
        pr1(s)


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
axiom2 = 'A'  # additional state


# debug
# pr(transitions)

# boundary markers are '&' and '#', respectively


def exists_tr(state_before, value_before, state_after, value_after, shift):
    return len([transition for transition in transitions
                if transition == [state_before, value_before, value_after, shift, state_after]]) == 1


# 1:    A1 → [q0, ¢, a, a, $]
pr(["{0} -> [{1},&,{2},{2},#]".format(axiom, init_state, t)
    for t in terminals])
pr1()
print("part 1/9 done")

# 2.1:  [q, ¢, X, a, $] → [¢, p, X, a, $],  if (p, ¢, R) ∈ δ(q, ¢) and q ∈ Q \ F
pr(["[{0},&,{2},{3},#] -> [&,{1},{2},{3},#]".format(q, p, x, a)
    for q in non_finite_states
    for p in non_finite_states
    for x in tape_symbols
    for a in terminals
    if exists_tr(q, '&', p, '&', 'r')])
# 2.2:  [¢, q, X, a, $] → [p, ¢, Y, a, $],  if (p, Y, L) ∈ δ(q, X) and q ∈ Q \ F
pr(["[&,{0},{2},{4},#] -> [{1},&,{3},{4},#]".format(q, p, x, y, a)
    for q in non_finite_states
    for p in non_finite_states
    for x in tape_symbols
    for y in tape_symbols
    for a in terminals
    if exists_tr(q, x, p, y, 'l')])
# 2.3:  [¢, q, X, a, $] → [¢, Y, a, p, $],  if (p, Y, R) ∈ δ(q, X) and q ∈ Q \ F
pr(["[&,{0},{2},{4},#] -> [&,{3},{4},{1},#]".format(q, p, x, y, a)
    for q in non_finite_states
    for p in non_finite_states
    for x in tape_symbols
    for y in tape_symbols
    for a in terminals
    if exists_tr(q, x, p, y, 'r')])
# 2.4:  [¢, X, a, q, $] → [¢, p, X, a, $],  if (p, $, L) ∈ δ(q, $) and q ∈ Q \ F
pr(["[&,{2},{4},{0},#] -> [&,{1},{2},{4},#]".format(q, p, x, y, a)
    for q in non_finite_states
    for p in non_finite_states
    for x in tape_symbols
    for y in tape_symbols
    for a in terminals
    if exists_tr(q, '#', p, '#', 'l')])
pr1()
print("part 2/9 done")

# 3.1:  [q, ¢, X, a, $] → a, q ∈ F
pr(["[{0},&,{1},{2},#] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 3.2:  [¢, q, X, a, $] → a, q ∈ F
pr(["[&,{0},{1},{2},#] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 3.3:  [¢, X, a, q, $] → a, q ∈ F
pr(["[&,{1},{2},{0},#] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
pr1()
print("part 3/9 done")

# 4.1:  A1 → [q0, ¢, a, a] A2
pr(["{0} -> [{1},&,{2},{2}]{3}".format(axiom, init_state, a, axiom2)
    for a in terminals])
# 4.2:  A2 → [a, a] A2
pr(["{0} -> [{1},{1}]{0}".format(axiom2, a)
    for a in terminals])
# 4.3:  A2 → [a, a, $]
pr(["{0} -> [{1},{1},#]".format(axiom2, a)
    for a in terminals])
pr1()
print("part 4/9 done")

# 5.1:  [q, ¢, X, a] → [¢, p, X, a],                if (p, ¢, R) ∈ δ(q, ¢) and q ∈ Q \ F
pr(["[{0},&,{2},{3}] -> [&,{1},{2},{3}]".format(q, p, x, a)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for a in terminals
    if exists_tr(q, '&', p, '&', 'r')])
# 5.2:  [¢, q, X, a] → [p, ¢, Y, a],                if (p, Y, L) ∈ δ(q, X) and q ∈ Q \ F
pr(["[&,{0},{2},{4}] -> [{1},&,{3},{4}]".format(q, p, x, y, a)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for a in terminals
    if exists_tr(q, x, p, y, 'l')])
# 5.3:  [¢, q, X, a] [Z, b] → [¢, Y, a] [p, Z, b],  if (p, Y, R) ∈ δ(q, X) and q ∈ Q \ F
pr(["[&,{0},{2},{5}][{4},{6}] -> [&,{3},{5}][{1},{4},{6}]".format(q, p, x, y, z, a, b)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for z in tape_symbols
    for a in terminals
    for b in terminals
    if exists_tr(q, x, p, y, 'r')])
pr1()
print("part 5/9 done")

# 6.1:  [q, X, a] [Z, b] → [Y, a] [p, Z, b],        if (p, Y, R) ∈ δ(q, X) and q ∈ Q \ F
pr(["[{0},{2},{5}][{4},{6}] -> [{3},{5}][{1},{4},{6}]".format(q, p, x, y, z, a, b)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for z in tape_symbols
    for a in terminals
    for b in terminals
    if exists_tr(q, x, p, y, 'r')])
# 6.2:  [Z, b] [q, X, a] → [p, Z, b] [Y, a],        if (p, Y, L) ∈ δ(q, X) and q ∈ Q \ F
pr(["[{4},{6}][{0},{2},{5}] -> [{1},{4},{6}][{3},{5}]".format(q, p, x, y, z, a, b)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for z in tape_symbols
    for a in terminals
    for b in terminals
    if exists_tr(q, x, p, y, 'l')])
# 6.3:  [q, X, a] [Z, b, $] → [Y, a] [p, Z, b, $],  if (p, Y, R) ∈ δ(q, X) and q ∈ Q \ F
pr(["[{0},{2},{5}][{4},{6},#] -> [{3},{5}][{1},{4},{6},#]".format(q, p, x, y, z, a, b)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for z in tape_symbols
    for a in terminals
    for b in terminals
    if exists_tr(q, x, p, y, 'r')])
pr1()
print("part 6/9 done")

# 7.1:  [q, X, a, $] → [Y, a, p, $],                if (p, Y, R) ∈ δ(q, X) and q ∈ Q \ F
pr(["[{0},{2},{4},#] -> [{3},{4},{1},#]".format(q, p, x, y, a)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for a in terminals
    if exists_tr(q, x, p, y, 'r')])
# 7.2:  [X, a, q, $] → [p, X, a, $],                if (p, $, L) ∈ δ(q, $) and q ∈ Q \ F
pr(["[{2},{3},{0},#] -> [{1},{2},{3},#]".format(q, p, x, a)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for a in terminals
    if exists_tr(q, '#', p, '#', 'l')])
# 7.3:  [Z, b] [q, X, a, $] → [p, Z, b] [Y, a, $],  if (p, Y, L) ∈ δ(q, X) and q ∈ Q \ F
pr(["[{4},{6}][{0},{2},{5},#] -> [{1},{4},{6}][{3},{5},#]".format(q, p, x, y, z, a, b)
    for q in non_finite_states
    for p in all_states
    for x in tape_symbols
    for y in tape_symbols
    for z in tape_symbols
    for a in terminals
    for b in terminals
    if exists_tr(q, x, p, y, 'l')])
pr1()
print("part 7/9 done")

# 8.1:  [q, ¢, X, a] → a,   if q ∈ F
pr(["[{0},&,{1},{2}] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 8.2:  [¢, q, X, a] → a,   if q ∈ F
pr(["[&,{0},{1},{2}] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 8.3:  [q, X, a] → a,      if q ∈ F
pr(["[{0},{1},{2}] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 8.4:  [q, X, a, $] → a,   if q ∈ F
pr(["[{0},{1},{2},#] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
# 8.5:  [X, a, q, $] → a,   if q ∈ F
pr(["[{1},{2},{0},#] -> {2}".format(q, x, a)
    for q in finite_states
    for x in tape_symbols
    for a in terminals])
pr1()
print("part 8/9 done")

# 9.1:  a[X, b] → ab
pr(["{1}[{0},{2}] -> {1}{2}".format(x, a, b)
    for x in tape_symbols
    for a in terminals
    for b in terminals])
# 9.2:  a[X, b, $] → ab
pr(["{1}[{0},{2},#] -> {1}{2}".format(x, a, b)
    for x in tape_symbols
    for a in terminals
    for b in terminals])
# 9.3:  [X, a]b → ab
pr(["[{0},{1}]{2} -> {1}{2}".format(x, a, b)
    for x in tape_symbols
    for a in terminals
    for b in terminals])
# 9.4:  [¢, X, a]b → ab
pr(["[&,{0},{1}]{2} -> {1}{2}".format(x, a, b)
    for x in tape_symbols
    for a in terminals
    for b in terminals])
print("part 9/9 done")

fout.close()
print("Execution time: {} seconds".format(time.time() - start_time))
