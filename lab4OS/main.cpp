#include <iostream>
#include <pthread.h>
#include <semaphore.h>
#include <cstdlib>
#include <fcntl.h>
#include <fstream>
#include <cctype>

using namespace std;

// Константы
const int k = 100; // Размер буфера
const int n = 3;   // Количество потоков-шифровщиков

// Структура для буфера
struct BufferItem {
    char character;
    bool processed;    // Флаг, что символ обработан
    bool encrypted;    // Флаг, что символ зашифрован
};

// Глобальные переменные
BufferItem buffer[k];  // Буфер
int cur_pos = 0;       // Текущая позиция в буфере для записи
int read_pos = 0;      // Текущая позиция в буфере для чтения
int write_pos = 0;     // Текущая позиция в буфере для записи зашифрованных данных

// Семафоры и мьютексы
sem_t *empty_slots;    // Семафор для свободных мест в буфере
sem_t *filled_slots;   // Семафор для заполненных мест (для шифровальщиков)
sem_t *encrypted_slots; // Семафор для зашифрованных мест (для писателя)
pthread_mutex_t buffer_mutex = PTHREAD_MUTEX_INITIALIZER; // Мьютекс для доступа к буферу

// Флаги завершения
bool file_finished = false;


char encrypt_char(char c) {
    if (!isalpha(c)) return c;
    c = toupper(c);
    if (c == 'Z') return 'A';
    return c + 1;
}

void* reader_thread(void* arg) {
    ifstream input_file("/Users/stepanivanov/CLionProjects/untitled6/input.txt");
    char ch;
    while (input_file >> noskipws >> ch) {
        sem_wait(empty_slots);
        pthread_mutex_lock(&buffer_mutex);
        buffer[cur_pos].character = ch;
        buffer[cur_pos].processed = false;
        buffer[cur_pos].encrypted = false;
        cur_pos = (cur_pos + 1) % k;
        pthread_mutex_unlock(&buffer_mutex);
        sem_post(filled_slots);
    }
    file_finished = true;
    input_file.close();
    for (int i = 0; i < n; i++) {
        sem_post(filled_slots);
    }
    return nullptr;
}

void* encryptor_thread(void* arg) {
    int thread_id = *(int*)arg;
    while (true) {
        sem_wait(filled_slots);
        pthread_mutex_lock(&buffer_mutex);
        if (file_finished && read_pos == cur_pos) {
            pthread_mutex_unlock(&buffer_mutex);
            break;
        }

        int found_pos = -1;
        for (int i = 0; i < k; i++) {
            int idx = (read_pos + i) % k;
            if (!buffer[idx].processed && !buffer[idx].encrypted) {
                found_pos = idx;
                break;
            }
        }

        if (found_pos != -1) {
            buffer[found_pos].character = encrypt_char(buffer[found_pos].character);
            buffer[found_pos].processed = true;
            buffer[found_pos].encrypted = true;
            if (found_pos == read_pos) {
                while (read_pos != cur_pos && buffer[read_pos].encrypted) {
                    read_pos = (read_pos + 1) % k;
                }
            }
            sem_post(encrypted_slots);
        }
        pthread_mutex_unlock(&buffer_mutex);
    }
    return nullptr;
}

void* writer_thread(void* arg) {
    ofstream output_file("/Users/stepanivanov/CLionProjects/untitled6/output.txt");
    while (true) {
        sem_wait(encrypted_slots);
        pthread_mutex_lock(&buffer_mutex);
        if (file_finished && write_pos == cur_pos && read_pos == cur_pos) {
            pthread_mutex_unlock(&buffer_mutex);
            break;
        }
        int found_pos = -1;
        for (int i = 0; i < k; i++) {
            int idx = (write_pos + i) % k;
            if (buffer[idx].encrypted) {
                found_pos = idx;
                break;
            }
        }

        if (found_pos != -1) {
            output_file << buffer[found_pos].character;
            buffer[found_pos].encrypted = false;
            if (found_pos == write_pos) {
                while (write_pos != cur_pos && !buffer[write_pos].encrypted) {
                    write_pos = (write_pos + 1) % k;
                }
            }
            sem_post(empty_slots);
        }

        pthread_mutex_unlock(&buffer_mutex);
    }
    output_file.close();
    return nullptr;
}

int main() {
    // Инициализация буфера
    for (int i = 0; i < k; i++) {
        buffer[i].processed = false;
        buffer[i].encrypted = false;
        buffer[i].character = '\0';
    }
    empty_slots = sem_open("/empty_slots", O_CREAT | O_EXCL, 0644, k);
    if (empty_slots == SEM_FAILED) {
        perror("sem_open empty_slots");
        exit(1);
    }
    filled_slots = sem_open("/filled_slots", O_CREAT | O_EXCL, 0644, 0);
    if (filled_slots == SEM_FAILED) {
        perror("sem_open filled_slots");
        exit(1);
    }
    encrypted_slots = sem_open("/encrypted_slots", O_CREAT | O_EXCL, 0644, 0);
    if (encrypted_slots == SEM_FAILED) {
        perror("sem_open encrypted_slots");
        exit(1);
    }

    pthread_t reader;
    pthread_t encryptors[n];
    pthread_t writer;

    int encryptor_ids[n];
    if (pthread_create(&reader, nullptr, reader_thread, nullptr) != 0) {
        cout << "Ошибка создания потока-читателя" << endl;
        return 1;
    }
    for (int i = 0; i < n; i++) {
        encryptor_ids[i] = i;
        if (pthread_create(&encryptors[i], nullptr, encryptor_thread, &encryptor_ids[i]) != 0) {
            cout << "Ошибка создания потока-шифровальщика " << i << endl;
            return 1;
        }
    }
    if (pthread_create(&writer, nullptr, writer_thread, nullptr) != 0) {
        cout << "Ошибка создания потока-писателя" << endl;
        return 1;
    }

    pthread_join(reader, nullptr);

    for (int i = 0; i < n; i++) {
        pthread_join(encryptors[i], nullptr);
    }
    sem_post(encrypted_slots);
    pthread_join(writer, nullptr);

    cout << "Результат записан в файл: output.txt" << endl;

    sem_close(empty_slots);
    sem_close(filled_slots);
    sem_close(encrypted_slots);

    sem_unlink("/empty_slots");
    sem_unlink("/filled_slots");
    sem_unlink("/encrypted_slots");

    pthread_mutex_destroy(&buffer_mutex);

    return 0;
}
