#include "header.h"


void addLinkFront(int data, link** firstLink)
{
	link *newLink = (link*)malloc(sizeof(link));
	if (newLink == 0)
	{
		printf("Lack of memory\n");
		exit(0);
	}
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
}

void displayLink(link** firstLink)
{
	if (*firstLink == NULL)
	{
		printf("EMPTY\n");
		return;
	}
	link *currentLink = *firstLink;
	while (currentLink)
	{
		printf("%d", currentLink->val);
		currentLink = currentLink->next;
	}
	printf("\n");
}

void deleteLink(int data, link** firstLink)
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

void cleanList(link** firstLink)
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
