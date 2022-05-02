#include <stdlib.h>
#include <stdio.h>
#include <signal.h>
#include <stdbool.h>
#include <unistd.h>
#include <errno.h>
#include <math.h>

int RandomNumberGenerator(int n)
{
  return n + rand() % 100 + 1;
}

bool PrintIsPrimeOrBreakIfZero(int n)
{
  if (n == 0)
  {
    printf("Terminando o programa.\n");
    return true;
  }
  int i;

  for (i = 2; i <= sqrt(n); i++)
  {
    if (n % i == 0)
    {
      printf("%d não é primo\n", n);
      return false;
    }
  }

  if (n <= 1)
  {
    printf("%d não é primo\n", n);
    return false;
  }
  return false;
  printf("%d é primo!\n", n);
}

int main(int arg, char *argv[])
{
  int fd[2];
  if (pipe(fd) == -1)
  {
    printf("Erro ao abrir o pipe\n");
  }

  int pid = fork();
  if (pid == 0)
  {
    //producer

    int n = 0;
    int maxIterations = 1000;
    int numberOfIterations = 0;

    while (numberOfIterations <= maxIterations)
    {
      numberOfIterations++;
      n = RandomNumberGenerator(n);
      write(fd[1], &n, sizeof(int));
    }
    n = 0;
    write(fd[1], &n, sizeof(int));
  }
  else
  {
    //consumer
    while (true)
    {
      int primeCandidate;
      read(fd[0], &primeCandidate, sizeof(int));
      if (PrintIsPrimeOrBreakIfZero(primeCandidate))
      {
        break;
      }
    }
  }

  return 0;
}