#include "header.h"


void linkList_addBack(link** firstLink, int data)
{
	link *newLink = (link*)malloc(sizeof(link));
	if (newLink == NULL)
	{
		printf("Lack of memory\n");
		link *tempLink = *firstLink;
		linkList_clean(&tempLink);
		exit(0);
	}
	if (*firstLink == NULL)
	{
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
	}
	link *currentLink = *firstLink;
	while (currentLink->next != NULL)
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
	if (newLink == NULL)
	{
		printf("Lack of memory\n");
		link *tempLink = *firstLink;
		linkList_clean(&tempLink);
		exit(0);
	}
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
}

void linkList_display(link* firstLink)
{
	if (firstLink == NULL)
	{
		printf("EMPTY\n");
		return;
	}
	link *currentLink = firstLink;
	while (currentLink)
	{
		printf("%d", currentLink->val);
		if (currentLink->next != NULL)
		{
			printf(", ");
		}
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
	if (*firstLink == NULL)
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
	while (currentLink != 0)
	{
		linkList_addFront(&newLink, currentLink->val);
		currentLink = currentLink->next;
	}
	linkList_clean(&*firstLink);
	*firstLink = newLink;
}

void linkList_deleteLeadingZeroes(link **firstLink)
{
	link* currentLink = *firstLink;
	while (currentLink->next != NULL)
	{
		if (currentLink->val == 0)
		{
			linkList_delete(&*firstLink, 0);
		}
		else
		{
			break;
		}
	}
}