#include <iostream>
#include <pthread.h>
#include <ctime>
#include <iomanip>
#include <cmath>

using namespace std;

double total_area = 0.0;
pthread_mutex_t mutexx = PTHREAD_MUTEX_INITIALIZER;
double a, b;
int n, m;

double f(double x) {
    return sin(x) + 2;
}

void* calculate_area(void* arg) {
    int N = *((int*)arg);
    double width = (b - a) / n;
    double start_x = a + N * width;
    double local_area = 0.0;
    double step = width / m;

    for (int i = 0; i < m; i++) {
        double x_left = start_x + i * step;
        double x_right = x_left + step;
        double midpoint = (x_left + x_right) / 2.0;
        local_area += f(midpoint) * step;
    }
    pthread_mutex_lock(&mutexx);
    total_area += local_area;
    pthread_mutex_unlock(&mutexx);

    return NULL;
}

int main() {

    cout << "Введите начало отрезка (a): ";
    cin >> a;
    cout << "Введите конец отрезка (b): ";
    cin >> b;
    cout << "Введите количество элементов/потоков (n): ";
    cin >> n;
    cout << "Введите количество разбиений для метода прямоугольников (m): ";
    cin >> m;
    cout << "Отрезок: [" << a << ", " << b << "]" << endl;
    pthread_t threads[n];
    int thread_ids[n];

    for (int i = 0; i < n; i++) {
        thread_ids[i] = i;
        if (pthread_create(&threads[i], NULL, calculate_area, &thread_ids[i]) != 0) {
            cerr << "Ошибка при создании потока " << i << endl;
            return 1;
        }
    }
    for (int i = 0; i < n; i++) {
        pthread_join(threads[i], NULL);
    }
    cout << "Приближенная площадь: " << fixed << setprecision(6) << total_area << endl;

    pthread_mutex_destroy(&mutexx);

    return 0;
}
