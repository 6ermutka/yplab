//Дан текст длиной n символов. Необходимо найти контрольную сумму по данному тексту в виде суммы кодов символов по модулю 256.
// Для нахождения суммы запустите k потоков, где k < n. Каждый i-й поток, i = 0, 1,…,k-1 должен обрабатывать только символы с номерами i + k *s, где s = 0, 1, 2,... –
// шаг работы потока.
#include <iostream>
#include <string>
#include <pthread.h>
#include <cstdlib>
#include <ctime>

using namespace std;

string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
string text;
int n, k;

int total_sum = 0;
pthread_mutex_t sum_mutex = PTHREAD_MUTEX_INITIALIZER;

void* function(void* args) {
    int index = *((int*)args);
    int s = 0, code, sum = 0;
    int temp = index + s * k;
    char ch;
    while (temp < n) {
        ch = text[temp];
        code = (int)ch;
        sum = (sum + code) % 256;
        s++;
        temp = index + s * k;
    }
    pthread_mutex_lock(&sum_mutex);
    total_sum = (total_sum + sum) % 256;
    pthread_mutex_unlock(&sum_mutex);
    return NULL;
}

int main() {
    srand(time(0));
    cout << "Введите число n (кол-во символов в тексте): ";
    cin >> n;
    cout << "Введите кол-во потоков: ";
    cin >> k;

    for (int i = 0; i < n; i++)
        text += characters[rand() % characters.length()];

    cout << "Исходный текст: " << text << endl;

    pthread_t threads[k];
    int thread_args[k];

    for (int i = 0; i < k; i++) {
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],
                nullptr,
                function,
                &thread_args[i]
        );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }

    for (int i = 0; i < k; i++) {
        pthread_join(threads[i], nullptr);
    }
    pthread_mutex_destroy(&sum_mutex);
    cout << "Контрольная сумма: " << total_sum << endl;
    int check_sum = 0;
    for (char c : text) {
        check_sum = (check_sum + (int)c) % 256;
    }
    cout << "Проверочная сумма: " << check_sum << endl;
    return 0;
}
