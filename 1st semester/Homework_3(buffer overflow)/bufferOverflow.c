/*	Overrun the buffer's boundary and overwrite adjacent memory 
	while writing data to a buffer to call procedure
	Sergeev Evgeniy  171 gr             */

#pragma check_stack(off)

#include <stdio.h>
#include <string.h>

void overflowedProcedure();
void notExecutableProcedure();

int main()
{
	overflowedProcedure();
	printf("\nReturn to main\n");
	return 0;
}


void overflowedProcedure()
{
	printf("First procedure is executing\n");
	printf("main  %p\n", &(main) );
	printf("notExecutableProcedure %p\n", &(notExecutableProcedure) );
	char data[5];
	scanf("%s", data);
	for (int i=0; i<200; i++)
	printf("%c", *(data-100+i) );
}

void notExecutableProcedure()
{
	printf("\nSecond procedure is executing\n");
}

#pragma pack(pop)