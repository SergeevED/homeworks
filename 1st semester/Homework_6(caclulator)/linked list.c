#include "linked list.h"
#include <stdio.h>
#include <stdlib.h>


void linkList_addBack(struct link** firstLink, int data)
{
	struct link *newLink = (struct link*)malloc(sizeof(struct link));
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
	struct link *currentLink = *firstLink;
	while (currentLink->next)
	{
		currentLink = currentLink->next;
	}
	struct link* temp = currentLink->next;
	currentLink->next = newLink;
	newLink->val = data;
	newLink->next = temp;
	return;
}


void linkList_addFront(struct link** firstLink, int data)
{
	struct link *newLink = (struct link*)malloc(sizeof(struct link));
	if (!newLink)
	{
		printf("Lack of memory\n");
		return;
	}
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
		return;
}

void linkList_display(struct link* firstLink)
{
	if (!firstLink)
	{
		printf("EMPTY\n");
		return;
	}
	struct	link *currentLink = firstLink;
	while (currentLink)
	{
		printf("%d", currentLink->val);
		currentLink = currentLink->next;
	}
	printf("\n");
	return;
}

void linkList_displayWithoutNewline(struct link* firstLink)
{
	if (!firstLink)
	{
		printf("EMPTY\n");
		return;
	}
	struct	link *currentLink = firstLink;
	while (currentLink)
	{
		printf("%d", currentLink->val);
		currentLink = currentLink->next;
	}
	return;
}

void linkList_delete(struct link** firstLink, int data)
{
	if ((*firstLink) == NULL || ((*firstLink)->val == data && (*firstLink)->next == 0) )
	{
		*firstLink = NULL;
		return;
	}

	struct link *currentLink = (*firstLink)->next;
	struct link *previousLink = *firstLink;

	if ((*firstLink)->val == data)
	{
		(*firstLink)->val = currentLink->val;
		(*firstLink)->next = currentLink->next;
		struct link *temp = currentLink;
		free(temp);
		return;
	}

	while (currentLink)
	{
		if (currentLink->val == data)
		{
			previousLink->next = currentLink->next;
			struct link *temp = currentLink;
			free(temp);
			return;
		}
		previousLink = previousLink->next;
		currentLink = currentLink->next;
	}
	return;
}

void linkList_clean(struct link** firstLink)
{
	if (!*firstLink)
	{
		return;
	}
	struct link *currentLink = *firstLink;
	while (currentLink)
	{
		struct link *temp = currentLink;
		currentLink = currentLink->next;
		free(temp);
	}
	return;
}

void linkList_reverse(struct link **firstLink)
{
	struct link *newLink = NULL;
	struct link *currentLink = *firstLink;
	while (currentLink)
	{
		linkList_addFront(&newLink, currentLink->val);
		currentLink = currentLink->next;
	}
	linkList_clean(&*firstLink);
	*firstLink = newLink;
	return;
}

struct link* linkList_getReversedList(struct link *firstLink)
{
	struct link *newLink = NULL;
	struct link *currentLink = firstLink;
	while (currentLink)
	{
		linkList_addFront(&newLink, currentLink->val);
		currentLink = currentLink->next;
	}
	return newLink;
}

void linkList_deleteLeadingZeroes(struct link **firstLink)
{
	if (!*firstLink)
	{
		return;
	}
	struct link* currentLink = *firstLink;
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
	return;
}

int linkList_length(struct link *firstLink)
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