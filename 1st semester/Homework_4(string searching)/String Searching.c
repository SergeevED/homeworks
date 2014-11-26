/*	Counting number of concurrences of substring of a string
	Sergeev Evgeniy  171 gr             */

#include <stdio.h>
#include <stdlib.h>

int scanString(char* str);

int main()
{
	char *str = (char*)malloc(sizeof(char));
	printf("Enter string\n");
	int strLength = scanString(str);

	char *substr = (char*)malloc(sizeof(char));
	printf("Enter substring\n");
	int substrLength = scanString(substr);

	int numbofSubstr = 0;
	int lengthOfHit = 0;
	for(int i = 0; i < strLength + 1; ++i)
	{
		
		if (str[i] == *(substr + lengthOfHit))
		{
			lengthOfHit++;
			if ((lengthOfHit == substrLength) && (lengthOfHit != 0))
			{

				lengthOfHit = 0;
				numbofSubstr++;
			}
		}
		else
		{
			lengthOfHit = 0;
		}
	}
	
	printf("%d", numbofSubstr);

	return 0;
}


int scanString(char* str)
{
	if (str == NULL)
	{
		printf("Lack of memory");
		exit(0);
	}
	int strLength = 0;
	while(true)
	{
		int usedMemory = 0;
		char c = '0';
		scanf("%c", &c);
		if (c == char(10))
		{
			break;
		}
		*(str + strLength) = c;
		strLength++;
		usedMemory++;
		if (usedMemory == 100)
		{
			str = (char*)realloc((void*)str, (strLength+100)*sizeof(char));
			if (str == NULL)
			{
				printf("Lack of memory");
				exit(0);
			}
		}
	}
	return strLength;
}