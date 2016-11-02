#include "calculator_math.h"

struct intStack
{
	struct intLink longVal;
	struct intStack *next;
} stackHead;

struct intLink stack_pop(struct intStack **stackHead);

void stack_push(struct intStack **stackHead, struct intLink *firstLink);

void stack_clean(struct intStack **stackHead);

