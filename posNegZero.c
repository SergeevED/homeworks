/* homework ¹1
	is number positive, negative or equal zero
	Sergeev Evgeniy 171 gr			*/

#include <stdio.h>
#include <stdint.h>

int posNegZero(int32_t);

int main(void)
{
	printf("Enter an integer number\n");
	int32_t n = 0;
	scanf("%d", &n);
	printf ("%d\n", posNegZero(n) );
	return(0);
}

int posNegZero(int32_t n)
{
	return ( (n>>0x1F) + !( n>>0x1F) + ~( !n + ~0 ) );
}