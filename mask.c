/* homework  ¹1
	x10 to x2 numeration 
	Sergeev Evgeniy 171 gr */

#include <stdio.h>
#include <stdint.h>

int main(void)
{
	printf("Enter an integer number\n");
	int32_t n;
	scanf("%d", &n);
	uint32_t mask=0x80000000;
	while (mask != 0)
	{
		printf("%d", (n&mask) != 0);
		mask = mask >> 1;
	}
	printf("\n");
	return 0;
}
