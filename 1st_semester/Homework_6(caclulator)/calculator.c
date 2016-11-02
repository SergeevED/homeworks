#include "calculator.h"
#include <stdio.h>
#include <stdlib.h>

struct intLink intLink_scan(int *is_correct, int *endOfFileScanned, char *operation)
{
	struct intLink number;
	number.head = NULL;
	number.sign = 0;
	char c;
	char c1;
	c = ' ';

	while (c == ' ' || c == '\n')			
	{
		c = (signed char)getchar();
		switch (c)
		{
			case ' ':
			case '\n':
				break;
			case '+':
			case '*':
			case '/':
			case '=':
				*operation = c;
				return number;
				break;
			case EOF:
				*endOfFileScanned = 1;
				return number;
				break;
			case '-':
				c1 = (signed char)getchar();
				if (c1 == EOF)
				{
					*endOfFileScanned = 1;
					*operation = c;
					return number;
				}
				else if (c1 == ' ' || c1 == '\n')
				{
					*operation = c;
					return number;
				}
				else if (c1 >= '0' && c1 <= '9')
				{
					number.sign = -1;
					linkList_addFront(&(number.head), c1 - '0');
				}
				else
				{
					printf("Unknown command\n");
					*is_correct = 0;
					return number;
				}
				break;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				number.sign = 1;
				linkList_addFront(&(number.head), c - '0');
				break;
			default:
				printf("Unknown command\n");
				*is_correct = 0;
				return number;
				break;
		}
	}

	c = '0';
	while ((c >= '0') && (c <= '9'))
	{
		c = (signed char)getchar();
		switch (c)
		{
			case EOF:
				*endOfFileScanned = 1;
				return number;
				break;
			case ' ':
			case '\n':
				return number;
				break;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				linkList_addFront(&(number.head), c - '0');
				break;
			default:
				printf("Unexpected sumbol '%c'\n", c);
				*is_correct = 0;
				return number;
				break;
		}
	}
	return number;
}


void intLink_deleteNumb(struct intLink *firstNum)		//freeing memory
{
	if (firstNum)
	{
		firstNum->sign = 0;
		if (firstNum->head)
		{
			linkList_clean(&(firstNum->head));
			firstNum->head = 0;
		}
	}
	return;
}
