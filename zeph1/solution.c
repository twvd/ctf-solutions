// http://ctf.tf/ctfs/zeph1/

#include <string.h>
#include <stdio.h>

int main(int argc, char **argv) {
	char name[40];
	int i;
	unsigned int result;

	printf("Enter name: ");
	scanf("%s", name);

	// Part 1
	result = 0;
	for (i = 0; i < strlen(name); i++) {
		result += name[i];
		result *= name[i];
		if (name[i] < 'J') {
			result *= 2;
		}
	}

	result %= 0x724;

	// Part 2
	for (i = 0; i < strlen(name); i++) {
		result += name[i];
		result *= name[i];
		if (name[i] < '@') {
			result *= 4;
		}
	}

	result %= 0x2225;

	// Part 3
	for (i = 0; i < strlen(name); i++) {
		result += name[i];
		result *= name[i];
		if (name[i] < 'T') {
			result *= 6;
		}
	}

	// Part 4
	for (i = 0; i < strlen(name); i++) {
		result += name[i];
		result *= name[i];
		if (name[i] < 'J') {
			result *= 5;
		}
	}
	result %= 0x2E34;

	// Part 5
	for (i = 0; i < strlen(name); i++) {
		result += name[i];
		result *= name[i];
		if (name[i] < '@') {
			result *= 7;
		}
	}

	printf("Key: %X%lu\n", result, result);

    return 0;
}
