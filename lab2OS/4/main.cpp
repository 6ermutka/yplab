// Написать программу формирования дека из элементов файла с помощью n потоков. Каждый поток может добавить элемент в начало или конец дека.
// Решение о месте вставки принимается случайным образом. Если в это время другой поток модифицирует выбранный участок дека,
// поток должен перейти в состояние ожидания.

#include <iostream>
#include <fstream>
#include <deque>
#include <pthread.h>
#include <cstdlib>
#include <ctime>

using namespace std;
int* number = nullptr;
deque<int> numbers {};
int n = 0;
pthread_mutex_t mutexx = PTHREAD_MUTEX_INITIALIZER;

void* fun(void* arg) {
    int N = *((int*)arg);
    bool k = rand() % 2;
    if(k){
        pthread_mutex_lock(&mutexx);
        numbers.push_back(number[N]);
        pthread_mutex_unlock(&mutexx);
    }
    else{
        pthread_mutex_lock(&mutexx);
        numbers.push_front(number[N]);
        pthread_mutex_unlock(&mutexx);
    }

    return NULL;
}

int main() {
    srand(time(0));
    ifstream file("/Users/stepanivanov/CLionProjects/untitled5/123.txt");
    if (!file.is_open()) {
        cout << "Ошибка открытия файла!" << endl;
        return 1;
    }
    cout << "Введите кол-во элементов в файле:";
    cin >> n;
    number = new int[n];
    for (int i = 0; i < n; i++) {
        file >> number[i];
    }
    file.close();
    pthread_t threads[n];
    int thread_args[n];
    for(int i = 0; i < n; i++){
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],
                nullptr,
                fun,
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
    pthread_mutex_destroy(&mutexx);
    cout << "Содержимое дека: ";
    for (int i = 0; i < numbers.size(); i++) {
        cout << numbers[i] << " ";
    }
    cout << endl;
}
