/*	Counting number of concurrences of substring of a string
	Sergeev Evgeniy  171 gr             */

#include <stdio.h>
#include <stdlib.h>

int main()
{
	char *str = (char*)malloc(sizeof(char));
	printf("Enter string\n");
	int strLength = 0;
	while(true)
	{
		char c = '0';
		scanf("%c", &c);
		if (c == char(10))
		{
			break;
		}
		*(str + strLength) = c;
		strLength++;
		str = (char*)realloc((void*)str, (strLength+1)*sizeof(char));
	}

	char *substr = (char*)malloc(sizeof(char));
	printf("Enter substring\n");
	int substrLength = 0;
	while(true)
	{
		char c = '0';
		scanf("%c", &c);
		if (c == char(10))
		{
			break;
		}
		*(substr + substrLength) = c;
		substrLength++;
		substr = (char*)realloc((void *)substr, (substrLength+1)*sizeof(char));
	}

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