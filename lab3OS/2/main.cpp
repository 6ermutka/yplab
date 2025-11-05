// 2. Дан файл, содержащий текст произвольной длины и ключ длиной k символов. Выполнить шифрование этого текста по алгоритму,
// рассмотренному в задаче № 5 лабораторной работы № 1. Для решения задачи реализовать три потока:
//- поток чтения информации из файла в буфер;
//- поток шифрования текста;
//- поток вывода шифрованного текста в результирующий файл.
#include <iostream>
#include <fstream>
#include <string>
#include <pthread.h>
#include <algorithm>

using namespace std;

pthread_mutex_t mutex_reader;      // для чтения данных
pthread_mutex_t mutex_encryptor;   // для шифрования
pthread_mutex_t mutex_writer;      // для записи
pthread_mutex_t mutex_buffer;      // для доступа к buffer
pthread_mutex_t mutex_encrypted;   // для доступа к encrypted_buffer
pthread_mutex_t mutex_file_end;    // для доступа к file_end

string buffer;
string encrypted_buffer;
bool file_end = false;
string key;
int k;

void* file_reader(void* arg) {
    ifstream file("/Users/stepanivanov/CLionProjects/untitled6/input.txt");
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
            pthread_mutex_unlock(&mutex_encryptor);
        } else {
            pthread_mutex_unlock(&mutex_reader);
        }
    }
    if (!block.empty()) {
        pthread_mutex_lock(&mutex_reader);
        pthread_mutex_lock(&mutex_buffer);
        buffer = block;
        pthread_mutex_unlock(&mutex_buffer);
        pthread_mutex_unlock(&mutex_encryptor);
    }
    pthread_mutex_lock(&mutex_file_end);
    file_end = true;
    pthread_mutex_unlock(&mutex_file_end);
    pthread_mutex_unlock(&mutex_encryptor);
    file.close();
    cout << "Чтение файла завершено" << endl;
    return NULL;
}
void* encryptor(void* arg) {
    while (true) {
        pthread_mutex_lock(&mutex_encryptor);
        pthread_mutex_lock(&mutex_file_end);
        bool end = file_end;
        pthread_mutex_unlock(&mutex_file_end);
        pthread_mutex_lock(&mutex_buffer);
        string block = buffer;
        buffer.clear();
        pthread_mutex_unlock(&mutex_buffer);
        if (end && block.empty()) {
            pthread_mutex_lock(&mutex_encrypted);
            encrypted_buffer = "EOF";
            pthread_mutex_unlock(&mutex_encrypted);
            pthread_mutex_unlock(&mutex_writer);
            break;
        }
        if (!block.empty()) {
            string encrypted_block = block;
            reverse(encrypted_block.begin(), encrypted_block.end());
            for (size_t i = 0; i < encrypted_block.length(); i++) {
                encrypted_block[i] = encrypted_block[i] ^ key[i % k];
            }
            pthread_mutex_lock(&mutex_encrypted);
            encrypted_buffer = encrypted_block;
            pthread_mutex_unlock(&mutex_encrypted);
            pthread_mutex_unlock(&mutex_writer);
        } else {
            pthread_mutex_unlock(&mutex_encryptor);
        }
    }
    cout << "Шифрование завершено" << endl;
    return NULL;
}
void* file_writer(void* arg) {
    ofstream file("/Users/stepanivanov/CLionProjects/untitled6/output.txt");
    if (!file.is_open()) {
        cout << "Ошибка создания файла!" << endl;
        return NULL;
    }
    while (true) {
        pthread_mutex_lock(&mutex_writer);
        pthread_mutex_lock(&mutex_encrypted);
        string block = encrypted_buffer;
        encrypted_buffer.clear();
        pthread_mutex_unlock(&mutex_encrypted);
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
            pthread_mutex_unlock(&mutex_encryptor);
        }
    }
    file.close();
    cout << "Запись в файл завершена" << endl;
    return NULL;
}

int main() {
    cout << "Введите длину ключа k: ";
    cin >> k;
    cout << "Введите ключ: ";
    cin >> key;
    if (key.length() != k) {
        cout << "Ошибка: длина ключа должна быть равна " << k << "!" << endl;
        return 1;
    }
    pthread_mutex_init(&mutex_reader, NULL);
    pthread_mutex_init(&mutex_encryptor, NULL);
    pthread_mutex_init(&mutex_writer, NULL);
    pthread_mutex_init(&mutex_buffer, NULL);
    pthread_mutex_init(&mutex_encrypted, NULL);
    pthread_mutex_init(&mutex_file_end, NULL);
    pthread_mutex_lock(&mutex_encryptor);
    pthread_mutex_lock(&mutex_writer);
    pthread_t reader_thread, encryptor_thread, writer_thread;
    pthread_create(&reader_thread, NULL, file_reader, NULL);
    pthread_create(&encryptor_thread, NULL, encryptor, NULL);
    pthread_create(&writer_thread, NULL, file_writer, NULL);
    pthread_join(reader_thread, NULL);
    pthread_join(encryptor_thread, NULL);
    pthread_join(writer_thread, NULL);
    pthread_mutex_destroy(&mutex_reader);
    pthread_mutex_destroy(&mutex_encryptor);
    pthread_mutex_destroy(&mutex_writer);
    pthread_mutex_destroy(&mutex_buffer);
    pthread_mutex_destroy(&mutex_encrypted);
    pthread_mutex_destroy(&mutex_file_end);
    cout << "Программа завершена успешно" << endl;
    return 0;
}