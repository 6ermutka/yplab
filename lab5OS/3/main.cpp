#include <iostream>
#include <fstream>
#include <pthread.h>
#include <string>

using namespace std;

// Файлы
string files[] = {
        "/Users/stepanivanov/CLionProjects/untitled6/1.txt",
        "/Users/stepanivanov/CLionProjects/untitled6/2.txt",
        "/Users/stepanivanov/CLionProjects/untitled6/3.txt"
};
string out_file = "/Users/stepanivanov/CLionProjects/untitled6/result.txt";


int values[3];
bool ready[3] = {false, false, false};
bool done[3] = {false, false, false};


pthread_mutex_t mutexes[3];
pthread_cond_t conds[3];
pthread_mutex_t writer_mutex = PTHREAD_MUTEX_INITIALIZER;
pthread_cond_t writer_cond = PTHREAD_COND_INITIALIZER;


void* reader(void* arg) {
    int id = *(int*)arg;

    // Каждый поток читает все 3 файла
    for (int file_num = 0; file_num < 3; file_num++) {
        ifstream file(files[file_num]);
        int num;

        while (file >> num) {
            // Проверяем критерий
            bool valid = false;
            if (id == 0) valid = (num > 0 && num % 2 == 0);      // четные положительные
            else if (id == 1) valid = (num > 0 && num % 2 != 0); // нечетные положительные
            else if (id == 2) valid = (num < 0);                 // отрицательные

            if (valid) {
                pthread_mutex_lock(&mutexes[id]);
                values[id] = num;
                ready[id] = true;
                pthread_mutex_unlock(&mutexes[id]);

                pthread_cond_signal(&writer_cond);

                pthread_mutex_lock(&mutexes[id]);
                while (ready[id]) {
                    pthread_cond_wait(&conds[id], &mutexes[id]);
                }
                pthread_mutex_unlock(&mutexes[id]);
            }
        }
        file.close();
    }

    done[id] = true;
    pthread_cond_signal(&writer_cond);
    pthread_exit(NULL);
}

void* writer(void* arg) {
    ofstream out(out_file);

    while (!(done[0] && done[1] && done[2])) {
        pthread_mutex_lock(&writer_mutex);
        while (!(ready[0] || ready[1] || ready[2]) && !(done[0] && done[1] && done[2])) {
            pthread_cond_wait(&writer_cond, &writer_mutex);
        }
        int min_val;
        int min_id = -1;
        bool found = false;

        for (int i = 0; i < 3; i++) {
            pthread_mutex_lock(&mutexes[i]);
            if (ready[i]) {
                if (!found || values[i] < min_val) {
                    min_val = values[i];
                    min_id = i;
                    found = true;
                }
            }
            pthread_mutex_unlock(&mutexes[i]);
        }

        if (found) {
            out << min_val << " ";
            pthread_mutex_lock(&mutexes[min_id]);
            ready[min_id] = false;
            pthread_mutex_unlock(&mutexes[min_id]);
            pthread_cond_signal(&conds[min_id]);
        }

        pthread_mutex_unlock(&writer_mutex);
    }

    out.close();
    pthread_exit(NULL);
}

int main() {
    for (int i = 0; i < 3; i++) {
        pthread_mutex_init(&mutexes[i], NULL);
        pthread_cond_init(&conds[i], NULL);
    }

    pthread_t readers[3];
    pthread_t writer_thread;
    int ids[3] = {0, 1, 2};

    cout << "Поток 0: четные положительные" << endl;
    cout << "Поток 1: нечетные положительные" << endl;
    cout << "Поток 2: отрицательные" << endl;

    for (int i = 0; i < 3; i++) {
        pthread_create(&readers[i], NULL, reader, &ids[i]);
    }
    pthread_create(&writer_thread, NULL, writer, NULL);

    for (int i = 0; i < 3; i++) {
        pthread_join(readers[i], NULL);
    }
    pthread_join(writer_thread, NULL);
    for (int i = 0; i < 3; i++) {
        pthread_mutex_destroy(&mutexes[i]);
        pthread_cond_destroy(&conds[i]);
    }
    pthread_mutex_destroy(&writer_mutex);
    pthread_cond_destroy(&writer_cond);

    return 0;
}
