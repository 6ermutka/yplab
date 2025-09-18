//Дан текст длиной n символов и ключ длиной k символов. Осуществите блочное шифрование текста по следующему алгоритму:
//- Разделить текст на блоки длиной k символов. Если n не кратно k, то допустимо, чтобы длина последнего блока была меньше k.
//- Внутри каждого блока выполнить перестановку символов так, чтобы первый символ занял место последнего, второй – предпоследнего и т.д. Последний символ должен оказаться на месте первого символа блока.
//- Применить ключ к каждому блоку. Шифрованный i-й символ блока должен быть получен, как результат исключающего или между i-м исходным символом блока и i-м символом ключа.
#include <iostream>
#include <string>
#include <pthread.h>
#include <cstdlib>
#include <algorithm>

using namespace std;

string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
string text;
string key;
string* blocks = nullptr;
int n, k;

void* function(void* args) {
    int block_index = *((int*)args);
    string str = blocks[block_index];
    string temp = str;
    for (int i = 0; i < str.length(); i++) {
        temp[i] = str[i] ^ key[i];
    }
    blocks[block_index] = temp;
    cout << "Зашифрованный блок " << block_index << ": " << temp << endl;

    return NULL;
}

int main() {
    srand(time(0));
    cout << "Введите число n: ";
    cin >> n;
    cout << "Введите число k: ";
    cin >> k;

    for(int i = 0; i < n; i++) text += characters[rand() % 63];
    for(int i = 0; i < k; i++) key += characters[rand() % 63];

    cout << "Исходный текст: " << text << endl;
    cout << "Ключ: " << key << endl;

    int num_blocks = (n + k - 1) / k;
    blocks = new string[num_blocks];

    for (int i = 0; i < num_blocks; i++) {
        int start = i * k;
        int end = min(start + k, n);
        blocks[i] = text.substr(start, end - start);
    }

    pthread_t threads[num_blocks];
    int thread_args[num_blocks];

    for (int i = 0; i < num_blocks; i++) {
        thread_args[i] = i;
        int result = pthread_create(
                &threads[i],
                nullptr, 
                function, 
                &thread_args[i]
                );
        if (result != 0) {
            cout << "Ошибка создания потока " << i << ": " << result << endl;
        }
    }

    for (int i = 0; i < num_blocks; i++) {
        pthread_join(
                threads[i], 
                nullptr
                );
    }
    
    string encrypted_text;
    for (int i = 0; i < num_blocks; i++) {
        encrypted_text += blocks[i];
    }
    cout << "Зашифрованный текст: " << encrypted_text << endl;

    delete[] blocks;
    return 0;
}
