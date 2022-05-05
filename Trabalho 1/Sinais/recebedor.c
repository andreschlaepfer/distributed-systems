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

    printf("PID do processo recebedor: ");
    printf("%d\n", getpid());

    signal(SIGUSR1, handle_sigusr1);
    signal(SIGUSR2, handle_sigusr2);
    signal(SIGTERM, handle_sigterm);

    while (running) {
        printf("Processa, processa, processa, ...\n");

        // blocking wait
        if (atoi(argv[1]) == 1) {
            pause();
        }

        // busy wait
        if (atoi(argv[1]) == 0) {
            while (running)
                ;
        }
    }

    return 0;
}
