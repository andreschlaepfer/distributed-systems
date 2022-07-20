import os 

def validate():
    dir = os.path.dirname(os.path.realpath(__file__))

    f = open(dir + "/log.txt", "r")
    r = open(dir +"/resultado.txt", "r")
    rLines = r.readlines()
    lines = f.readlines()
    requests = []
    grants = []
    releases = []

    for line in lines:
        if ("[R] Request" in line):
            requests.append(int(line.split("-")[1]))
            continue
        if ("[S] Grant" in line):
            if (len(grants) != len(releases)):
                print(line)
                print(len(grants))
                print(len(releases))
                raise Exception(
                    "1 Invalid log file: invalid grants and releases sequence")
            grants.append(int(line.split("-")[1]))
            continue
        if ("[R] Release" in line):
            if (len(releases) != len(grants) - 1):
                raise Exception(
                    "2 Invalid log file: invalid grants and releases sequence")
            releases.append(int(line.split("-")[1]))
            continue

    for i in range(len(requests)):
        if (requests[i] != grants[i] or grants[i] != releases[i]):
            raise Exception(
                "3 Invalid log file: invalid grants and releases sequence")

    if(len(rLines) == 1280):
        print("Log file was successfully validated")


if __name__ == "__main__":
    validate()
