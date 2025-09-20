//Дано бинарное дерево, элементы которого содержат целые числа. Найдите сумму элементов этого дерева.
#include <iostream>
#include <string>
#include <pthread.h>
#include <cstdlib>
#include <algorithm>

using namespace std;

struct tree{
    int Data;
    tree *Left;
    tree *Right;
};
int k, x, ind = 1, total_sum = 0;
tree *Root;


void addElement(tree*& root, int value) {
    if (root == NULL) {
        root = new tree;
        (*root).Data = value;
        (*root).Left = NULL;
        (*root).Right = NULL;
    } else if (value < (*root).Data) {
        addElement((*root).Left, value);
    } else {
        addElement((*root).Right, value);
    }
}

void printTree(tree *root)
{
    if (root)
    {
        cout << (*root).Data << " ";
        printTree((*root).Left);
        printTree((*root).Right);
    }
}


void countInd(tree* root) {
    if (root == NULL) return;
    if ((*root).Left == NULL && (*root).Right == NULL) {
        ind++;
    } else {
        countInd((*root).Left);
        countInd((*root).Right);
    }
}

void* function(void* args) {
    int N = *((int*)args);
    int sum = 0;
    tree* current = Root;
    for(int i=0; i < N;i++)  current = (*current).Right;
    while (current != NULL) {
        sum += (*current).Data;
        current = (*current).Left;
    }
    total_sum += sum;
    return NULL;
}

int main() {
    srand(time(0));
    Root = new tree;
    (*Root).Left = NULL;
    (*Root).Right = NULL;
    cout << "Введите кол-во элементов в дереве: ";
    cin >> k;
    cout << "Введите корневое значение элемента в дереве: ";
    cin >> x;
    (*Root).Data = x;
    for(int i=1; i < k; i++){
        cout << "Введите значение " << i << " элемента дерева: ";
        cin >> x;
        addElement(Root, x);
    }
    printTree(Root);
    countInd(Root);
    pthread_t threads[ind];
    int thread_args[ind];
    for(int i=0; i < ind; i++){
        thread_args[i] = i;
        pthread_create(
                &threads[i],
                NULL,
                function,
                &thread_args[i]);
    }
    for (int i = 0; i < ind; i++) {
        pthread_join(threads[i], NULL);
    }
    cout << endl << "Сумма элементов: " << total_sum;

    return 0;
}
