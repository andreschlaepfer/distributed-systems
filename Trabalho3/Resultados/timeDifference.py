import os 

def process():
    dir = os.path.dirname(os.path.realpath(__file__))
    f = open(dir + "/test_3/n=8/resultado.txt", "r")
    lines = f.readlines()

    firstLine = lines[0]
    [firstT, firstM] = firstLine.split(" ")[-1].split('.')

    lastLine = lines[-1]
    [lastT, lastM] = lastLine.split(" ")[-1].split('.')

    print(diffBetweenTime(firstT, lastT)*1000 + diffBetweenMs(firstM, lastM))


def diffBetweenTime(fT, lT):
    fT = fT.split(":")
    lT = lT.split(":")
    fT = int(fT[0]) * 3600 + int(fT[1]) * 60 + int(fT[2])
    lT = int(lT[0]) * 3600 + int(lT[1]) * 60 + int(lT[2])
    return lT - fT

def diffBetweenMs(fM, lM):
    return int(lM) - int(fM)
    

if __name__ == "__main__":
    process()