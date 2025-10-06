#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <ctime>
#include <iomanip>
#include <unistd.h>

using namespace std;

int n, m;
float** mtx = nullptr;
float* SUM = nullptr;
bool* row_ready = nullptr;

pthread_mutex_t mutexx = PTHREAD_MUTEX_INITIALIZER;

void* function(void* args) {
    int N = *((int*)args);
    bool ready = false;
    while (!ready) {
        pthread_mutex_lock(&mutexx);
        ready = row_ready[N];
        pthread_mutex_unlock(&mutexx);
        if (!ready) {
            usleep(1000);
        }
    }
    float sum = 0;
    for (int j = 0; j < m; j++) {
        sum += mtx[N][j];
    }
    pthread_mutex_lock(&mutexx);
    SUM[N] = sum;
    pthread_mutex_unlock(&mutexx);
    return 0;
}

void* creatematrix(void* args) {
    int i = *((int*)args);
    for (int j = 0; j < m; j++) {
        mtx[i][j] = (float)(rand() % 100) / 10.0f;
    }
    pthread_mutex_lock(&mutexx);
    row_ready[i] = true;
    pthread_mutex_unlock(&mutexx);
    return 0;
}

int main() {
    srand(time(0));
    cout << "Введите кол-во строк в матрице (n): ";
    cin >> n;
    cout << "Введите кол-во столбцов в матрице (m): ";
    cin >> m;
    mtx = new float*[n];
    for (int i = 0; i < n; i++) {
        mtx[i] = new float[m];
    }
    SUM = new float[n];
    row_ready = new bool[n];
    for (int i = 0; i < n; i++) {
        row_ready[i] = false;
    }
    pthread_t threads[n];
    int thread_args[n];
    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        pthread_create(
                &threads[i],
                NULL,
                creatematrix,
                &thread_args[i]);
    }
    pthread_t threads1[n];
    int thread_args1[n];
    for (int i = 0; i < n; i++) {
        thread_args1[i] = i;
        pthread_create(
                &threads1[i],
                NULL,
                function,
                &thread_args1[i]);
    }
    for (int i = 0; i < n; i++) {
        pthread_join(threads1[i], NULL);
    }
    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], NULL);
    }
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            cout << setw(6) << fixed << setprecision(1) << mtx[i][j] << " ";
        }
        cout << " | Сумма: " << SUM[i] << endl;
    }
    float total_sum = 0;
    for (int i = 0; i < n; i++) {
        total_sum += SUM[i];
    }
    cout << "Общая сумма матрицы: " << total_sum << endl;
    pthread_mutex_destroy(&mutexx);

    return 0;
}
