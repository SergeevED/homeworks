#include "linked list.h"

struct intLink
{
	short sign;
	link *head;
};


intLink intLink_scan(int *is_correct, char *operation);

void intLink_deleteNumb(intLink *firstNum);