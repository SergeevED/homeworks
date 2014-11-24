#ifndef _LINKEDLIST_H_
#define _LINKEDLIST_H_

#include <stdio.h>
#include <stdlib.h>

struct link
{
	int val;
	link *next;
};

void addLink(int data, link** firstLink);

void displayLink(link** firstLink);

void deleteLink(int data, link** firstLink);

void cleanList(link** firstLink);

#endif