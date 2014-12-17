/* homework ¹6
stack calculator
Addition, subtraction, multiplication, division of long numbers
Sergeev Evgeniy 171 gr*/

#include "stack.h"
#include <stdio.h>
#include <stdlib.h>

int main()
{
	printf("Calculator for long numbers\n'+', '-', '*', '/' are supported\nNumbers and operations should be separated by space\n"
			"Mathematical expression must end with a '='\nExample: a b + c d * - =	(a, b, c, d  - integer numbers)\nEnter 'Q' to exit\n\n");
	intStack *stackHead = NULL;
	while(1)
	{
		char operation = '0';
		int is_correct = 1;							//true = 1, false = 0
		char c = '0';

		intLink tempNum; 
		tempNum = intLink_scan(&is_correct, &operation);
		if (!is_correct)
		{
			intLink_deleteNumb(&tempNum);
			stack_clean(&stackHead);
			exit(0);
		}

		if (operation == '0')
		{
			stack_push(&stackHead, &tempNum);
			continue;
		}
		else if (operation == '=')
		{
			if (!stackHead)
			{
				printf("'=' scanned, though operation was expected\n");
				stack_clean(&stackHead);
				exit(0);
			}
			if (stackHead)
			{
				if (stackHead->next)
				{
					printf("'=' scanned, though operation was expected\n");
					stack_clean(&stackHead);
					exit(0);
				}
				linkList_reverse(&stackHead->longVal.head);
				if (stackHead->longVal.sign == -1 && stackHead->longVal.head->val != 0)
				{
					printf("-");
				}
				linkList_display(stackHead->longVal.head);
				stack_clean(&stackHead);
			}
			stackHead = NULL;
			continue;
		}
		else
		{
			intLink secondNum = stack_pop(&stackHead);
			intLink firstNum = stack_pop(&stackHead);
			if (!firstNum.head || !secondNum.head)
			{
				printf("Operation has been scanned though number was expected\n");
				intLink_deleteNumb(&firstNum);
				intLink_deleteNumb(&secondNum);
				stack_clean(&stackHead);
				exit(0);
			}

			if (operation == '-')
			{
				secondNum.sign *= -1;
			}
		
			if (operation == '/')
			{
				int is_zero = 1;
				link* tempLink = secondNum.head;
				while (tempLink)
				{
					if (tempLink->val)
					{
						is_zero = 0;
						break;
					}
					tempLink = tempLink->next;
				}
				if (is_zero)
				{
					printf("Division by zero is impossible\n");
					intLink_deleteNumb(&firstNum);
					intLink_deleteNumb(&secondNum);
					stack_clean(&stackHead);
					exit(0);
				}
			}

			intLink resultNum = intLink_calcResult(firstNum, secondNum, operation);
			linkList_reverse(&resultNum.head);
			stack_push(&stackHead, &resultNum);
			intLink_deleteNumb(&firstNum);
			intLink_deleteNumb(&secondNum);
		}
	}
	return 0;
}
