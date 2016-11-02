/*	divide float into sign, exp and mant
	using byte fields
	Sergeev Evgeniy  171 gr             */

#include <stdio.h>
#include <stdint.h>

int main()
{
	union value
	{
		float fval;
		struct fieldVal
		{
			unsigned mant:  23;
			unsigned exp:   8;
			unsigned sign:  1; 
		} bval;
	} data;
	data.fval = 0.0f;
	printf("Enter float\n");
	scanf("%f", &data.fval);

	if (data.bval.exp == 255 && data.bval.mant == 0) 
	{
		if (data.bval.sign > 0) printf("Positive infinity\n");
		else printf("Negative infinity\n");
	}
	else if (data.bval.exp == 255 && data.bval.mant != 0)
	{
		printf("NaN\n");
	}
	else 
	{
		(data.bval.sign == 0) ? printf("Sign\n+\n") : printf("Sign\n-\n");
		printf("Exp\n");
		printf("2^%d\n", data.bval.exp - 127); 
		float flMant = 0.0f;
		for (int i = 0; i < 23; i++)
		{
			float tempMant = data.bval.mant >> (22 - i) & 1;
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
