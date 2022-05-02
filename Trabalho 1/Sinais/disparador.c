#include <stdlib.h>
#include <stdio.h>
#include <signal.h>
#include <stdbool.h>
#include <unistd.h>
#include <errno.h>

void SendSignal(int receiverPid, int signal)
{
  if (kill(receiverPid, signal) == -1)
  {
    if (errno == ESRCH)
    {
      printf("PID alvo não encontrado!\n");
    }
    return;
  }
  printf("Sinal enviado\n");
}

int main(int arg, char *argv[])
{

  while (true)
  {
    int targetPid;
    int signal;
    char runProgram;

    printf("Digite qualquer letra para parar o programa\n");
    printf("Envie um sinal para um processo!\nDigite o PID: ");
    if (scanf("%d", &targetPid) != 1)
    {
      break;
    }
    printf("\n");
    printf("Digite o sinal: ");
    if (scanf("%d", &signal) != 1)
    {
      break;
    }
    printf("\n");
    SendSignal(targetPid, signal);
  }
  return 0;
}
