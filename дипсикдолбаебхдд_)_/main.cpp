#include <iostream>
#include <pthread.h>
#include <vector>
#include <deque>
#include <random>
#include <fstream>
#include <string>
#include <chrono>
#include <thread>

class ThreadSafeDeque {
private:
    std::deque<std::string> deque;
    pthread_mutex_t mutex;
    pthread_cond_t cond_front;
    pthread_cond_t cond_back;
    bool front_locked;
    bool back_locked;

public:
    ThreadSafeDeque() {
        pthread_mutex_init(&mutex, nullptr);
        pthread_cond_init(&cond_front, nullptr);
        pthread_cond_init(&cond_back, nullptr);
        front_locked = false;
        back_locked = false;
    }

    ~ThreadSafeDeque() {
        pthread_mutex_destroy(&mutex);
        pthread_cond_destroy(&cond_front);
        pthread_cond_destroy(&cond_back);
    }

    void push_front(const std::string& element) {
        pthread_mutex_lock(&mutex);
        
        // Ждем, пока освободится доступ к началу дека
        while (front_locked) {
            pthread_cond_wait(&cond_front, &mutex);
        }
        
        // Блокируем начало дека
        front_locked = true;
        pthread_mutex_unlock(&mutex);

        // Имитируем работу с деком (можно добавить небольшую задержку)
        std::this_thread::sleep_for(std::chrono::milliseconds(1));
        
        pthread_mutex_lock(&mutex);
        deque.push_front(element);
        std::cout << "Поток " << pthread_self() << " добавил в начало: " << element << std::endl;
        
        // Разблокируем начало дека и уведомляем ожидающие потоки
        front_locked = false;
        pthread_cond_broadcast(&cond_front);
        pthread_mutex_unlock(&mutex);
    }

    void push_back(const std::string& element) {
        pthread_mutex_lock(&mutex);
        
        // Ждем, пока освободится доступ к концу дека
        while (back_locked) {
            pthread_cond_wait(&cond_back, &mutex);
        }
        
        // Блокируем конец дека
        back_locked = true;
        pthread_mutex_unlock(&mutex);

        // Имитируем работу с деком
        std::this_thread::sleep_for(std::chrono::milliseconds(1));
        
        pthread_mutex_lock(&mutex);
        deque.push_back(element);
        std::cout << "Поток " << pthread_self() << " добавил в конец: " << element << std::endl;
        
        // Разблокируем конец дека и уведомляем ожидающие потоки
        back_locked = false;
        pthread_cond_broadcast(&cond_back);
        pthread_mutex_unlock(&mutex);
    }

    void print_deque() {
        pthread_mutex_lock(&mutex);
        std::cout << "\nСодержимое дека (" << deque.size() << " элементов):" << std::endl;
        for (size_t i = 0; i < deque.size(); ++i) {
            std::cout << "[" << i << "]: " << deque[i] << std::endl;
        }
        pthread_mutex_unlock(&mutex);
    }

    size_t size() {
        pthread_mutex_lock(&mutex);
        size_t result = deque.size();
        pthread_mutex_unlock(&mutex);
        return result;
    }
};

struct ThreadData {
    ThreadSafeDeque* deque;
    std::vector<std::string>* elements;
    int thread_id;
    int num_threads;
};

// Генератор случайных чисел для выбора места вставки
bool get_random_insert_position() {
    static std::random_device rd;
    static std::mt19937 gen(rd());
    static std::uniform_int_distribution<> dis(0, 1);
    return dis(gen) == 0; // true - в начало, false - в конец
}

void* worker_thread(void* arg) {
    ThreadData* data = static_cast<ThreadData*>(arg);
    ThreadSafeDeque* deque = data->deque;
    std::vector<std::string>* elements = data->elements;
    int thread_id = data->thread_id;
    int num_threads = data->num_threads;

    // Каждый поток обрабатывает свою часть элементов
    for (size_t i = thread_id; i < elements->size(); i += num_threads) {
        const std::string& element = (*elements)[i];
        
        bool insert_at_front = get_random_insert_position();
        
        if (insert_at_front) {
            deque->push_front(element);
        } else {
            deque->push_back(element);
        }
    }

    return nullptr;
}

std::vector<std::string> read_file_elements(const std::string& filename) {
    std::vector<std::string> elements;
    std::ifstream file(filename);
    std::string line;
    
    if (!file.is_open()) {
        std::cerr << "Ошибка: не удалось открыть файл " << filename << std::endl;
        return elements;
    }
    
    while (std::getline(file, line)) {
        if (!line.empty()) {
            elements.push_back(line);
        }
    }
    
    file.close();
    return elements;
}

int main(int argc, char* argv[]) {
    if (argc != 3) {
        std::cout << "Использование: " << argv[0] << " <файл> <количество_потоков>" << std::endl;
        return 1;
    }

    std::string filename = argv[1];
    int num_threads = std::stoi(argv[2]);

    if (num_threads <= 0) {
        std::cerr << "Ошибка: количество потоков должно быть положительным числом" << std::endl;
        return 1;
    }

    // Чтение элементов из файла
    std::vector<std::string> elements = read_file_elements(filename);
    if (elements.empty()) {
        std::cerr << "Файл пуст или не содержит данных" << std::endl;
        return 1;
    }

    std::cout << "Прочитано " << elements.size() << " элементов из файла" << std::endl;
    std::cout << "Используется " << num_threads << " потоков" << std::endl;

    ThreadSafeDeque deque;
    std::vector<pthread_t> threads(num_threads);
    std::vector<ThreadData> thread_data(num_threads);

    auto start_time = std::chrono::high_resolution_clock::now();

    // Создание потоков
    for (int i = 0; i < num_threads; ++i) {
        thread_data[i] = {&deque, &elements, i, num_threads};
        if (pthread_create(&threads[i], nullptr, worker_thread, &thread_data[i]) != 0) {
            std::cerr << "Ошибка при создании потока " << i << std::endl;
            return 1;
        }
    }

    // Ожидание завершения всех потоков
    for (int i = 0; i < num_threads; ++i) {
        pthread_join(threads[i], nullptr);
    }

    auto end_time = std::chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end_time - start_time);

    // Вывод результатов
    deque.print_deque();
    std::cout << "\nВремя выполнения: " << duration.count() << " мс" << std::endl;
    std::cout << "Всего элементов в деке: " << deque.size() << std::endl;

    return 0;
}
