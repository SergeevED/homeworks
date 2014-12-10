#include "header.h"


intLink intLink_scanNum(bool *is_correct, char *operation)
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
				printf("You quit the programm");
				is_correct = false;
				return number;
				break;
			case '+':
				number.sign = 1;
				break;
			case '-':
				number.sign = -1;
				break;
			case ' ':
			case '\n':
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
				break;
			default:
				printf("Unexpected sumbol '%c'\n", c);
				*is_correct = false;
				return number;
				break;
		}
	}

	if (c >= '0' && c <= '9')
	{
		linkList_addFront(&(number.head), c - '0');
	}
	c = '0';

	while ((c >= '0') && (c <= '9'))
	{
		scanf("%c", &c);
		switch (c)
		{
			case 'Q':
				printf("You quit the programm");
				is_correct = false;
				return number;
				break;
			case '*':
				*operation = '*';
				return number;
				break;
			case '/':
				*operation = '/';
				return number;
				break;
			case '+':
				*operation = '+';
				return number;
				break;
			case '-':
				*operation = '-';
				return number;
				break;
			case ' ':
			case '\n':
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
				*is_correct = false;
				return number;
				break;
		}
	}
	return number;
}




intLink intLink_calcResult(intLink firstNum, intLink secondNum, char operation)
{
	intLink resultNum;
	resultNum.head = NULL;
	if ((firstNum.sign * secondNum.sign == 1) && (operation == '+' || operation == '-'))
	{
		resultNum = intLink_sumNum(firstNum, secondNum);
		resultNum.sign = secondNum.sign;
	}
	else if ((firstNum.sign * secondNum.sign == -1) && (operation == '+' || operation == '-'))
	{
		resultNum = intLink_subtractNum(firstNum, secondNum);
		resultNum.sign *= firstNum.sign;
	}
	else if (operation == '*')
	{
		resultNum = intLink_multiplicateNum(firstNum, secondNum);
		resultNum.sign = firstNum.sign * secondNum.sign;
	}
	else if (operation == '/')
	{
		resultNum = intLink_divideNum(firstNum, secondNum);
		resultNum.sign = firstNum.sign * secondNum.sign;
	}
	linkList_deleteLeadingZeroes(&(resultNum.head));
	if (resultNum.head != NULL)
	{
		if ((*resultNum.head).val == 0)
		{
			resultNum.sign = 1;
		}
	}
	return resultNum;
}


void intLink_deleteNumbs(intLink *firstNum, intLink *secondNum, intLink *thirdNum)		//freeing memory
{
	if (firstNum != NULL)
	{
		firstNum->sign = 0;
		if (firstNum->head != NULL)
		{
			linkList_clean(&(firstNum->head));
			firstNum->head = 0;
		}
	}
	if (secondNum!= NULL)
	{
		secondNum->sign = 0;
		if (secondNum->head != NULL)
		{
			linkList_clean(&(secondNum->head));
			secondNum->head = 0;
		}
	}

	if (thirdNum != NULL)
	{
		thirdNum->sign = 0;
		if (thirdNum->head != NULL)
		{
			linkList_clean(&(thirdNum->head));
			thirdNum->head = 0;
		}
	}
}


