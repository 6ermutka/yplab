//4. Имеется n потоков, генерирующих случайные целые числа. Занести эти числа в хеш-таблицу, представляющую собой массив из 10 указателей на списки целых чисел.
// Для определения местоположения числа x в хеш-таблице использовать следующую хеш-функцию:
// Если поток собирается записать число x в k-ю строку таблицы, то он должен проверить, не пишет ли другой поток свое число в эту же строку.

#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <ctime>


using namespace std;

// Хеш-таблица - массив из 10 указателей на массивы (списки)
const int TABLE_SIZE = 10;
int* hash_table[TABLE_SIZE]; // указатели на массивы чисел
int table_sizes[TABLE_SIZE] = {0}; // текущие размеры каждого списка
int table_capacities[TABLE_SIZE] = {0}; // емкости каждого списка

pthread_mutex_t table_mutexes[TABLE_SIZE];
int n;
int numbers_per_thread;

int hash_func(int x) {
    return x % TABLE_SIZE;
}

void add_to_hash_table(int value) {
    int index = hash_func(value);
    pthread_mutex_lock(&table_mutexes[index]);
    if (table_capacities[index] == 0) {
        table_capacities[index] = 10;
        hash_table[index] = new int[table_capacities[index]];
    }
    if (table_sizes[index] >= table_capacities[index]) {
        int new_capacity = table_capacities[index] * 2;
        int* new_array = new int[new_capacity];
        for (int i = 0; i < table_sizes[index]; i++) {
            new_array[i] = hash_table[index][i];
        }
        delete[] hash_table[index];
        hash_table[index] = new_array;
        table_capacities[index] = new_capacity;
    }
    hash_table[index][table_sizes[index]] = value;
    table_sizes[index]++;
    pthread_mutex_unlock(&table_mutexes[index]);
}

void* thread_function(void* arg) {
    for (int i = 0; i < numbers_per_thread; i++) {
        int random_number = rand() % 1000;
        add_to_hash_table(random_number);
    }
    return NULL;
}

void print_hash_table() {
    for (int i = 0; i < TABLE_SIZE; i++) {
        cout << "[" << i << "]: ";
        if (table_sizes[i] == 0) {
            cout << "пусто";
        } else {
            for (int j = 0; j < table_sizes[i]; j++) {
                cout << hash_table[i][j];
                if (j < table_sizes[i] - 1) {
                    cout << " ";
                }
            }
        }
        cout << endl;
    }
}
void cleanup_hash_table() {
    for (int i = 0; i < TABLE_SIZE; i++) {
        if (hash_table[i] != nullptr) {
            delete[] hash_table[i];
            hash_table[i] = nullptr;
        }
        table_sizes[i] = 0;
        table_capacities[i] = 0;
    }
}

int main() {
    srand(time(NULL));
    for (int i = 0; i < TABLE_SIZE; i++) {
        hash_table[i] = nullptr;
        table_sizes[i] = 0;
        table_capacities[i] = 0;
    }
    for (int i = 0; i < TABLE_SIZE; i++) {
        pthread_mutex_init(&table_mutexes[i], NULL);
    }
    cout << "Введите количество потоков (n): ";
    cin >> n;
    cout << "Введите количество чисел для генерации каждым потоком: ";
    cin >> numbers_per_thread;
    pthread_t threads[n];
    int thread_ids[n];
    for (int i = 0; i < n; i++) {
        thread_ids[i] = i + 1;
        if (pthread_create(&threads[i], NULL, thread_function, &thread_ids[i]) != 0) {
            cerr << "Ошибка при создании потока " << i + 1 << endl;
            return 1;
        }
    }
    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], NULL);
    }
    print_hash_table();
    cleanup_hash_table();
    for (int i = 0; i < TABLE_SIZE; i++) {
        pthread_mutex_destroy(&table_mutexes[i]);
    }

    return 0;
}