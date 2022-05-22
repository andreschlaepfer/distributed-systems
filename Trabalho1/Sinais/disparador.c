#include <errno.h>
#include <signal.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

void SendSignal(int receiverPid, int signal) {
    if (kill(receiverPid, signal) == -1) {
        if (errno == ESRCH) {
            printf("PID alvo não encontrado!\n");
        }
        return;
    }
    printf("Sinal enviado\n");
}

int main(int arg, char *argv[]) {
    int targetPid;

    printf("Digite qualquer letra para parar o programa\n");
    printf("Envie um sinal para um processo!\nDigite o PID: ");
    if (scanf("%d", &targetPid) != 1) {
        return 0;
    }
    printf("\n");

    while (true) {
        int signal;

        printf("Digite o sinal: ");
        if (scanf("%d", &signal) != 1) {
            break;
        }
        printf("\n");
        SendSignal(targetPid, signal);
    }
    return 0;
}
