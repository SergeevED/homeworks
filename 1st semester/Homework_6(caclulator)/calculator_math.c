#include "calculator_math.h"
#include <stdio.h>
#include <stdlib.h>


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
	if (resultNum.head)
	{
		if (!resultNum.head->val)
		{
			resultNum.sign = 1;
		}
	}
	return resultNum;
}


intLink intLink_sumNum(intLink firstNum, intLink secondNum)
{
	intLink resultNum;
	resultNum.head = NULL;
	link *firstLink = firstNum.head;
	link *secondLink = secondNum.head;
	int temp = 0;
	while(firstLink || secondLink)
	{
		if (firstLink) 
		{
			temp += firstLink->val;
			firstLink = firstLink->next;
		}
		if (secondLink) 
		{
			temp += secondLink->val;
			secondLink = secondLink->next;
		}
		linkList_addFront(&(resultNum.head), temp);
		temp = 0;
	}
	linkList_addFront(&(resultNum.head), 0);
	link* currentNum = resultNum.head;
	
	while(currentNum->next)
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
	while(firstLink || secondLink)
	{
		if (firstLink) 
		{
			temp += firstLink->val;
			firstLink = firstLink->next;
		}
		if (secondLink) 
		{
			temp -= secondLink->val;
			secondLink = secondLink->next;
		}
		linkList_addFront(&(resultNum.head), temp);
		temp = 0;
	}
	link *currentNum = resultNum.head;
	while (currentNum)
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
			while (currentNum)
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
	if (resultNum.sign != 1 && resultNum.sign != -1)			//make sign '+' in case of zero
	{
		resultNum.sign = 1;
	}
	currentNum = resultNum.head;
	while(currentNum->next)
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



intLink intLink_multiplicateNum(intLink firstNum, intLink secondNum)
{
	intLink resultNum;
	resultNum.head = NULL;
	link *firstLink = firstNum.head;
	link *secondLink = secondNum.head;
	int temp = 0;
	while (firstLink || secondLink)
	{
		if (firstLink)
		{
			firstLink = firstLink->next;
			linkList_addFront(&(resultNum.head), 0);
		}
		if (secondLink)
		{
			secondLink = secondLink->next;
			linkList_addFront(&(resultNum.head), 0);
		}
	}

	firstLink = firstNum.head;
	secondLink = secondNum.head;
	link *currentResLink = resultNum.head;
	link *previousResLink = resultNum.head;
	while (secondLink)
	{
		while (firstLink)
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
	while(currentNum->next)
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


intLink intLink_divideNum(intLink dividend, intLink divider) 
{
	int dividerSign = divider.sign;
	intLink resultNum;
	intLink partialDividend;
	resultNum.head = NULL;
	partialDividend.head = NULL;
	partialDividend.sign = 1;
	intLink reversedDividend;
	reversedDividend.head = linkList_getReversedList(dividend.head);
	
	int lengthOfDivider = linkList_length(divider.head);
	link *currentDividendDigit = reversedDividend.head;
	
	int lengthOfPartialDividend = linkList_length(partialDividend.head);
	for (int i = 0; i < lengthOfDivider - lengthOfPartialDividend; i++)
	{
		if (currentDividendDigit)
		{
			linkList_addFront(&(partialDividend.head), currentDividendDigit->val);
			currentDividendDigit = currentDividendDigit->next;
		}
	}
	divider.sign = -1;
	intLink tempNum;
	tempNum = intLink_calcResult(partialDividend, divider, '+');
	if ((tempNum.sign == -1) && (currentDividendDigit))
	{
		linkList_addFront(&(partialDividend.head), currentDividendDigit->val);
		currentDividendDigit = currentDividendDigit->next;
	}
	intLink_deleteNumb(&tempNum);

	while (true)
	{
		int j = 1;
		for (j = 1; j <= 10; j++)
		{
			divider.sign = -1;
			link* temp1 = partialDividend.head;
			partialDividend = intLink_calcResult(partialDividend, divider, '-');
			linkList_clean(&temp1);
			linkList_reverse(&(partialDividend.head));
			if (partialDividend.sign == -1)
			{
				divider.sign = 1;
				link* temp2 = partialDividend.head;
				partialDividend = intLink_calcResult(partialDividend, divider, '+');
				linkList_clean(&temp2);
				linkList_deleteLeadingZeroes(&(partialDividend.head));
				linkList_reverse(&(partialDividend.head));
				linkList_addBack(&(resultNum.head), j - 1);
				break;
			}
		}
		if (currentDividendDigit)
		{
			linkList_addFront(&(partialDividend.head), currentDividendDigit->val);
			currentDividendDigit = currentDividendDigit->next;
		}
		else
		{
			break;
		}
	} 
	divider.sign = dividerSign;
	

	if (dividend.sign == -1)		//incrementation resultNum in case of negative remainder
		{
			int remainderIsZero = 1;
			link* tempLink1 = partialDividend.head;
			while (tempLink1)
			{
				if (tempLink1->val)
				{
					remainderIsZero = 0;
					break;
				}
				tempLink1 = tempLink1->next;
			}
			if (!remainderIsZero)
			{
				link* tempLink2 = resultNum.head;
				while (tempLink2->next)
				{
					tempLink2 = tempLink2->next;
				}
				(tempLink2->val)++;
			}
		}

	intLink_deleteNumb(&partialDividend);
	intLink_deleteNumb(&reversedDividend);
	return resultNum;
}