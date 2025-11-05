//Дан файл содержащий текст произвольной длины. Найти контрольную сумму текста, как сумму кодов символов по модулю 256.
// Реализовать два потока: первый для чтения содержимого файла буфер длиной m символов, второй – для расчета контрольной суммы.

#include <stdio.h>
#include <pthread.h>

pthread_mutex_t mutex_reader;    // для синхронизации чтения
pthread_mutex_t mutex_calculator; // для синхронизации расчета
pthread_mutex_t mutex_data;       // для доступа к общим данным

char character;
int data_available = 0;
int file_end = 0;
unsigned char checksum = 0;

void* file_reader(void* arg) {
    FILE* file = fopen("/Users/stepanivanov/CLionProjects/untitled6/test.txt", "r");
    if (!file) {
        printf("Ошибка открытия файла\n");
        return NULL;
    }
    while (1) {
        pthread_mutex_lock(&mutex_reader);
        pthread_mutex_lock(&mutex_data);
        int ch = fgetc(file);
        if (ch == EOF) {
            file_end = 1;
            pthread_mutex_unlock(&mutex_data);
            pthread_mutex_unlock(&mutex_calculator);
            break;
        }
        character = (char)ch;
        data_available = 1;
        pthread_mutex_unlock(&mutex_data);
        pthread_mutex_unlock(&mutex_calculator);
    }
    fclose(file);
    return NULL;
}

void* checksum_calculator(void* arg) {
    while (1) {
        pthread_mutex_lock(&mutex_calculator);
        pthread_mutex_lock(&mutex_data);
        if (file_end && data_available == 0) {
            pthread_mutex_unlock(&mutex_data);
            break;
        }
        if (data_available) {
            checksum = (checksum + (unsigned char)character) % 256;
            data_available = 0;
        }
        pthread_mutex_unlock(&mutex_data);
        pthread_mutex_unlock(&mutex_reader);
    }
    return NULL;
}

int main() {
    pthread_mutex_init(&mutex_reader, NULL);
    pthread_mutex_init(&mutex_calculator, NULL);
    pthread_mutex_init(&mutex_data, NULL);
    pthread_mutex_lock(&mutex_calculator);
    pthread_t reader_thread, calculator_thread;
    pthread_create(&reader_thread, NULL, file_reader, NULL);
    pthread_create(&calculator_thread, NULL, checksum_calculator, NULL);
    pthread_join(reader_thread, NULL);
    pthread_join(calculator_thread, NULL);
    printf("Контрольная сумма файла: %d\n", checksum);
    pthread_mutex_destroy(&mutex_reader);
    pthread_mutex_destroy(&mutex_calculator);
    pthread_mutex_destroy(&mutex_data);
}