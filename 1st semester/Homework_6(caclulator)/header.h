#ifndef _LINKEDLIST_H_
#define _LINKEDLIST_H_

#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <stdlib.h>
#include <math.h>

struct link
{
	short val;
	link *next;
};

struct intLink
{
	short sign;
	link *head;
};

void linkList_addFront(link** firstLink, int data);

void linkList_display(link** firstLink);

void linkList_delete(link** firstLink, int data);

void linkList_clean(link** firstLink);

void linkList_reverse(link **firstLink);

void linkList_deleteLeadingZeroes(link **firstLink);

intLink intLink_scanNum(bool* is_correct, char *operation);

intLink intLink_calcResult(intLink firstNum, intLink secondNum, char operation);

intLink intLink_sumNum(intLink firstNum, intLink secondNum);

intLink intLink_subtractNum(intLink firstNum, intLink secondNum);

intLink intLink_multiplicateNum(intLink firstNum, intLink secondNum);


void intLink_deleteNumbs(intLink *firstNum, intLink *secondNum, intLink *thirdNum);


#endif