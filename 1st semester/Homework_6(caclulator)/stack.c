#include "stack.h"
#include <stdio.h>
#include <stdlib.h>


void stack_push(intStack **stackHead, intLink *number)
{
	intStack *newLink = (intStack*)malloc(sizeof(intStack));
	if (!newLink)
	{
		printf("Lack of memory\n");
		return;
	}
	newLink->next = *stackHead;
	newLink->longVal = *number;
	*stackHead = newLink;
}

intLink stack_pop(intStack **stackHead)
{
	if (!*stackHead)
	{
		intLink tempVal;
		tempVal.head = NULL;
		tempVal.sign = 0;
		return tempVal;
	}
	intStack *temp = *stackHead;
	intLink firstVal = (*stackHead)->longVal;
	*stackHead = (*stackHead)->next;
	free(temp);
	return firstVal;
}

void stack_clean(intStack **stackHead)
{
	if (!*stackHead)
	{
		return;
	}
	intStack *currentLink = *stackHead;
	while (currentLink)
	{
		intLink_deleteNumb(&currentLink->longVal);
		intStack *temp = currentLink;
		currentLink = currentLink->next;
		free(temp);
	}
}



