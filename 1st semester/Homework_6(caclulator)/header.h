#ifndef _LINKEDLIST_H_
#define _LINKEDLIST_H_

#include <stdio.h>
#include <stdlib.h>
#include <math.h>

struct link
{
	int val;
	link *next;
};

void addLinkBack(int data, link** firstLink);

void addLinkFront(int data, link** firstLink);

void displayLink(link** firstLink);

void deleteLink(int data, link** firstLink);

void cleanList(link** firstLink);

link* scanNum(int *sign, bool* is_correct);

link* calcResult(link *firstLink, link *secondLink, int firstSign, int secondSign, int *resultSign);

link* sumNum(link *firstLink, link *secondLink);

link* subtractNum(link *firstLink, link *secondLink, int *resultSign);

#endif