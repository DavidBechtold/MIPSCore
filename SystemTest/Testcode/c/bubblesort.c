#include <stdbool.h>

static int array[] = {7,3,4,5,6,1};
static int length = 6;

void bubblesort(int *array, int length);

_start()
{
	bubblesort(array, length);
	return 0;
}

void bubblesort(int *array, int length)
{
     int i, j;
     for (i = 0; i < length - 1; ++i) 
     {

 				for (j = 0; j < length - i - 1; ++j) 
        {
 	    			if (array[j] > array[j + 1]) 
            {
					 		int tmp = array[j];
					 		array[j] = array[j + 1];
					 		array[j + 1] = tmp;
					 	}
 				}
     }
 }
