#include <iostream>
#include <pthread.h>
#include <semaphore.h>
#include <fcntl.h>
#include <cstring>

using namespace std;


const int POOL_SIZE = 256;  // Размер пула печати в байтах
const int M = 3;            // Количество потоков печати

// Структура пула печати
struct PrintPool {
    char buffer[POOL_SIZE];
    int used = 0;           // Использовано байт
    int free_space = POOL_SIZE; // Свободно байт
};


PrintPool pool;
pthread_mutex_t pool_mutex = PTHREAD_MUTEX_INITIALIZER;
sem_t *pool_free_space;     // Семафор свободного места
sem_t *pool_used_space;     // Семафор занятого места
bool all_done = false;

const char* input_files[] = {
        "/Users/stepanivanov/CLionProjects/untitled6/print_file_1.txt",
        "/Users/stepanivanov/CLionProjects/untitled6/print_file_2.txt",
        "/Users/stepanivanov/CLionProjects/untitled6/print_file_3.txt"
};

const char* output_file = "/Users/stepanivanov/CLionProjects/untitled6/output_print.txt";


void* printer_thread(void* arg) {
    int thread_id = *((int*)arg);
    const char* filename = input_files[thread_id];
    
    FILE* file = fopen(filename, "rb");
    cout << "Поток " << thread_id << " начал печать файла: " << filename << endl;

    char ch;
    // по одному символу
    while (fread(&ch, 1, 1, file) == 1) {
        sem_wait(pool_free_space);

        pthread_mutex_lock(&pool_mutex);
        pool.buffer[pool.used] = ch;
        pool.used++;
        pool.free_space--;
        pthread_mutex_unlock(&pool_mutex);
        sem_post(pool_used_space);
    }
    fclose(file);
    return nullptr;
}

void* pool_manager_thread(void* arg) {
    // Открываем файл для записи
    FILE* out_file = fopen(output_file, "wb");
    while (true) {
        sem_wait(pool_used_space);
        pthread_mutex_lock(&pool_mutex);
        if (all_done && pool.used == 0) {
            pthread_mutex_unlock(&pool_mutex);
            break;
        }
        
        if (pool.used > 0) {
            fwrite(pool.buffer, 1, 1, out_file);
            fflush(out_file);
            memmove(pool.buffer, pool.buffer + 1, pool.used - 1);
            pool.used--;
            pool.free_space++;
            sem_post(pool_free_space);
        }
        pthread_mutex_unlock(&pool_mutex);
    }
    fclose(out_file);
    cout << "Поток управления пулом завершил работу" << endl;

    return nullptr;
}

int main() {
    pool_free_space = sem_open("/pool_free_space", O_CREAT | O_EXCL, 0644, POOL_SIZE);
    if (pool_free_space == SEM_FAILED) {
        perror("sem_open pool_free_space");
        exit(1);
    }
    pool_used_space = sem_open("/pool_used_space", O_CREAT | O_EXCL, 0644, 0);
    if (pool_used_space == SEM_FAILED) {
        perror("sem_open pool_used_space");
        exit(1);
    }
    pthread_t printer_threads[M];
    pthread_t manager_thread;
    int thread_ids[M];
    
    pthread_create(&manager_thread, nullptr, pool_manager_thread, nullptr);
    
    for (int i = 0; i < M; i++) {
        thread_ids[i] = i;
        pthread_create(&printer_threads[i], nullptr, printer_thread, &thread_ids[i]);
    }

    for (int i = 0; i < M; i++) {
        pthread_join(printer_threads[i], nullptr);
    }
    pthread_mutex_lock(&pool_mutex);
    all_done = true;
    pthread_mutex_unlock(&pool_mutex);
    sem_post(pool_used_space);
    pthread_join(manager_thread, nullptr);
    cout << "Результат записан в файл: " << output_file << endl;
    sem_close(pool_free_space);
    sem_close(pool_used_space);
    sem_unlink("/pool_free_space");
    sem_unlink("/pool_used_space");
    pthread_mutex_destroy(&pool_mutex);
    return 0;
}
