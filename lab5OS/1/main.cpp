#include <iostream>
#include <pthread.h>

using namespace std;

int n; // Кол-во элементов
int* massiv; // Массив элементов
bool* half_processed; // Флаг - Половина прошла
bool* pass_completed; // Флаг - проход выполнен
pthread_cond_t* conds; // Событие
pthread_mutex_t mutexx = PTHREAD_MUTEX_INITIALIZER; // Доп мьютекс

void* function(void* args) {
    int N = *((int*)args);
    int flag = n / 2;
    bool swapped = false;

    pthread_mutex_lock(&mutexx);

    if (N > 0) {
        while (!half_processed[N - 1] && !pass_completed[N - 1]) {
            pthread_cond_wait(&conds[N], &mutexx);
        }
    }

    if (pass_completed[N]) {
        pthread_mutex_unlock(&mutexx);
        return NULL;
    }

    for (int i = 0; i < n - N - 1; i++) {
        if (massiv[i] > massiv[i + 1]) {
            int temp = massiv[i];
            massiv[i] = massiv[i + 1];
            massiv[i + 1] = temp;
            swapped = true;
        }

        if (i == flag - 1 && N < n - 1) {
            half_processed[N] = true;
            pthread_cond_signal(&conds[N + 1]);
        }
    }

    if (!swapped && N < n - 1) {
        pass_completed[N + 1] = true;
        pthread_cond_signal(&conds[N + 1]);
    }

    pass_completed[N] = true;
    pthread_mutex_unlock(&mutexx);
    return NULL;
}

int main() {
    cout << "Введите кол-во элементов массива: ";
    cin >> n;

    massiv = new int[n];
    half_processed = new bool[n];
    pass_completed = new bool[n];
    conds = new pthread_cond_t[n];

    cout << "Введите элементы массива: ";
    for (int i = 0; i < n; i++) {
        cin >> massiv[i];
        half_processed[i] = false;
        pass_completed[i] = false;
        pthread_cond_init(&conds[i], NULL);
    }

    pthread_t* threads = new pthread_t[n];
    int* thread_args = new int[n];

    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        pthread_create(&threads[i], NULL, function, &thread_args[i]);
    }

    half_processed[0] = true;
    pthread_cond_signal(&conds[0]);

    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], NULL);
    }

    cout << "Отсортированный массив: ";
    for (int i = 0; i < n; i++) {
        cout << massiv[i] << " ";
    }
    cout << endl;

    for (int i = 0; i < n; i++) {
        pthread_cond_destroy(&conds[i]);
    }
    pthread_mutex_destroy(&mutexx);

    delete[] massiv;
    delete[] half_processed;
    delete[] pass_completed;
    delete[] conds;
    delete[] threads;
    delete[] thread_args;

    return 0;
}
