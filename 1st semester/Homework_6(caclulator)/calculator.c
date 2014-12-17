#include "calculator.h"
#include <stdio.h>
#include <stdlib.h>

intLink intLink_scan(int *is_correct, char *operation)
{
	intLink number;
	number.head = NULL;
	char c = '\n';

	while (c == ' ' || c == '\n')			
	{
		scanf("%c", &c);
		switch (c)
		{
			case 'Q':
				printf("You quit the program\n");
				*is_correct = 0;
				return number;
				break;
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
			case '-':
				char c1;
				scanf("%c", &c1);
				if (c1 == ' ')
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
					printf("Unexpected sumbol '%c'\n", c);
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
				printf("Unexpected sumbol '%c'\n", c);
				*is_correct = 0;
				return number;
				break;
		}
	}

	c = '0';
	while ((c >= '0') && (c <= '9'))
	{
		scanf("%c", &c);
		switch (c)
		{
			case 'Q':
				printf("You quit the program");
				is_correct = 0;
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


void intLink_deleteNumb(intLink *firstNum)		//freeing memory
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
}