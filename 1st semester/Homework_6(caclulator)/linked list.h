struct link
{
	short val;
	link *next;
};

void linkList_addBack(link** firstLink, int data);

void linkList_addFront(link** firstLink, int data);

void linkList_display(link* firstLink);

void linkList_delete(link** firstLink, int data);

void linkList_clean(link** firstLink);

void linkList_reverse(link **firstLink);

link* linkList_getReversedList(link* firstLink);

void linkList_deleteLeadingZeroes(link **firstLink);

int linkList_length(link *firstLink);