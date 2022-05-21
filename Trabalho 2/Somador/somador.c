#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

int acm = 0;
char *values;

struct lock {
    int held;
};

struct thread_args {
    int start_range;
    struct lock lock;
    int end_range;
};

void acquire(struct lock lock) {
    while (__sync_lock_test_and_set(&lock.held, 1))
        ;
}

void release(struct lock *lock) {
    lock->held = 0;
}

void *sum(void *arg) {
    struct thread_args *args = arg;
    int sum = 0;

    for (int i = args->start_range; i < args->end_range; i++) {
        sum += values[i];
    }

    acquire(args->lock);
    acm += sum;
    release(&args->lock);

    return NULL;
}

void create_threads(int N, int K) {
    pthread_t *th = (pthread_t *)malloc(sizeof(pthread_t) * K);
    struct lock lock;
    lock.held = 0;
    struct thread_args args[K];
    int i;
    for (i = 0; i < K; i++) {
        args[i].start_range = i * N / K;
        args[i].lock = lock;
        args[i].end_range = (i + 1) * N / K;
        pthread_create(&th[i], NULL, &sum, (void *)&args[i]);
    }
    for (i = 0; i < K; i++) {
        pthread_join(th[i], NULL);
    }
    free(th);
}

void generate_values(int N) {
    int i;
    for (i = 0; i < N; i++) {
        values[i] = (char)(rand() % 200 - 100);
    }
}

int main(int argc, char *argv[]) {
    if (argc != 3) {
        printf("Invalid arguments\n");
        return 0;
    }
    srand(time(NULL));

    int N = atoi(argv[1]);
    int K = atoi(argv[2]);
    double spentTime = 0;

    for (int i = 0; i < 10; i++) {
        values = (char *)malloc(sizeof(char) * N);
        clock_t start, finish;
        generate_values(N);
        start = clock();
        create_threads(N, K);
        finish = clock();
        spentTime += (double)(finish - start) / CLOCKS_PER_SEC;
        free(values);
    }

    printf("%.6f", spentTime / 10);

    return 0;
}