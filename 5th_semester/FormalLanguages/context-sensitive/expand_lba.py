# expands a single-track LBA tape state into multi-track tape state
import sys

if len(sys.argv) < 2:
    print("Usage: python expand_lba.py <single-track tape state>")
    sys.exit(0)

dict = {
    "&": "&  ",
    "#": "#  ",
    "0": "000",
    "1": "100",
    "2": "010",
    "3": "110",
    "4": "001",
    "5": "101",
    "6": "011",
    "7": "111"
}

for i in range(3):
    for c in sys.argv[1]:
        print(dict[c][i]),
    print("")
