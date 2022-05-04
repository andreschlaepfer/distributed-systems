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

int CheckPrime(int n) {
    if (n <= 1) {
        return 0;
    }
    for (int i = 2; i <= sqrt((double)n); i++) {
        if (n % i == 0) {
            return 0;
        }
    }
    return 1;
}

int main(int arg, char *argv[]) {
    struct sockaddr_in caddr;
    struct sockaddr_in saddr = {
        .sin_family = AF_INET,
        .sin_addr.s_addr = htonl(INADDR_ANY),
        .sin_port = htons(5000)};

    int server = socket(AF_INET, SOCK_STREAM, 0);
    bind(server, (struct sockaddr *)&saddr, sizeof saddr);
    listen(server, 5);

    int csize = sizeof caddr;
    int cliente;
    char buff[150];
    int isPrime;
    cliente = accept(server, (struct sockaddr *)&caddr, (unsigned int *)&csize);

    while (true) {
        printf("Comecando a ouvir...\n");
        bzero(buff, sizeof buff);
        read(cliente, buff, sizeof buff);
        if (atoi(buff) == 0) {
            printf("Terminando o programa...\n");
            break;
        }
        printf("Mensagem recebida: %s\n", buff);
        isPrime = CheckPrime(atoi(buff));
        printf("Resultado obtido: %d\n", isPrime);
        bzero(buff, sizeof buff);
        sprintf(buff, "%d", isPrime);
        write(cliente, buff, sizeof buff);
    }
    close(cliente);
    return 0;
}
