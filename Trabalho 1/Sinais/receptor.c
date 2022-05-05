#include <errno.h>
#include <signal.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

bool running = true;

void handle_sigusr1(int signal) {
    printf("Alerta: Voce recebeu o sinal SIGUSR1!\n");
}
void handle_sigusr2(int signal) {
    printf("Alerta: Voce enviou o sinal SIGUSR2!\n");
}
void handle_sigterm(int signal) {
    printf("Terminando o programa...\n");
    running = false;
}

int main(int arg, char *argv[]) {
    if (arg != 2) {
        printf("O programa deve receber um argumento.\n\n0: busy wait\n1: blocking wait\n");
        return 1;
    }

    int waitingMode = atoi(argv[1]);

    if (waitingMode != 0 && waitingMode != 1) {
        printf("O argumento deve ser 0 ou 1.\n\n0: busy wait\n1: blocking wait\n");
        return 1;
    }

    printf("PID do processo receptor: ");
    printf("%d\n", getpid());

    signal(SIGUSR1, handle_sigusr1);
    signal(SIGUSR2, handle_sigusr2);
    signal(SIGTERM, handle_sigterm);

    while (running) {
        printf("Processa, processa, processa, ...\n");

        // busy wait
        if (atoi(argv[1]) == 0) {
            while (running)
                ;
        }

        // blocking wait
        if (atoi(argv[1]) == 1) {
            pause();
        }
    }

    return 0;
}
