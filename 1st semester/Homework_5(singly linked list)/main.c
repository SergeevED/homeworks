/* homework №5
singly linked list 
Sergeev Evgeniy 171 gr*/

#include "header.h"

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
				linkList_addFront(&firstLink, numb);
				break;
			case 'p':
				linkList_display(&firstLink);
				break;
			case 'r':
				scanf("%d", &numb);
				linkList_delete(&firstLink, numb);
				break;
			case 'q':
				linkList_clean(&firstLink);
				exit(0);
				break;
		}
	}
return 0;
}