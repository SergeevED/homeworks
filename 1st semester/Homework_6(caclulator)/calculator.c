#include "header.h"


link* scanNum(int* sign, bool* is_correct)
{
	link *firstLink = NULL;
	char c = '\n';
	while( c == '\n' || c == ' ')
	{
		scanf("%c", &c);
		if (c == 'Q')
		{
			exit(0);
		}
		if (!(c == '+' || c == '-' || c == ' ' || c == '\n' || ((c >= '0') && (c <= '9'))))
		{
			printf("Unexpected sumbol '%c'\n", c);
			*is_correct = false;
			return NULL;
		}
		else if (c == '-')
		{
			*sign = -1;
			c = '0';
			break;
		}
		else if (c == '+') 
		{
			*sign = 1;
			c = '0';
			break;
		}
		else if ((c >= '0') && (c <='9'))
		{
			*sign = 1;
		}
	}
		if (c != 0)
		{
			addLinkFront(c - '0', &firstLink);
		}
		while ((c >= '0') && (c <= '9'))
		{
			scanf("%c", &c);
			if (!(c == ' ' || c == '\n' || ((c >= '0') && (c <= '9'))))
			{
				printf("Unexpected sumbol '%c'\n", c);
				*is_correct = false;
				return NULL;
			}
			if (c >= '0' && c <= '9')
			{
				addLinkFront(c - '0', &firstLink);
			}
		}
		return firstLink;
}



link* calcResult(link *firstLink, link *secondLink, int firstSign, int secondSign, int *resultSign)
{
	link *resultNum = NULL;
	if (firstSign * secondSign == 1)
	{
		*resultSign = firstSign;
		resultNum = sumNum(firstLink, secondLink);
	}
	else
	{
		resultNum = subtractNum(firstLink, secondLink, resultSign);
		*resultSign *= firstSign;
	}
	
	//deleting leading zeros
	link* currentLink = resultNum;
	while (currentLink->next != NULL)
	{
		if (currentLink->val == 0)
		{
			deleteLink(0, &resultNum);
		}
		else
		{
			break;
		}
	}
	return resultNum;
}



link* sumNum(link *firstLink, link *secondLink)
{
	link *resultNum = (link*)malloc(sizeof(link));
	resultNum = NULL;
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
		addLinkFront(temp, &resultNum);
		temp = 0;
	}
	addLinkFront(0, &resultNum);
	link* currentNum = resultNum;
	
	while(currentNum->next != NULL)
	{
		if (currentNum->next->val >= 10)
		{
			currentNum->next->val -= 10;
			currentNum->val++;
		}
		if (currentNum->val >= 10)
		{
			currentNum = resultNum;
			continue;
		}
		currentNum = currentNum->next;
	}
	return resultNum;
}



link* subtractNum(link *firstLink, link *secondLink, int *resultSign)
{
	link *resultNum = (link*)malloc(sizeof(link));
	resultNum = NULL;
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
		addLinkFront(temp, &resultNum);
		temp = 0;
	}
	link *currentNum = resultNum;
	while (currentNum != NULL)
	{
		if (currentNum->val > 0)
		{
			*resultSign = 1;
			break;
		}
		else if (currentNum->val < 0)
		{
			*resultSign = -1;
			currentNum = resultNum;
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
	currentNum = resultNum;
	while(currentNum->next != NULL)
	{
		if (currentNum->next->val < 0)
		{
			currentNum->val = currentNum->val - 1;
			currentNum->next->val += 10;
		}
		if (currentNum->val < 0)
		{
			currentNum = resultNum;
			continue;
		}
		
		currentNum = currentNum->next;
	}
	return resultNum;
}