/*	homework ¹1
	fitsBits 
	Sergeev Evgeniy 171 gr*/

#include <stdio.h>
#include <stdint.h>

int fitsBist(int32_t,int32_t);

int main(void)
{
	printf("Enter a number and a power\n");
	int32_t x,n;
	scanf("%d%d", &x, &n);
	printf("%d\n", fitsBist(x,n) );
	return 0;
}

int fitsBist(int32_t x ,int32_t n)
{
	return !(((~x & (x >> 0x1F)) + (x & ~(x >> 0x1F))) >> (n + ~0));
}
