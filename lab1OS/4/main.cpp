//Дана квадратная матрица размерности 4×4. Найдите определитель этой матрицы.
#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <iomanip>
const int m = 4, n = 4, N = 24;
int indexs[N] = {
        11223344, 11233442, 11243243, 12213443, 12233144, 12243341,
        13213244, 13223441, 13243142, 14213342, 14223143, 14233241,
        11223443, 11233244, 11243342, 12213344, 12233441, 12243143,
        13213442, 13223144, 13243241, 14213243, 14223341, 14233142
};
float mtx[m][n];
int sum[N];

using namespace std;

void* function(void* args) {
    int N = *((int*)args);
    int temp1,temp2;
    for(int i=0;i<n;i++){
        temp1 = indexs[N]%10;
        indexs[N] = indexs[N]/10;
        temp2 = indexs[N]%10;
        indexs[N] = indexs[N]/10;
        sum[N] = sum[N]*mtx[temp2-1][temp1-1];
    }
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
    pthread_t threads1[m];
    int thread_args1[m];
    pthread_t threads[N];
    int thread_args[N];

    for(int i = 0;i<N;i++) sum[i] = 1;

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
                threads1[i],
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

    for (int i = 0; i < N; i++) {
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

    for (int i = 0; i < n; i++) {
        pthread_join(
                threads[i],
                nullptr
        );
    }
    int otvet = 0;
    for(int i=0;i<N/2;i++) {
        otvet = sum[i]+otvet;
    }
    for(int i=N/2;i<N;i++) {
        otvet = otvet-sum[i];
    }
    cout << "Определитель = " << otvet;
    return 0;
}
