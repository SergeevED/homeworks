/*	divide float into sign, exp and mant
	using union		
	Sergeev Evgeniy  171 gr             */


#include <stdio.h>
#include <stdint.h>

int main()
{
	union value
	{
		int32_t ival;
		float fval;
	} data;

	data.fval = 0.0f;
	printf("Enter float\n");
	scanf("%f", &data.fval);
	int32_t sign, exp, mant;
	sign = (int)(data.ival >> 31) & 0x1;
	exp =  (int)(data.ival >> 23) & 0xFF;
	mant = (int)data.ival & 0x7FFFFF;
	
	if (exp == 255 && mant == 0) 
	{
		if (sign > 0) printf("+ Infinity\n");
		else printf("- Infinity\n");
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
			float tempMant = (mant >> (22 - i)) & 1;
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