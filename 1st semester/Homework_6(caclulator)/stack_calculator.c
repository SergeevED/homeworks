/* homework ¹6
stack calculator
Addition, subtraction, multiplication, division of long numbers
Sergeev Evgeniy 171 gr*/

#include "stack.h"
#include <stdio.h>
#include <stdlib.h>

int main()
{
	printf("Calculator for long numbers\n'+', '-', '*', '/', '=' are supported\nNumbers and operations should be separated by 'Enter' or 'Space'\n"
			"Example: a b + c d * - =	(a, b, c, d  - integer numbers)\n\n");
	struct intStack *stackHead = NULL;
	while(1)
	{
		char operation = '0';
		int endOfFileScanned = 0;
		int is_correct = 1;							//true = 1, false = 0
		char c = '0';

		struct intLink tempNum; 
		tempNum = intLink_scan(&is_correct, &endOfFileScanned, &operation);
		if (!is_correct)
		{
			intLink_deleteNumb(&tempNum);
			stack_clean(&stackHead);
			exit(1);
		}
		if (tempNum.head)
		{
			stack_push(&stackHead, &tempNum);
		}
		else if (operation == '=')
		{
			if (!stackHead)
			{
				printf("Not enough arguments\n");
				stack_clean(&stackHead);
				exit(1);
			}
			else
			{
				struct link* printedLink = linkList_getReversedList(stackHead->longVal.head);
				if (stackHead->longVal.sign == -1 && stackHead->longVal.head->val)
				{
					printf("-");
				}
				linkList_display(printedLink);
				linkList_clean(&printedLink);
			}
		}
		else if (operation == '+' || operation == '-' || operation == '*' || operation == '/')
		{
			struct intLink firstNum = stack_pop(&stackHead);
			struct intLink secondNum = stack_pop(&stackHead);
			if (!firstNum.head || !secondNum.head)
			{
				printf("Not enough arguments\n");
				intLink_deleteNumb(&firstNum);
				intLink_deleteNumb(&secondNum);
				stack_clean(&stackHead);
				exit(1);
			}
			if (operation == '-')
			{
				secondNum.sign *= -1;
			}
			if (operation == '/')
			{
				int is_zero = 1;
				struct link* tempLink = secondNum.head;
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
					printf("Division by zero\n");
					intLink_deleteNumb(&firstNum);
					intLink_deleteNumb(&secondNum);
					stack_clean(&stackHead);
					exit(1);
				}
			}
			struct intLink resultNum = intLink_calcResult(firstNum, secondNum, operation);
			linkList_reverse(&resultNum.head);
			stack_push(&stackHead, &resultNum);
			intLink_deleteNumb(&firstNum);
			intLink_deleteNumb(&secondNum);
		}
		if (endOfFileScanned)
		{
			if (!stackHead)
			{
				exit(0);
			}
			else
			{
				struct intStack *currentNumb = stackHead;
				printf("[");
				struct link* printedLink = linkList_getReversedList(currentNumb->longVal.head);
				if (currentNumb->longVal.sign == -1 && currentNumb->longVal.head->val)
				{
					printf("-");
				}
				linkList_displayWithoutNewline(printedLink);
				linkList_clean(&printedLink);
				currentNumb = currentNumb->next;
				while (currentNumb)
				{
					printf("; ");
					struct link* printedLink = linkList_getReversedList(currentNumb->longVal.head);
					if (currentNumb->longVal.sign == -1 && currentNumb->longVal.head->val)
					{
						printf("-");
					}
					linkList_displayWithoutNewline(printedLink);
					linkList_clean(&printedLink);
					currentNumb = currentNumb->next;
				}
				printf("]\n");
				stack_clean(&stackHead);
				exit(0);
			}
		}
	}
	return 0;
}
