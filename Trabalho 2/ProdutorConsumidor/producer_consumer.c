#include <math.h>
#include <pthread.h>
#include <semaphore.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#define CONSUMED_MAX pow(10, 5)
#define MUST_PRINT_PRIMES 1

int N;
int *numbers;
int numbers_qty = 0;
int consumed = 0;
pthread_t *threads;
sem_t mutex;
sem_t empty;
sem_t full;

int isPrime(int n) {
    if (n <= 1)
        return 0;

    for (int i = 2; i <= sqrt((double)n); i++)
        if (n % i == 0)
            return 0;

    return 1;
}

void *producer() {
    while (consumed <= CONSUMED_MAX) {
        sem_wait(&empty);
        sem_wait(&mutex);
        numbers[numbers_qty + 1] = (rand() % (int)pow(10, 7)) + 1;
        numbers_qty++;
        sem_post(&mutex);
        sem_post(&full);
    }
    return NULL;
}

void *consumer() {
    while (consumed <= CONSUMED_MAX) {
        sem_wait(&full);
        sem_wait(&mutex);
        int number = numbers[numbers_qty - 1];
        numbers_qty--;
        if (MUST_PRINT_PRIMES && isPrime(number)) {
            printf("%d é primo!\n", number);
        }
        consumed++;
        sem_post(&mutex);
        sem_post(&empty);
    }
    return NULL;
}

void create_threads(int P, int C) {
    threads = (pthread_t *)malloc(sizeof(pthread_t) * (P + C));
    int i;
    for (i = 0; i < P; i++) {
        pthread_create(&threads[i], NULL, &producer, NULL);
    }
    for (i = P; i < P + C; i++) {
        pthread_create(&threads[i], NULL, &consumer, NULL);
    }
}

void join_threads(int P, int C) {
    int i;
    for (i = 0; i < P + C; i++) {
        pthread_join(threads[i], NULL);
    }
    free(threads);
}

int main(int argc, char *argv[]) {
    if (argc != 4) {
        printf("Invalid arguments\n");
        return 0;
    }
    N = atoi(argv[1]);
    int P = atoi(argv[2]);
    int C = atoi(argv[3]);
    clock_t start, finish;
    double spentTime = 0;

    srand(time(NULL));

    numbers = (int *)malloc(sizeof(int) * N);

    for (int i = 0; i < 10; i++) {
        numbers_qty = 0;
        consumed = 0;

        sem_init(&mutex, 0, 1);
        sem_init(&full, 0, 0);
        sem_init(&empty, 0, N);

        start = clock();
        create_threads(P, C);
        join_threads(P, C);
        finish = clock();
        spentTime += (double)(finish - start) / CLOCKS_PER_SEC;

        sem_destroy(&mutex);
        sem_destroy(&full);
        sem_destroy(&empty);
    }

    printf("%.6f", spentTime / 10);

    return 0;
}