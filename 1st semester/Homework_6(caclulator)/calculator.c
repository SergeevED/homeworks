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
		resultNum.sign = firstNum.sign;
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

	linkList_deleteLeadingZeroes(&(resultNum.head));

	return resultNum;
}



intLink intLink_sumNum(intLink firstNum, intLink secondNum)
{
	intLink resultNum;
	resultNum.head = NULL;
	link *firstLink = firstNum.head;
	link *secondLink = secondNum.head;
	int temp = 0;
	while(firstLink != NULL || secondLink != NULL)
	{
		if (firstLink != NULL) 
		{
			temp += firstLink->val;
			firstLink = firstLink->next;
		}
		if (secondLink != NULL) 
		{
			temp += secondLink->val;
			secondLink = secondLink->next;
		}
		linkList_addFront(&(resultNum.head), temp);
		temp = 0;
	}
	linkList_addFront(&(resultNum.head), 0);
	link* currentNum = resultNum.head;
	
	while(currentNum->next != NULL)
	{
		if (currentNum->next->val >= 10)
		{
			currentNum->next->val -= 10;
			currentNum->val++;
		}
		if (currentNum->val >= 10)
		{
			currentNum = resultNum.head;
			continue;
		}
		currentNum = currentNum->next;
	}
	return resultNum;
}



intLink intLink_subtractNum(intLink firstNum, intLink secondNum)
{
	intLink resultNum;
	resultNum.head = NULL;
	link *firstLink = firstNum.head;
	link *secondLink = secondNum.head;
	int temp = 0;
	while(firstLink != NULL || secondLink != NULL)
	{
		if (firstLink != NULL) 
		{
			temp += firstLink->val;
			firstLink = firstLink->next;
		}
		if (secondLink != NULL) 
		{
			temp -= secondLink->val;
			secondLink = secondLink->next;
		}
		linkList_addFront(&(resultNum.head), temp);
		temp = 0;
	}
	link *currentNum = resultNum.head;
	while (currentNum != NULL)
	{
		if (currentNum->val > 0)
		{
			resultNum.sign = 1;
			break;
		}
		else if (currentNum->val < 0)
		{
			resultNum.sign = -1;
			currentNum = resultNum.head;
			while (currentNum != NULL)
			{
				currentNum->val *= -1;
				currentNum = currentNum->next;
			}
			break;
		}
		else
		{
			currentNum = currentNum->next;
		}
	}
	currentNum = resultNum.head;
	while(currentNum->next != NULL)
	{
		if (currentNum->next->val < 0)
		{
			currentNum->val = currentNum->val - 1;
			currentNum->next->val += 10;
		}
		if (currentNum->val < 0)
		{
			currentNum = resultNum.head;
			continue;
		}
		
		currentNum = currentNum->next;
	}
	return resultNum;
}


void intLink_deleteNumbs(intLink *firstNum, intLink *secondNum, intLink *thirdNum)
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


intLink intLink_multiplicateNum(intLink firstNum, intLink secondNum)
{
	intLink resultNum;
	resultNum.head = NULL;
	link *firstLink = firstNum.head;
	link *secondLink = secondNum.head;
	int temp = 0;
	while (firstLink != NULL || secondLink != NULL)
	{
		if (firstLink != NULL)
		{
			firstLink = firstLink->next;
			linkList_addFront(&(resultNum.head), 0);
		}
		if (secondLink != NULL)
		{
			secondLink = secondLink->next;
			linkList_addFront(&(resultNum.head), 0);
		}
	}

	firstLink = firstNum.head;
	secondLink = secondNum.head;
	link *currentResLink = resultNum.head;
	link *previousResLink = resultNum.head;
	while (secondLink != NULL)
	{
		while (firstLink != NULL)
		{
			currentResLink->val += firstLink->val * secondLink->val;
			firstLink = firstLink->next;
			currentResLink = currentResLink->next;
		}
		previousResLink = previousResLink->next;
		currentResLink = previousResLink;
		firstLink = firstNum.head;
		secondLink = secondLink->next;
	}
	
	link *currentNum = resultNum.head;
	while(currentNum->next != NULL)
	{
		if (currentNum->val >= 10)
		{
			currentNum->next->val += currentNum->val / 10;
			currentNum->val %= 10;
		}
		currentNum = currentNum->next;
	}
	linkList_reverse(&(resultNum.head));
	return resultNum;
}


	
	
