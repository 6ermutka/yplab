#include <iostream>
#include <pthread.h>
#include <semaphore.h>
#include <cstdlib>
#include <fcntl.h>
#include <fstream>
#include <cctype>

using namespace std;


const int k = 100;
const int n = 3;


struct BufferItem {
    char character;
    bool processed;
    bool decrypted;
};


BufferItem buffer[k];
int cur_pos = 0;
int read_pos = 0;
int write_pos = 0;


sem_t *empty_slots;
sem_t *filled_slots;
sem_t *decrypted_slots;
pthread_mutex_t buffer_mutex = PTHREAD_MUTEX_INITIALIZER;

bool file_finished = false;

char decrypt_char(char c) {
    if (!isalpha(c)) return c;
    c = tolower(c);
    if (c == 'A') return 'Z';
    return c - 1;
}

void* reader_thread(void* arg) {
    ifstream input_file("/Users/stepanivanov/CLionProjects/untitled6/output.txt");
    char ch;
    while (input_file >> noskipws >> ch) {
        sem_wait(empty_slots);
        pthread_mutex_lock(&buffer_mutex);
        buffer[cur_pos].character = ch;
        buffer[cur_pos].processed = false;
        buffer[cur_pos].decrypted = false;
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

void* decryptor_thread(void* arg) {
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
            if (!buffer[idx].processed && !buffer[idx].decrypted) {
                found_pos = idx;
                break;
            }
        }
        if (found_pos != -1) {
            buffer[found_pos].character = decrypt_char(buffer[found_pos].character);
            buffer[found_pos].processed = true;
            buffer[found_pos].decrypted = true;
            if (found_pos == read_pos) {
                while (read_pos != cur_pos && buffer[read_pos].decrypted) {
                    read_pos = (read_pos + 1) % k;
                }
            }
            sem_post(decrypted_slots);
        }
        pthread_mutex_unlock(&buffer_mutex);
    }
    return nullptr;
}

void* writer_thread(void* arg) {
    ofstream output_file("/Users/stepanivanov/CLionProjects/untitled6/decrypted.txt");
    while (true) {
        sem_wait(decrypted_slots);
        pthread_mutex_lock(&buffer_mutex);
        if (file_finished && write_pos == cur_pos && read_pos == cur_pos) {
            pthread_mutex_unlock(&buffer_mutex);
            break;
        }
        int found_pos = -1;
        for (int i = 0; i < k; i++) {
            int idx = (write_pos + i) % k;
            if (buffer[idx].decrypted) {
                found_pos = idx;
                break;
            }
        }
        if (found_pos != -1) {
            output_file << buffer[found_pos].character;
            buffer[found_pos].decrypted = false;
            if (found_pos == write_pos) {
                while (write_pos != cur_pos && !buffer[write_pos].decrypted) {
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
    for (int i = 0; i < k; i++) {
        buffer[i].processed = false;
        buffer[i].decrypted = false;
        buffer[i].character = '\0';
    }
    empty_slots = sem_open("/empty_slots_decrypt", O_CREAT | O_EXCL, 0644, k);
    if (empty_slots == SEM_FAILED) {
        perror("sem_open empty_slots");
        exit(1);
    }
    filled_slots = sem_open("/filled_slots_decrypt", O_CREAT | O_EXCL, 0644, 0);
    if (filled_slots == SEM_FAILED) {
        perror("sem_open filled_slots");
        exit(1);
    }
    decrypted_slots = sem_open("/decrypted_slots", O_CREAT | O_EXCL, 0644, 0);
    if (decrypted_slots == SEM_FAILED) {
        perror("sem_open decrypted_slots");
        exit(1);
    }
    pthread_t reader;
    pthread_t decryptors[n];
    pthread_t writer;
    int decryptor_ids[n];
    if (pthread_create(&reader, nullptr, reader_thread, nullptr) != 0) {
        cout << "Ошибка создания потока-читателя" << endl;
        return 1;
    }
    for (int i = 0; i < n; i++) {
        decryptor_ids[i] = i;
        if (pthread_create(&decryptors[i], nullptr, decryptor_thread, &decryptor_ids[i]) != 0) {
            cout << "Ошибка создания потока-дешифровщика " << i << endl;
            return 1;
        }
    }
    if (pthread_create(&writer, nullptr, writer_thread, nullptr) != 0) {
        cout << "Ошибка создания потока-писателя" << endl;
        return 1;
    }
    pthread_join(reader, nullptr);
    for (int i = 0; i < n; i++) {
        pthread_join(decryptors[i], nullptr);
    }
    sem_post(decrypted_slots);
    pthread_join(writer, nullptr);

    cout << "Результат записан в файл: decrypted.txt" << endl;

    // Удаляем семафоры
    sem_close(empty_slots);
    sem_close(filled_slots);
    sem_close(decrypted_slots);

    sem_unlink("/empty_slots_decrypt");
    sem_unlink("/filled_slots_decrypt");
    sem_unlink("/decrypted_slots");

    pthread_mutex_destroy(&buffer_mutex);

    return 0;
}
