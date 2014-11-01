/*	homework ¹1
	raise number to power 
	Sergeev Evgeniy 171 gr*/

#include <stdio.h>
#include <stdint.h>

int64_t pow_mine(int32_t,int32_t);

int main(void)
{
	printf("Enter a number and a power\n");
	int32_t a,n;
	scanf("%d%d", &a, &n);
	printf ("%ld\n", pow_mine(a,n) );
	return 0;
}

int64_t pow_mine(int32_t a, int32_t n)
{
	static int64_t subRes = 1;
	if (!n) return 1;
	subRes=pow_mine(a, n >> 1);
	subRes*=subRes;
	if (n & 1)
	{
		subRes *= a;
	}
	return subRes;
}
