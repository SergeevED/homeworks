/* homework ¹6
Addition and subtraction of long numbers
Sergeev Evgeniy 171 gr*/


#include "header.h"

int main()
{
	printf("Calculator for long numbers\n'+', '-' are supported\nNumbers and operations should be separated by space\nExample: a + b	(a, b  - integer numbers)\nEnter 'Q' to exit\n\n");
	while(true)
	{
		int operation = 0;
		int firstSign = 0, secondSign = 0;
		bool is_correct = true;
		link *firstNum = NULL;

		char c = '\n';
		while(c != '\n')
		{
			if (c == 'Q')
		{
			exit(0);
		}
			scanf("%c", &c);
		}

		firstNum = scanNum(&firstSign, &is_correct);
		if (is_correct == false)
		{
			cleanList(&firstNum);
			continue;
		}
		
		
		while (c == ' ' || c == '\n')
		{
			scanf("%c", &c);
			if (c == 'Q')
			{
				cleanList(&firstNum);
				exit(0);
			}
			if (!(c == ' ' || c == '\n' || c == '+' || c == '-'))
			{
				printf("Unexpected sumbol '%c'\n", c);
				is_correct = false;
				break;
			}
			if (c == '+')
			{
				operation = 1;
			}
			else if (c == '-')
			{
				operation = -1;
			}
		}
		if (is_correct == false)
		{
			cleanList(&firstNum);
			continue;
		}
		link *secondNum = NULL;
		secondNum = scanNum(&secondSign, &is_correct);
		if (is_correct == false)
		{
			cleanList(&firstNum);
			cleanList(&secondNum);
			continue;
		}
		if (firstNum == NULL || secondNum == NULL)
		{
			printf("Lack of memory");
			cleanList(&firstNum);
			cleanList(&secondNum);
			exit(0);
		}

		secondSign *= operation;

		link *resultNum = NULL;
		int resultSign = 0;
		resultNum = calcResult(firstNum, secondNum, firstSign, secondSign, &resultSign);
			
		if (resultSign == -1)
		{
			resultNum->val *= -1;
		}

		displayLink(&resultNum);
		cleanList(&resultNum);
		cleanList(&firstNum);
		cleanList(&secondNum);
	}
	

	return 0;
}

