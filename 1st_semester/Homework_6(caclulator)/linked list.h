struct link
{
	int val;
	struct link *next;
} firstLink;

void linkList_addBack(struct link** firstLink, int data);

void linkList_addFront(struct link** firstLink, int data);

void linkList_display(struct link* firstLink);

void linkList_displayWithoutNewline(struct link* firstLink);

void linkList_delete(struct link** firstLink, int data);

void linkList_clean(struct link** firstLink);

void linkList_reverse(struct link **firstLink);

struct link* linkList_getReversedList(struct link* firstLink);

void linkList_deleteLeadingZeroes(struct link **firstLink);

int linkList_length(struct link *firstLink);