#include "linked list.h"

struct intLink
{
	short sign;
	struct link *head;
} firstNum;


struct intLink intLink_scan(int *is_correct, int *endOfFileScanned, char *operation);

void intLink_deleteNumb(struct intLink *firstNum);