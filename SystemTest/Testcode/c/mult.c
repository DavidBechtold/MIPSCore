long multi(int a, int b);

_start()
{
	long a = multi(1E9,5);
	int b = a >> 3;
	
	return 0;
}

long multi(int a, int b)
{ 
	return (long)a * (long)b;
}
