//Дана матрица из вещественных чисел, содержащая n строк и m столбцов. Отсортируйте каждый столбец матрицы по возрастанию методом обмена.
#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <iomanip>
const int m = 10, n = 20;
float mtx[m][n];

using namespace std;

void* function(void* args) {
    int N = *((int*)args);
    int i = 0;
    int j;
    float temp;
    int swapped;
    do {
        swapped = 0;
        j = 0;
        while (j < m - i - 1) {
            if (mtx[j][N] > mtx[j + 1][N]) {
                temp = mtx[j][N];
                mtx[j][N] = mtx[j + 1][N];
                mtx[j + 1][N] = temp;
                swapped = 1;
            }
            j++;
        }
        i++;
    } while (swapped && i < m - 1);

    return NULL;
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

    for (int i = 0; i < n; i++) {
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


    for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
            cout << setw(6) << mtx[i][j] << " ";
        }
        cout << endl;
    }

    return 0;
}
