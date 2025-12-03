#include <iostream>
#include <pthread.h>
#include <semaphore.h>
#include <cstdlib>
#include <ctime>
#include <fcntl.h>

using namespace std;

struct zapis{
    string material;
    int count;
};

const int n = 10; //Кол-во потоков
const int k = 2; //Max заявок от одного потока
int processed = 0; //Счетчик для обработанных
int total = 0; //Счетчик для кол-во всего заявок

sem_t *semaphor_write; // Семафор для создания заявок
sem_t *semaphor_read;  // Семафор для создания общей заявки
pthread_mutex_t mutex_data; // для доступа к общим данным

zapis zayavki[n*k]; //Масив записей
zapis obchaya_zayavka[5]; //Общаяя заявка
string materials[] = {"Пластик", "Дерево", "Нефть", "Песок", "Мех"};


void* fun1(void* args){
    int index = *((int*)args);
    int local_index = index * k;
    zapis zayavka;
    int num_requests = rand()%k + 1;
    pthread_mutex_lock(&mutex_data);
    total += num_requests;
    pthread_mutex_unlock(&mutex_data);
    for(int i = 0; i < num_requests; i++)
    {
        sem_wait(semaphor_write);
        zayavka.material = materials[rand()%5];
        zayavka.count = rand()%100 + 1;
        pthread_mutex_lock(&mutex_data);
        zayavki[local_index + i] = zayavka;
        pthread_mutex_unlock(&mutex_data);
        sem_post(semaphor_read);
    }
    return nullptr;
}

void* fun2(void* args){
    int read_index = 0;
    for (int i = 0; i < 5; i++) {
        obchaya_zayavka[i].material = materials[i];
        obchaya_zayavka[i].count = 0;
    }
    while(processed < total){
        sem_wait(semaphor_read);
        pthread_mutex_lock(&mutex_data);
        if (read_index >= total) {
            pthread_mutex_unlock(&mutex_data);
            break;
        }
        zapis temp = zayavki[read_index];
        processed++;
        read_index++;
        pthread_mutex_unlock(&mutex_data);
        for (int i = 0; i < 5; i++) {
            if (obchaya_zayavka[i].material == temp.material) {
                obchaya_zayavka[i].count += temp.count;
                break;
            }
        }
    }
    return nullptr;
}


int main() {
    srand(time(NULL));
    pthread_mutex_init(&mutex_data, NULL);
    // СОЗДАНИЕ ИМЕНОВАННЫХ СЕМАФОРОВ через sem_open():
    // Параметры sem_open():
    // 1. "/sem_write" - имя семафора (должно начинаться со /)
    // 2. O_CREAT | O_EXCL - флаги: создаем новый семафор
    // 3. 0644 - права доступа (rw-r--r--)
    // 4. n*k - начальное значение семафора
    semaphor_write = sem_open("/sem_write", O_CREAT | O_EXCL, 0644, n*k);
    if (semaphor_write == SEM_FAILED) {
        perror("sem_open write");
        exit(1);
    }
    semaphor_read = sem_open("/sem_read", O_CREAT | O_EXCL, 0644, 0);
    if (semaphor_read == SEM_FAILED) {
        perror("sem_open read");
        exit(1);
    }
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
    pthread_create(&processor, nullptr, fun2, nullptr);
    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], nullptr);
    }
    sem_post(semaphor_read);
    pthread_join(processor, nullptr);

    cout << "\n=== ИТОГОВАЯ ЗАЯВКА ===" << endl;
    cout << "Всего создано заявок: " << total << endl;
    cout << "Всего обработано: " << processed << endl;
    cout << "\nИтоговые количества:" << endl;
    for (int i = 0; i < 5; i++) {
        cout << obchaya_zayavka[i].material << ": "
             << obchaya_zayavka[i].count << endl;
    }
    sem_close(semaphor_write);
    sem_close(semaphor_read);
    sem_unlink("/sem_write");
    sem_unlink("/sem_read");
    pthread_mutex_destroy(&mutex_data);
    return 0;
}
