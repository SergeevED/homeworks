/*	homework ¹5
	singly linked list 
	Sergeev Evgeniy 171 gr*/

#include <stdio.h>
#include <stdlib.h>
#include "singlyLinkedList.h"

int main()
{
	link *firstLink = NULL;
	printf("Enter 'a' <number> to add <number>\nEnter 'p' to display all values\n"
		   "Enter 'r' <number> to delete first entry of <number>\nEnter 'q' to exit\n");
	while (true)
	{
		char c = '\n';
		int numb = 0;
		while (c!='a' && c!='p' && c!='r' && c!='q')
		{
			scanf("%c", &c);
		}
		if (c == 'a' || c == 'r')
		{
			scanf("%d", &numb);
		}
		switch (c)
		{
		case 'a':
			addLink(numb, &firstLink);
			break;
		case 'p':
			displayLink(&firstLink);
			break;
		case 'r':
			deleteLink(numb, &firstLink);
			break;
		case 'q':
			cleanList(&firstLink);
			exit(0);
			break;
		}
	}
return 0;
}