#include <stdio.h>
#include <stdlib.h>
#include <time.h>

int main(int argc, char **argv) {
    char key[17] = "AaAaAaAaEhGfEeFc";
    char offset = 0;
    int i;

    srand(time(NULL));
    offset = (rand() % 19);

    for (i = 0; i < 16; i++) {
        key[i] += offset;
    }

    printf("Key: %s\n", key);
}