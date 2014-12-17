#include "linked list.h"
#include <stdio.h>
#include <stdlib.h>


void linkList_addBack(link** firstLink, int data)
{
	link *newLink = (link*)malloc(sizeof(link));
	if (!newLink)
	{
		printf("Lack of memory\n");
		return;
	}
	if (!*firstLink)
	{
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
	}
	link *currentLink = *firstLink;
	while (currentLink->next)
	{
		currentLink = currentLink->next;
	}
	link* temp = currentLink->next;
	currentLink->next = newLink;
	newLink->val = data;
	newLink->next = temp;
}


void linkList_addFront(link** firstLink, int data)
{
	link *newLink = (link*)malloc(sizeof(link));
	if (!newLink)
	{
		printf("Lack of memory\n");
		return;
	}
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
}

void linkList_display(link* firstLink)
{
	if (!firstLink)
	{
		printf("EMPTY\n");
		return;
	}
	link *currentLink = firstLink;
	while (currentLink)
	{
		printf("%d", currentLink->val);
		currentLink = currentLink->next;
	}
	printf("\n");
}

void linkList_delete(link** firstLink, int data)
{
	if ((*firstLink) == NULL || ((*firstLink)->val == data && (*firstLink)->next == 0) )
	{
		*firstLink = NULL;
		return;
	}

	link *currentLink = (*firstLink)->next;
	link *previousLink = *firstLink;

	if ((*firstLink)->val == data)
	{
		(*firstLink)->val = currentLink->val;
		(*firstLink)->next = currentLink->next;
		link *temp = currentLink;
		free(temp);
		return;
	}

	while (currentLink)
	{
		if (currentLink->val == data)
		{
			previousLink->next = currentLink->next;
			link *temp = currentLink;
			free(temp);
			return;
		}
		previousLink = previousLink->next;
		currentLink = currentLink->next;
	}
}

void linkList_clean(link** firstLink)
{
	if (!*firstLink)
	{
		return;
	}
	link *currentLink = *firstLink;
	while (currentLink)
	{
		link *temp = currentLink;
		currentLink = currentLink->next;
		free(temp);
	}
}

void linkList_reverse(link **firstLink)
{
	link *newLink = NULL;
	link *currentLink = *firstLink;
	while (currentLink)
	{
		linkList_addFront(&newLink, currentLink->val);
		currentLink = currentLink->next;
	}
	linkList_clean(&*firstLink);
	*firstLink = newLink;
}

link* linkList_getReversedList(link *firstLink)
{
	link *newLink = NULL;
	link *currentLink = firstLink;
	while (currentLink)
	{
		linkList_addFront(&newLink, currentLink->val);
		currentLink = currentLink->next;
	}
	return newLink;
}

void linkList_deleteLeadingZeroes(link **firstLink)
{
	if (!*firstLink)
	{
		return;
	}
	link* currentLink = *firstLink;
	while (currentLink->next)
	{
		if (!currentLink->val)
		{
			linkList_delete(&*firstLink, 0);
		}
		else
		{
			break;
		}
	}
}

int linkList_length(link *firstLink)
{
	int lengthOfList = 0;
	if (!firstLink)
	{
		return 0;
	}
	while (firstLink)
	{
		firstLink = firstLink->next;
		lengthOfList++;
	}
	return lengthOfList;
}