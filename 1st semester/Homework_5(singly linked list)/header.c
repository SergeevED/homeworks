#ifndef _LINKEDLIST_H_
#define _LINKEDLIST_H_

#include <stdio.h>
#include <stdlib.h>

struct link
{
	int val;
	link *next;
};

void linkList_addBack(link** firstLink, int data);

void linkList_addFront(link** firstLink, int data);

void linkList_display(link** firstLink);

void linkList_delete(link** firstLink, int data);

void linkList_clean(link** firstLink);

void linkList_reverse(link **firstLink);

void linkList_deleteLeadingZeroes(link **firstLink);

#endif