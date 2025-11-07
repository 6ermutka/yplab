#include <iostream>
#include <pthread.h>
#include <cstdlib>
#include <ctime>

using namespace std;

class CMonitor {
private:
    int table_size;
    int* occupied_rows;
    pthread_mutex_t* row_mutexes;

public:
    CMonitor(int size) {
        table_size = size;
        occupied_rows = new int[table_size];
        row_mutexes = new pthread_mutex_t[table_size];
        for (int i = 0; i < table_size; i++) {
            occupied_rows[i] = 0;
            pthread_mutex_init(&row_mutexes[i], NULL);
        }
    }

    ~CMonitor() {
        for (int i = 0; i < table_size; i++) {
            pthread_mutex_destroy(&row_mutexes[i]);
        }
        delete[] occupied_rows;
        delete[] row_mutexes;
    }

    void OccupyRow(int row_number) {
        if (row_number < 0 || row_number >= table_size) {
            cout << "Ошибка: неверный номер строки " << row_number << endl;
            return;
        }
        pthread_mutex_lock(&row_mutexes[row_number]);
        occupied_rows[row_number] = 1;
    }

    void FreeRow(int row_number) {
        if (row_number < 0 || row_number >= table_size) {
            cout << "Ошибка: неверный номер строки " << row_number << endl;
            return;
        }
        occupied_rows[row_number] = 0;
        pthread_mutex_unlock(&row_mutexes[row_number]);
    }
};

int TABLE_SIZE;
int** hash_table = nullptr;
int* table_sizes = nullptr;
int* table_capacities = nullptr;

CMonitor* table_monitor = nullptr;
int n;
int numbers_per_thread;
int hash_base;

int hash_func(int x) {
    return x % hash_base;
}

void add_to_hash_table(int value) {
    int index = hash_func(value);
    table_monitor->OccupyRow(index);
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
    table_monitor->FreeRow(index);
}

void* thread_function(void* arg) {
    for (int i = 0; i < numbers_per_thread; i++) {
        int random_number = rand() % 1000;
        add_to_hash_table(random_number);
    }
    return NULL;
}

void print_hash_table() {
    cout << "\n=== ХЕШ-ТАБЛИЦА (строк: " << TABLE_SIZE << ", основание: " << hash_base << ") ===" << endl;
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
            cout << " (" << table_sizes[i] << " элементов)";
        }
        cout << endl;
    }
}

void initialize_hash_table(int size) {
    TABLE_SIZE = size;
    hash_table = new int*[TABLE_SIZE];
    table_sizes = new int[TABLE_SIZE];
    table_capacities = new int[TABLE_SIZE];

    for (int i = 0; i < TABLE_SIZE; i++) {
        hash_table[i] = nullptr;
        table_sizes[i] = 0;
        table_capacities[i] = 0;
    }
}

void cleanup_hash_table() {
    for (int i = 0; i < TABLE_SIZE; i++) {
        if (hash_table[i] != nullptr) {
            delete[] hash_table[i];
        }
    }
    delete[] hash_table;
    delete[] table_sizes;
    delete[] table_capacities;
    delete table_monitor;
    hash_table = nullptr;
    table_sizes = nullptr;
    table_capacities = nullptr;
    table_monitor = nullptr;
}

int main() {
    srand(time(NULL));

    int table_size;
    cout << "Введите кол-во строк хеш-таблицы: ";
    cin >> table_size;

    initialize_hash_table(table_size);

    cout << "Введите основание хеш-функции: ";
    cin >> hash_base;
    cout << "Введите количество потоков (n): ";
    cin >> n;
    cout << "Введите количество чисел для генерации каждым потоком: ";
    cin >> numbers_per_thread;
    table_monitor = new CMonitor(TABLE_SIZE);

    pthread_t threads[n];
    int thread_ids[n];

    for (int i = 0; i < n; i++) {
        thread_ids[i] = i + 1;
        if (pthread_create(&threads[i], NULL, thread_function, &thread_ids[i]) != 0) {
            cerr << "Ошибка при создании потока " << i + 1 << endl;
            cleanup_hash_table();
            return 1;
        }
    }

    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], NULL);
    }

    print_hash_table();
    cleanup_hash_table();

    return 0;
}
