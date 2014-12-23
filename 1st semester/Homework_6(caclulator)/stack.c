#include "stack.h"
#include <stdio.h>
#include <stdlib.h>


void stack_push(struct intStack **stackHead, struct intLink *number)
{
	struct intStack *newLink = (struct intStack*)malloc(sizeof(struct intStack));
	if (!newLink)
	{
		printf("Lack of memory\n");
		return;
	}
	newLink->next = *stackHead;
	newLink->longVal = *number;
	*stackHead = newLink;
	return;
}

struct intLink stack_pop(struct intStack **stackHead)
{
	if (!*stackHead)
	{
		struct intLink tempVal;
		tempVal.head = NULL;
		tempVal.sign = 0;
		return tempVal;
	}
	struct intStack *temp = *stackHead;
	struct intLink firstVal = (*stackHead)->longVal;
	*stackHead = (*stackHead)->next;
	free(temp);
	return firstVal;
}

void stack_clean(struct intStack **stackHead)
{
	if (!*stackHead)
	{
		return;
	}
	struct intStack *currentLink = *stackHead;
	while (currentLink)
	{
		intLink_deleteNumb(&currentLink->longVal);
		struct intStack *temp = currentLink;
		currentLink = currentLink->next;
		free(temp);
	}
	return;
}



