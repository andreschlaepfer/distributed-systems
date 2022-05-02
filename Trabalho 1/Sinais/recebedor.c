#include <stdlib.h>
#include <stdio.h>
#include <signal.h>
#include <stdbool.h>
#include <unistd.h>
#include <errno.h>

bool running = true;

void handle_sigusr1(int signal)
{
  printf("Alerta: Voce recebeu o sinal SIGUSR1!\n");
}
void handle_sigusr2(int signal)
{
  printf("Alerta: Voce enviou o sinal SIGUSR2!\n");
}
void handle_sigterm(int signal)
{
  printf("Terminando o programa...\n");
  running = false;
}

int main(int arg, char *argv[])
{
  printf("PID do processo recebedor: ");
  printf("%d\n", getpid());

  signal(SIGUSR1, handle_sigusr1);
  signal(SIGUSR2, handle_sigusr2);
  signal(SIGTERM, handle_sigterm);

  while (running)
  {
    printf("Processa, processa, processa, ...\n");
    pause();
    if (running == false)
    {
      break;
    }
    printf("De volta ao trabalho...\n");
  }

  return 0;
}
