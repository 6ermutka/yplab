//Дана матрица из вещественных чисел, содержащая n строк и m столбцов. Найдите номер строки с максимальным средним значением.
#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <iomanip>
const int m = 10, n = 20;
float mtx[m][n];
float SUM[n];

using namespace std;

void* function(void* args) {
    int N = *((int*)args);
    SUM[N] = 0;
    for (int i = 0; i < n; i++) {
        SUM[N] += mtx[N][i];
    }
    SUM[N] = SUM[N]/n;
    return 0;
}

void* creatmatrix(void* args){
    int i = *((int*)args);
    for (int j = 0; j < n; j++)
        mtx[i][j] = (float)(rand() % 100);
    return 0;
}

int main() {
    srand(time(0));
    pthread_t threads1[m];     // Массив идентификаторов потоков
    int thread_args1[m];       // Массив аргументов для потоков
    pthread_t threads[n];     // Массив идентификаторов потоков
    int thread_args[n];       // Массив аргументов для потоков


    for(int i = 0; i < m; i++){
        thread_args1[i] = i;
        int result = pthread_create(
                &threads1[i],
                nullptr,
                creatmatrix,
                &thread_args1[i]
                );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }

    for (int i = 0; i < m; i++) {
        pthread_join(
                threads1[i],      // ID потока, которого ждём
                nullptr
        );
    }

    for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
            cout << setw(6) << mtx[i][j] << " ";
        }
        cout << endl;
    }
    cout << endl;

    for (int i = 0; i < m; i++) {
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],      // Указатель на переменную для ID потока (выходной параметр)
                nullptr,          // Атрибуты потока (по умолчанию - NULL)
                function,          // Функция, которую будет выполнять поток
                &thread_args[i]   // Аргумент для функции потока (указатель на номер столбца)
        );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }

    for (int i = 0; i < n; i++) {
        pthread_join(
                threads[i],      // ID потока, которого ждём
                nullptr          // Указатель для возвращаемого значения
        );
    }

    cout << "Среднее значение столбцов:" << endl;
    for (int i = 0; i < m; i++) {
        cout << "Столбец " << i << ": " << SUM[i] << endl;
    }
    cout << endl;

    int imax = 0;
    float max = SUM[0];
    for (int i = 1; i < m; i++) {
        if (SUM[i] > max) {
            max = SUM[i];
            imax = i;
        }
    }
    cout << "Искомый столбец № " << imax << " с минимальной суммой: " << max << endl;

    return 0;
}
