# Usage: python to_fair_lba.py <in-file> <out-file>
import re
import sys

lines_in = open(sys.argv[1], 'r')
fout = open(sys.argv[2], 'w+')

fout.write("""; Fair LBA generated with <3
; Alphabet: {'0'..'7', '&', '#')
; Input alphabet: {'0', '1'}, where '0' == 000, '1' == 100\n\n""")

alphabet = {
    "&": "&",
    "#": "#",
    "000": "0",
    "100": "1",
    "010": "2",
    "110": "3",
    "001": "4",
    "101": "5",
    "011": "6",
    "111": "7"
}


def cmp_alphabet(a, b):
    return cmp(a[::-1], b[::-1])


# noinspection PyShadowingNames
def gen_right_side(left_side, given_right_side):
    right_side = given_right_side
    if right_side == "-":
        right_side = left_side
    else:
        for i in range(3):
            if right_side[i] == '.':
                right_side = right_side[:i] + left_side[i] + right_side[(i + 1):]
    return right_side


for raw_line in lines_in:
    if raw_line.startswith(";") or raw_line.startswith("\n"):
        fout.write(raw_line)
        continue

    prod = raw_line.strip("\n").split()

    pattern = re.compile("^{}$".format(prod[1]))
    left_sides = [x for x in alphabet if pattern.match(x)]
    left_sides.sort(cmp_alphabet)

    for left_side in left_sides:
        right_side = gen_right_side(left_side, prod[2])
        # before "merging" the tracks
        print("{} {} {} {} {}".format(
            prod[0], left_side, right_side,
            prod[3], prod[4]
        ))
        # merge the tracks
        left_side_short = alphabet[left_side]
        right_side_short = alphabet[right_side]
        fout.write("{} {} {} {} {}\n".format(
            prod[0], left_side_short, right_side_short,
            prod[3], prod[4]
        ))

fout.close()
print 'ok'
