/*	divide float into sign, exp and mant
	using pointer		
	Sergeev Evgeniy  171 gr             */

#include <stdio.h>
#include <stdint.h>

int main()
{
	int32_t ival;
	float fval;
	printf("Enter float\n");
	scanf("%f", &fval); 
	ival = *((int*)&fval);
	int32_t sign, exp, mant;
	sign = ival >> 31 & 1;
	exp = ival >> 23 & ((1 << 8) - 1);
	mant = ival & ((1 << 23) - 1);
	
	if (exp == 255 && mant == 0) 
	{
		if (sign > 0) printf("Positive infinity\n");
		else printf("Negative infinity\n");
	}
	else if (exp == 255 && mant != 0) 
	{
		printf("NaN\n");
	}

	else 
	{
		(sign == 0) ? printf("Sign\n+\n") : printf("Sign\n-\n");
		printf("Exp\n");
		printf("2^%d\n", exp - 127); 
		float flMant = 0.0f;
		for (int i = 0; i < 23; i++)
		{
			float tempMant = mant >> (22 - i) & 1;
			if (tempMant != 0)
				for (int j = i; j >= 0 ; j--)
				{
					tempMant /= 2.0;
				}
				flMant += tempMant;
		}	
		printf("Mant\n%f\n", 1 + flMant );
	}

	return 0;
}

