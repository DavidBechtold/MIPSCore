const unsigned int msb = 0x80000000;
unsigned int *address = (unsigned int*) 0xA0;


_start()
{
	*address = 1;
	while(1)
	{
		if((*address & msb) == msb)
			*address = 1;
		else
			*address = *address << 1;
	}
}
