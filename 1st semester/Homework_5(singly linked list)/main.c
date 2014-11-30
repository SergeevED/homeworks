/* homework ¹5
singly linked list 
Sergeev Evgeniy 171 gr*/

#include "LinkedList.h"

int main()
{
	link *firstLink = NULL;
	printf("Enter 'a' <number> to add <number>\nEnter 'p' to display all values\n"
	"Enter 'r' <number> to delete first entry of <number>\nEnter 'q' to exit\n");
	while (true)
	{
		char c = '\n';
		int numb = 0;
		scanf("%c", &c);
		switch (c)
		{
			case 'a':
				scanf("%d", &numb);
				addLink(numb, &firstLink);
				break;
			case 'p':
				displayLink(&firstLink);
				break;
			case 'r':
				scanf("%d", &numb);
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