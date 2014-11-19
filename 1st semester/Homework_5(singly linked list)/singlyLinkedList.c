struct link
{
	int val;
	link *next;
};

void addLink(int data, link** firstLink)
{
	link *newLink = (link *)malloc(sizeof(link));
	if (newLink)
	{
		newLink->next = *firstLink;
		newLink->val = data;
		*firstLink = newLink;
	}
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
		if (currentLink->next != NULL)
		{
			printf(", ");
		}
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
	link *currentLink = (*firstLink)->next;
	while (currentLink)
	{
		link *temp = currentLink;
		currentLink = currentLink->next;
		free(temp);
	}
	free(*firstLink);
}