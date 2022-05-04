#include <arpa/inet.h>
#include <errno.h>
#include <math.h>
#include <netdb.h>
#include <signal.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <strings.h>
#include <sys/socket.h>
#include <unistd.h>

void PrintResult(int result) {
    if (result == 1) {
        printf("É primo!\n");
    }
    if (result == 0) {
        printf("Não é primo\n");
    }
}

int RandomNumberGenerator(int n) {
    return n + random() % 100 + 1;
}

int main(int arg, char *argv[]) {
    int n = 1;
    int maxIterations = 10;
    int numberOfIterations = 1;

    struct sockaddr_in saddr = {
        .sin_family = AF_INET,
        .sin_addr.s_addr = htonl(INADDR_ANY),
        .sin_port = htons(5000)};

    int server = socket(AF_INET, SOCK_STREAM, 0);
    if (connect(server, (struct sockaddr *)&saddr, sizeof saddr) != 0) {
        printf("Não foi possível estabelecer conexão\n");
        exit(0);
    }
    printf("Conectado!\n");

    char buff[150];

    while (numberOfIterations <= maxIterations) {
        printf("%d: ", numberOfIterations);
        bzero(buff, sizeof buff);
        n = RandomNumberGenerator(n);
        sprintf(buff, "%d", n);
        printf("%s  ", buff);
        write(server, buff, sizeof buff);
        bzero(buff, sizeof buff);
        read(server, buff, sizeof buff);
        PrintResult(atoi(buff));
        numberOfIterations++;
    }
    printf("Saindo do loop!\n");
    n = 0;
    bzero(buff, sizeof buff);
    sprintf(buff, "%d", n);
    write(server, buff, sizeof buff);
    close(server);
    return 0;
}
