//На некотором предприятии имеется n цехов, которые независимо друг от друга формируют заявки на приобретение материалов.
// Все заявки поступают в единый обрабатывающий центр, который должен сформировать единую заявку в виде массива записей,
// одержащих информацию о наименовании и общем количестве этого материала. Каждый цех может сформировать не более чем k заявок.
// Напишите программу для формирования общей заявки при указанных условиях.

#include <iostream>
#include <pthread.h>
#include <semaphore.h>
#include <cstdlib>
#include <ctime>

using namespace std;

struct zapis{
    string material;
    int count;
};

const int n = 10;
const int k = 10;
int index_n;
int processed = 0;
int total = 0;

sem_t semaphor_write; //Семафор дл создания заявок
sem_t semaphor_read; //Семафор для создания общей заявки
pthread_mutex_t mutex_data; // для доступа к общим данным

zapis zayavki[n*k];
zapis obchaya_zayavka[5];
string materials[] = {"Пластик", "Дерево", "Нефть", "Песок", "Мех"};


void* fun1(void* args){
    int index = *((int*)args);
    index_n = index;
    zapis zayavka;
    int num_requests = rand()%k + 1;
    total = num_requests + total;
    for(int i = 0; i < num_requests; i++)
    {
        sem_wait(&semaphor_write);
        zayavka.material = materials[rand()%6];
        zayavka.count = rand()%100;
        pthread_mutex_lock(&mutex_data);
        zayavki[index_n] = zayavka;
        pthread_mutex_unlock(&mutex_data);
        index_n = index_n+index;
        sem_post(&semaphor_read);
    }
}

void* fun2(void* args){
    int read_index = 0;
    for (int i = 0; i < 5; i++) {
        obchaya_zayavka[i].material = materials[i];
        obchaya_zayavka[i].count = 0;
    }
    while(processed < total){
        sem_wait(&semaphor_write);
        pthread_mutex_lock(&mutex_data);
        zapis temp = zayavki[read_index];
        processed++;
        read_index++;
        sem_post(&semaphor_write);
        pthread_mutex_unlock(&mutex_data);
        for (int i = 0; i < 5; i++) {
            if (obchaya_zayavka[i].material == temp.material) {
                obchaya_zayavka[i].count += temp.count;
                break;
            }
        }
    }
}


int main() {
    srand(time(NULL));
    pthread_mutex_init(&mutex_data, NULL);
    sem_init(&semaphor_write, 0, n*k);
    sem_init(&semaphor_read, 0, 0);
    pthread_t threads[n], processor;
    int thread_args[n];
    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],
                nullptr,
                fun1,
                &thread_args[i]
        );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }
    for (int i = 0; i < n; i++) {
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],
                nullptr,
                fun1,
                &thread_args[i]
        );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }
    pthread_create(&processor, nullptr, fun2, nullptr);
    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], nullptr);
    }
    pthread_join(processor, nullptr);

    // Выводим итоговую заявку
    cout << "\n=== ИТОГОВАЯ ЗАЯВКА ===" << endl;
    cout << "Всего создано заявок: " << total << endl;
    cout << "Всего обработано: " << processed << endl;
    cout << "\nИтоговые количества:" << endl;
    for (int i = 0; i < 5; i++) {
        cout << obchaya_zayavka[i].material << ": "
             << obchaya_zayavka[i].count << endl;
    }

    sem_destroy(&semaphor_write);
    sem_destroy(&semaphor_read);
    pthread_mutex_destroy(&mutex_data);
    return 0;
}
