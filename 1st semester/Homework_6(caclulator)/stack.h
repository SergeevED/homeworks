#include "calculator_math.h"

struct intStack
{
	intLink longVal;
	intStack *next;
};

intLink stack_pop(intStack **stackHead);

void stack_push(intStack **stackHead, intLink *link);

void stack_clean(intStack **stackHead);

