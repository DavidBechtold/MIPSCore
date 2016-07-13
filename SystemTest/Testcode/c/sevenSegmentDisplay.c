unsigned int *address = (unsigned int*) 0xA0; //Decimal 160

void showValue(unsigned char value);
unsigned int segmentA(void);
unsigned int segmentB(void);
unsigned int segmentC(void);
unsigned int segmentD(void);
unsigned int segmentE(void);
unsigned int segmentF(void);
unsigned int segmentG(void);

_start()
{
	int i = 0;
	while(1)
	{
		showValue(i++);
		if(i >= 10)
			i = 0;
	}
	return 0;
}

void showValue(unsigned char value)
{
	switch(value)
	{
		case 0: *address=(segmentA()|segmentB()|segmentC()|segmentD()|segmentF()|segmentE()); break;
		case 1: *address=(segmentB()|segmentC()); break;
		case 2: *address=(segmentA()|segmentB()|segmentG()|segmentE()|segmentD()); break;
		case 3: *address=(segmentA()|segmentB()|segmentC()|segmentD()| segmentG()); break;
		case 4: *address=(segmentB()|segmentF()|segmentG()|segmentC()); break;
		case 5: *address=(segmentA()|segmentF()|segmentG()|segmentC()|segmentD()); break;
		case 6: *address=(segmentA()|segmentF()|segmentG()|segmentC()|segmentD()|segmentE()); break;
		case 7: *address=(segmentA()|segmentB()|segmentC()); break;
		case 8: *address=(segmentA()|segmentB()|segmentC()|segmentD()|segmentF()|segmentE()|segmentG()); break;
		case 9: *address=(segmentA()|segmentB()|segmentC()|segmentF()|segmentG()); break;
		default: *address = 0xFFFFFFFF; break;
	}
}

unsigned int segmentA(void)
{
	return ((1<<15)|(1<<23));
}

unsigned int segmentB(void)
{
	return ((1<<30)|(1<<29));
}

unsigned int segmentC(void)
{
	return ((1<<27)|(1<<26));
}

unsigned int segmentD(void)
{
	return ((1<<17)|(1<<9));
}

unsigned int segmentE(void)
{
	return ((1<<3)|(1<<2));
}

unsigned int segmentF(void)
{
	return ((1<<5)|(1<<6));
}

unsigned int segmentG(void)
{
	return ((1<<12)|(1<<20));
}
