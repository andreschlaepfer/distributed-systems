import sys
from datetime import datetime


def validate():

    dir = sys.argv[1]
    n = int(sys.argv[2])
    r = int(sys.argv[3])

    f = open(dir, "r")

    expected_size = n*r

    f = open(dir, "r")
    lines = f.readlines()
    if len(lines) != expected_size:
        raise Exception(
            "Invalid result file: number of lines is different than expected")
    for i in range(len(lines)):
        if lines[i] == lines[-1]:
            break
        [firstT, firstM] = lines[i].split(" ")[-1].split('.')
        [nextT, nextM] = lines[i+1].split(" ")[-1].split('.')
        if diffBetweenTime(firstT, nextT)*1000 + diffBetweenMs(firstM, nextM) < 0:
            raise Exception(
                "Invalid result file: Lines out of order")
    print("Result file was successfully validated")


def diffBetweenTime(fT, lT):
    fT = fT.split(":")
    lT = lT.split(":")
    fT = int(fT[0]) * 3600 + int(fT[1]) * 60 + int(fT[2])
    lT = int(lT[0]) * 3600 + int(lT[1]) * 60 + int(lT[2])
    return lT - fT


def diffBetweenMs(fM, lM):
    return int(lM) - int(fM)


if __name__ == "__main__":
    validate()
