/* homework ¹6
Addition, subtraction, multiplication, division of long numbers
Sergeev Evgeniy 171 gr*/

#include "calculator_math.h"
#include <stdio.h>
#include <stdlib.h>

int main()
{
	printf("Calculator for long numbers\n'+', '-', '*', '/' are supported\nNumbers and operations should be separated by space\nExample: a + b	(a, b  - integer numbers)\nEnter 'Q' to exit\n\n");
	
	while(1)
	{
		char operation = '0';
		int is_correct = 1;							//true = 1, false = 0
		char c = '\n';

		intLink firstNum;
		firstNum = intLink_scan(&is_correct, &operation);
		if (!is_correct)
		{
			intLink_deleteNumb(&firstNum);
			exit(0);
		}
		if (operation != '0')							//expected that operation will not be scanned
		{
			printf("Unexpected sumbol '%c'\n", operation);
			intLink_deleteNumb(&firstNum);
			exit(0);
		}

		intLink notScanned;
		notScanned = intLink_scan(&is_correct, &operation);	
		if (!is_correct)
		{
			intLink_deleteNumb(&firstNum);
			intLink_deleteNumb(&notScanned);
			exit(0);
		}
		if (operation == '0')							//expected that operation will be scanned
		{
			printf("Operation was expected");
			intLink_deleteNumb(&firstNum);
			intLink_deleteNumb(&notScanned);
			exit(0);
		}
		if (operation == '=')
		{
			printf("Unexpected sumbol '%c'\n", operation);
			intLink_deleteNumb(&firstNum);
			exit(0);
		}

		char notScannedOperation = '0';			//expected that operation will not be scanned
		intLink secondNum;
		secondNum = intLink_scan(&is_correct, &notScannedOperation);
		if (!is_correct)
		{
			intLink_deleteNumb(&firstNum);
			intLink_deleteNumb(&secondNum);
			exit(0);
		}
		if (notScannedOperation != '0')
		{
			printf("Unexpected sumbol '%c'\n", notScannedOperation);
			intLink_deleteNumb(&firstNum);
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
				exit(0);
			}
		}
		
		intLink resultNum;
		resultNum = intLink_calcResult(firstNum, secondNum, operation);
			
		if (resultNum.sign == -1 && resultNum.head->val != 0)
		{
			printf("-");
		}
		
		linkList_display(resultNum.head);
		intLink_deleteNumb(&firstNum);
		intLink_deleteNumb(&secondNum);
		intLink_deleteNumb(&resultNum);
	}
	
	return 0;
}