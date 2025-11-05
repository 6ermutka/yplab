// Дешифрование файла, зашифрованного по алгоритму из задачи №2
#include <iostream>
#include <fstream>
#include <string>
#include <pthread.h>
#include <algorithm>

using namespace std;

pthread_mutex_t mutex_reader;      // для чтения данных
pthread_mutex_t mutex_decryptor;   // для дешифрования
pthread_mutex_t mutex_writer;      // для записи
pthread_mutex_t mutex_buffer;      // для доступа к buffer
pthread_mutex_t mutex_decrypted;   // для доступа к decrypted_buffer
pthread_mutex_t mutex_file_end;    // для доступа к file_end

string buffer;
string decrypted_buffer;
bool file_end = false;
string key;
int k;

void* file_reader(void* arg) {
    ifstream file("/Users/stepanivanov/CLionProjects/untitled6/output.txt");
    if (!file.is_open()) {
        cout << "Ошибка открытия файла!" << endl;
        return NULL;
    }
    string block;
    char ch;
    int count = 0;
    while (file.get(ch)) {
        pthread_mutex_lock(&mutex_reader);
        block += ch;
        count++;
        if (count == k) {
            pthread_mutex_lock(&mutex_buffer);
            buffer = block;
            pthread_mutex_unlock(&mutex_buffer);
            block = "";
            count = 0;
            pthread_mutex_unlock(&mutex_decryptor);
        } else {
            pthread_mutex_unlock(&mutex_reader);
        }
    }
    if (!block.empty()) {
        pthread_mutex_lock(&mutex_reader);
        pthread_mutex_lock(&mutex_buffer);
        buffer = block;
        pthread_mutex_unlock(&mutex_buffer);
        pthread_mutex_unlock(&mutex_decryptor);
    }
    pthread_mutex_lock(&mutex_file_end);
    file_end = true;
    pthread_mutex_unlock(&mutex_file_end);
    pthread_mutex_unlock(&mutex_decryptor);
    file.close();
    cout << "Чтение зашифрованного файла завершено" << endl;
    return NULL;
}

void* decryptor(void* arg) {
    while (true) {
        pthread_mutex_lock(&mutex_decryptor);
        pthread_mutex_lock(&mutex_file_end);
        bool end = file_end;
        pthread_mutex_unlock(&mutex_file_end);
        pthread_mutex_lock(&mutex_buffer);
        string block = buffer;
        buffer.clear();
        pthread_mutex_unlock(&mutex_buffer);
        if (end && block.empty()) {
            pthread_mutex_lock(&mutex_decrypted);
            decrypted_buffer = "EOF";
            pthread_mutex_unlock(&mutex_decrypted);
            pthread_mutex_unlock(&mutex_writer);
            break;
        }
        if (!block.empty()) {
            string decrypted_block = block;
            for (size_t i = 0; i < decrypted_block.length(); i++) {
                decrypted_block[i] = decrypted_block[i] ^ key[i % k];
            }
            reverse(decrypted_block.begin(), decrypted_block.end());

            pthread_mutex_lock(&mutex_decrypted);
            decrypted_buffer = decrypted_block;
            pthread_mutex_unlock(&mutex_decrypted);
            pthread_mutex_unlock(&mutex_writer);
        } else {
            pthread_mutex_unlock(&mutex_decryptor);
        }
    }
    cout << "Дешифрование завершено" << endl;
    return NULL;
}

void* file_writer(void* arg) {
    ofstream file("/Users/stepanivanov/CLionProjects/untitled6/decrypted.txt");
    if (!file.is_open()) {
        cout << "Ошибка создания файла!" << endl;
        return NULL;
    }
    while (true) {
        pthread_mutex_lock(&mutex_writer);
        pthread_mutex_lock(&mutex_decrypted);
        string block = decrypted_buffer;
        decrypted_buffer.clear();
        pthread_mutex_unlock(&mutex_decrypted);
        if (block == "EOF") {
            break;
        }
        if (!block.empty()) {
            file << block;
            file.flush();
        }
        pthread_mutex_unlock(&mutex_reader);
        pthread_mutex_lock(&mutex_file_end);
        bool end = file_end;
        pthread_mutex_unlock(&mutex_file_end);
        if(end){
            pthread_mutex_unlock(&mutex_decryptor);
        }
    }
    file.close();
    cout << "Запись дешифрованного текста в файл завершена" << endl;
    return NULL;
}

int main() {
    cout << "Введите длину ключа k: ";
    cin >> k;
    cout << "Введите ключ для дешифрования: ";
    cin >> key;
    if (key.length() != k) {
        cout << "Ошибка: длина ключа должна быть равна " << k << "!" << endl;
        return 1;
    }
    pthread_mutex_init(&mutex_reader, NULL);
    pthread_mutex_init(&mutex_decryptor, NULL);
    pthread_mutex_init(&mutex_writer, NULL);
    pthread_mutex_init(&mutex_buffer, NULL);
    pthread_mutex_init(&mutex_decrypted, NULL);
    pthread_mutex_init(&mutex_file_end, NULL);
    pthread_mutex_lock(&mutex_decryptor);
    pthread_mutex_lock(&mutex_writer);
    pthread_t reader_thread, decryptor_thread, writer_thread;
    pthread_create(&reader_thread, NULL, file_reader, NULL);
    pthread_create(&decryptor_thread, NULL, decryptor, NULL);
    pthread_create(&writer_thread, NULL, file_writer, NULL);
    pthread_join(reader_thread, NULL);
    pthread_join(decryptor_thread, NULL);
    pthread_join(writer_thread, NULL);
    pthread_mutex_destroy(&mutex_reader);
    pthread_mutex_destroy(&mutex_decryptor);
    pthread_mutex_destroy(&mutex_writer);
    pthread_mutex_destroy(&mutex_buffer);
    pthread_mutex_destroy(&mutex_decrypted);
    pthread_mutex_destroy(&mutex_file_end);
    cout << "Дешифрование завершено успешно" << endl;
    cout << "Результат сохранен в файле decrypted.txt" << endl;

    return 0;
}