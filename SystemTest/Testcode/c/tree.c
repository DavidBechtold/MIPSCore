#include <stdlib.h>

typedef struct _node node;
typedef node *Tree;

struct _node { 
    int value; 
    struct _node *left; 
    struct _node *right; 
};

unsigned int *ledAddress = (unsigned int*) 0xA0;


Tree *NewTree(int value);
void DeleteTree(Tree *tree);
void AddNumberToTree(Tree *tree, int value);

void *malloc(unsigned int size);
void free(void *ptr);

_start()
{
	Tree* tree = NewTree(5);
	AddNumberToTree(tree, 6);
	AddNumberToTree(tree, 4);
	AddNumberToTree(tree, 5);

	DeleteTree(tree);
	return 0;
}

Tree *NewTree(int value)
{
	node *tree =  (node*) malloc(sizeof(node));

	tree->value = value;
	tree->left = NULL;
	tree->right = NULL;
	return (Tree*) tree;
}

void DeleteTree(Tree *tree)
{
	node *tmp = (node*) tree;

	if(tmp == NULL)
		return;

	if(tmp->left != NULL)
		DeleteTree((Tree *) tmp->left);	
	if(tmp->right != NULL) 
		DeleteTree((Tree *) tmp->right);

	free(tmp);
	tmp = NULL;
}

void AddNumberToTree(Tree *tree, int value)
{
	node *tmp = (node*) tree;

	if(tmp == NULL)
		return;

	if(tmp->value == value)
		return;

	if(value > tmp->value)
	{
		if(tmp->left == NULL)
		{
			tmp->left = (node*) malloc(sizeof(node));
			tmp->left->value = value;
			tmp->left->left = NULL;
			tmp->left->right = NULL;
		}
		else
			AddNumberToTree((Tree *) tmp->left, value);
	}
	else
	{
		if(tmp->right == NULL)
		{
			tmp->right = (node*) malloc(sizeof(node));
			tmp->right->value = value;
			tmp->right->left = NULL;
			tmp->right->right = NULL;
		}
		else
			AddNumberToTree((Tree *) tmp->right, value);
	}
}

unsigned int address = 8;
void *malloc(unsigned int size)
{
	//TODO make better implementation with linked list
	if(size == 0)
		return;

	unsigned int startAddress = address;
	address += size;
	return (void *) startAddress;
}

void free(void *ptr)
{
	//TODO 
}




