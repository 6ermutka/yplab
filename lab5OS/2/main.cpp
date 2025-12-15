#include <iostream>
#include <pthread.h>
#include <cmath>

using namespace std;

int x; // Степень в которую надо возвести
int n; // Кол-во элементов в ряд Маклорена;
double total_sum = 0; // Сумма ряда
double* powers; // Массив для хранения степеней
double* factorials; // Массив для хранения факториалов
bool* power_ready; // Флаги готовности степеней
bool* factorial_ready; // Флаги готовности факториалов

pthread_cond_t* power_conds; // Условные переменные для степеней
pthread_cond_t* factorial_conds; // Условные переменные для факториалов
pthread_mutex_t* power_mutexes; // Мьютексы для степеней
pthread_mutex_t* factorial_mutexes; // Мьютексы для факториалов
pthread_mutex_t sum_mutex = PTHREAD_MUTEX_INITIALIZER; // Мьютекс для суммы

void* power_calc(void* args) {
    int i = *((int*)args);
    pthread_mutex_lock(&power_mutexes[i]);

    double result = 1.0;
    for (int j = 0; j < i; j++) {
        result *= x;
    }
    powers[i] = result;
    power_ready[i] = true;

    pthread_cond_signal(&power_conds[i]);
    pthread_mutex_unlock(&power_mutexes[i]);

    return NULL;
}

void* factorial_calc(void* args) {
    int i = *((int*)args);
    pthread_mutex_lock(&factorial_mutexes[i]);

    double result = 1.0;
    for (int j = 1; j <= i; j++) {
        result *= j;
    }
    factorials[i] = result;
    factorial_ready[i] = true;

    pthread_cond_signal(&factorial_conds[i]);
    pthread_mutex_unlock(&factorial_mutexes[i]);

    return NULL;
}

void* term_calc(void* args) {
    int i = *((int*)args);

    pthread_mutex_lock(&power_mutexes[i]);
    while (!power_ready[i]) {
        pthread_cond_wait(&power_conds[i], &power_mutexes[i]);
    }
    double current_power = powers[i];
    pthread_mutex_unlock(&power_mutexes[i]);

    pthread_mutex_lock(&factorial_mutexes[i]);
    while (!factorial_ready[i]) {
        pthread_cond_wait(&factorial_conds[i], &factorial_mutexes[i]);
    }
    double current_factorial = factorials[i];
    pthread_mutex_unlock(&factorial_mutexes[i]);

    double term = current_power / current_factorial;

    pthread_mutex_lock(&sum_mutex);
    total_sum += term;
    pthread_mutex_unlock(&sum_mutex);


    return NULL;
}

int main() {
    cout << "Введите x: ";
    cin >> x;
    cout << "Введите n: ";
    cin >> n;

    powers = new double[n];
    factorials = new double[n];
    power_ready = new bool[n];
    factorial_ready = new bool[n];

    power_conds = new pthread_cond_t[n];
    factorial_conds = new pthread_cond_t[n];
    power_mutexes = new pthread_mutex_t[n];
    factorial_mutexes = new pthread_mutex_t[n];

    for (int i = 0; i < n; i++) {
        power_ready[i] = false;
        factorial_ready[i] = false;
        pthread_cond_init(&power_conds[i], NULL);
        pthread_cond_init(&factorial_conds[i], NULL);
        pthread_mutex_init(&power_mutexes[i], NULL);
        pthread_mutex_init(&factorial_mutexes[i], NULL);
    }

    pthread_t* power_threads = new pthread_t[n];
    pthread_t* factorial_threads = new pthread_t[n];
    pthread_t* term_threads = new pthread_t[n];
    int* thread_args = new int[n];

    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        pthread_create(&power_threads[i], NULL, power_calc, &thread_args[i]);
        pthread_create(&factorial_threads[i], NULL, factorial_calc, &thread_args[i]);
    }

    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        pthread_create(&term_threads[i], NULL, term_calc, &thread_args[i]);
    }

    for (int i = 0; i < n; i++) {
        pthread_join(power_threads[i], NULL);
        pthread_join(factorial_threads[i], NULL);
        pthread_join(term_threads[i], NULL);
    }

    cout << "\nСумма ряда Маклорена (первых " << n << " элементов): " << total_sum << endl;
    cout << "Значение exp(" << x << ") из math.h: " << exp(x) << endl;
    cout << "Разница: " << abs(exp(x) - total_sum) << endl;

    for (int i = 0; i < n; i++) {
        pthread_cond_destroy(&power_conds[i]);
        pthread_cond_destroy(&factorial_conds[i]);
        pthread_mutex_destroy(&power_mutexes[i]);
        pthread_mutex_destroy(&factorial_mutexes[i]);
    }

    delete[] powers;
    delete[] factorials;
    delete[] power_ready;
    delete[] factorial_ready;
    delete[] power_conds;
    delete[] factorial_conds;
    delete[] power_mutexes;
    delete[] factorial_mutexes;
    delete[] power_threads;
    delete[] factorial_threads;
    delete[] term_threads;
    delete[] thread_args;

    pthread_mutex_destroy(&sum_mutex);

    return 0;
}
