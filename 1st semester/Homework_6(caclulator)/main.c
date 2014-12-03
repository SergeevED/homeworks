/* homework ¹6
Addition and subtraction of long numbers
Sergeev Evgeniy 171 gr*/


#include "header.h"

int main()
{
	printf("Calculator for long numbers\n'+', '-', '*' are supported\nNumbers and operations should be separated by space\nExample: a + b	(a, b  - integer numbers)\nEnter 'Q' to exit\n\n");
	
	
	while(true)
	{

		char operation = '0';
		bool is_correct;
		is_correct = true;
		char c = '\n';

		intLink firstNum;
		firstNum = intLink_scanNum(&is_correct, &operation);
		if (is_correct == false)
		{
			intLink_deleteNumbs(&firstNum, NULL, NULL);
			exit(0);
		}
		
		if (operation == '0')
		{
			while (c == ' ' || c == '\n')			//scanning operation
			{
				scanf("%c", &c);
				switch (c)
				{
					case 'Q':
						printf("You quit the program");
						intLink_deleteNumbs(&firstNum, NULL, NULL);
						exit(0);
						break;
					case '*':
						operation = '*';
						break;
					case '+':
						operation = '+';
						break;
					case '-':
						operation = '-';
						break;
					case ' ':
					case '\n':
						break;
					default:
						printf("Unexpected sumbol '%c'\n", c);
						intLink_deleteNumbs(&firstNum, NULL, NULL);
						exit(0);
						break;
				}
			}
		}
		
		intLink secondNum;
		secondNum = intLink_scanNum(&is_correct, &operation);
		if (is_correct == false)
		{
			intLink_deleteNumbs(&firstNum, &secondNum, NULL);
			exit(0);
		}
		if (firstNum.head == NULL || secondNum.head == NULL)
		{
			printf("Lack of memory");
			intLink_deleteNumbs(&firstNum, &secondNum, NULL);
			exit(0);
		}

		if (operation == '-')
		{
			secondNum.sign *= -1;
		}
		
		intLink resultNum;
		resultNum = intLink_calcResult(firstNum, secondNum, operation);
			
		if (resultNum.sign == -1 && (*resultNum.head).val != 0)
		{
			printf("-");
		}
		

		linkList_display(&(resultNum.head));
		intLink_deleteNumbs(&firstNum, &secondNum, &resultNum);
	}
	

	return 0;
}
